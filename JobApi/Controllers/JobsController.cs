using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobApi.Model;

namespace JobApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }
            return await _context.Jobs
                .Include(j => j.Categories)
                .Include(j => j.Company)
                .Include(j => j.Sector)
                .ToListAsync();
        }

        [HttpGet("company/{id}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsByCompany(int id)
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }
            return await _context.Jobs
                .Include(j => j.Categories)
                .Include(j => j.Company)
                .Include(j => j.Sector)
                .Where(j => j.CompanyId == id)
                .ToListAsync();
        }

        // GET: api/Jobs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }
            Job? job = await _context.Jobs
                .Include(j => j.Categories)
                .Include(j => j.Company).ThenInclude(c => c!.Jobs)
                .Include(j => j.Sector)
                .Include(j => j.Applications).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            job.Company!.JobCount = job.Company.Jobs.Count;
            job.Company.Jobs = new List<Job>();

            return job;
        }

        // PUT: api/Jobs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJob(int id, Job input)
        {
            if (id != input.Id || await _context.Jobs.FindAsync(id) == null)
            {
                return BadRequest();
            }

            Job job = (await _context.Jobs.Include(j => j.Categories).FirstOrDefaultAsync(j => j.Id == id))!;
            job.ContractType = input.ContractType;
            job.Description = input.Description;
            job.Profile = input.Profile;
            job.Offer = input.Offer;
            job.Location = input.Location;
            job.Name = input.Name;
            job.Salary = input.Salary;
            job.Deadline = input.Deadline;
            job.SectorId = input.SectorId == 0 ? null : input.SectorId;
            job.Categories.Where(c => !input.Categories.Select(c => c.Id).Contains(c.Id)).ToList()
                .ForEach(c => job.Categories.Remove(c));
            IEnumerable<int> existingCats = job.Categories.Select(m => m.Id);
            IEnumerable<Category> addCats = input.Categories.Where(c => !existingCats.Contains(c.Id));
            foreach (Category c in addCats)
            {
                job.Categories.Add(_context.Categories.First(iC => iC.Id == c.Id));
            }
            _context.Entry(job).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // POST: api/Jobs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Job>> PostJob(Job job)
        {
            if (_context.Jobs == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Jobs' is null.");
            }
            var categories = job.Categories;
            job.Categories = new List<Category>();
            job.SectorId = job.SectorId == 0 ? null : job.SectorId;

            _context.Jobs.Add(job);
            foreach (Category c in categories)
            {
                job.Categories.Add(_context.Categories.Single(cat => cat.Id == c.Id));
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (JobExists(job.Id))
                {
                    return Conflict();
                }
                throw;
            }

            return CreatedAtAction("GetJob", new { id = job.Id }, job);
        }

        // DELETE: api/Jobs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }
            Job? job = await _context.Jobs.Include(j => j.Categories).FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
            {
                return NotFound();
            }
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobExists(int id)
        {
            return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Job>>> SearchJobs(string type, string terms, int[] categories, int sector, int company, string salary)
        {
            if (_context.Jobs == null)
            {
                return NotFound();
            }

            var jobs = _context.Jobs.Where(j => true);

            if (company != 0)
            {
                jobs = jobs.Where(j => j.CompanyId == company);
            }

            if (sector != 0)
            {
                jobs = jobs.Where(j => j.SectorId == sector);
            }

            jobs = jobs.Include(j => j.Categories);

            if (categories != null)
            {
                jobs = jobs.Where(j => j.Categories.Any(c => categories.Contains(c.Id)));
            }

            //Other search params, I'm lazy now

            jobs = jobs.Include(j => j.Company).Include(j => j.Sector);

            var jobList = await jobs.ToListAsync();

            return await jobs.ToListAsync();
        }
    }
}
