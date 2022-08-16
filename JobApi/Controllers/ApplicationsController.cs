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
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Applications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }
          return await _context.Applications
              .Include(a => a.Job).ThenInclude(j => j.Company)
              .Include(a => a.User)
              .ToListAsync();
        }

        // GET: api/Applications/5
        [HttpGet("{userId}/{jobId}")]
        public async Task<ActionResult<Application>> GetApplication(string userId, int jobId)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }
            var application = await _context.Applications
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.JobId == jobId && a.UserId == userId);

            if (application == null)
            {
                return NotFound();
            }

            return application;
        }
        
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsByUser(string id)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }

            return await _context.Applications
                .Where(a => a.UserId == id)
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.User)
                .ToListAsync();
        }

        [HttpGet("job/{id}")]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplicationsByJob(int id)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }

            return await _context.Applications
                .Where(a => a.JobId == id)
                .Include(a => a.Job)
                .Include(a => a.User)
                .ToListAsync();
        }

        // PUT: api/Applications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{user}/{job}")]
        public async Task<IActionResult> PutApplication(string user, int job, Application application)
        {
            if (job != application.JobId || user != application.UserId)
            {
                return BadRequest();
            }

            _context.Entry(application).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(user, job))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Applications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Application>> PostApplication(Application application)
        {
            if (_context.Applications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Applications'  is null.");
            }
            _context.Applications.Add(application);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApplicationExists(application.UserId, application.JobId))
                {
                    return Conflict();
                }

                throw;
            }

            return application;
        }

        // DELETE: api/Applications/5
        [HttpDelete("{user}/{job}")]
        public async Task<IActionResult> DeleteApplication(string user, int job)
        {
            if (_context.Applications == null)
            {
                return NotFound();
            }
            Application? application = _context.Applications.First(a => a.UserId == user && a.JobId == job);
            if (application == null)
            {
                return NotFound();
            }

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApplicationExists(string user, int job)
        {
            return (_context.Applications?.Any(e => e.JobId == job && e.UserId == user)).GetValueOrDefault();
        }
    }
}
