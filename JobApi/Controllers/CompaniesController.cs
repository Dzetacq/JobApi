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
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var withJobs = await _context.Companies.Include(c => c.Jobs).ToListAsync();
            withJobs.ForEach(c => c.JobCount = c.Jobs.Count);
            withJobs.ForEach(c => c.Jobs = new List<Job>());
            return withJobs;
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies
                .Include(c => c.Admin)
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            company.JobCount = company.Jobs.Count;
            return company;
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompaniesByUser(string id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            List<Company>? companies = await _context.Companies
                .Include(c => c.Admin)
                .Include(c => c.Jobs)
                .Where(c => c.Admin != null && c.Admin.Id == id)
                .ToListAsync();

            if (companies == null)
            {
                return NotFound();
            }
            companies.ForEach(c => c.JobCount = c.Jobs.Count);
            companies.ForEach(c => c.Jobs = new List<Job>());
            return companies;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<Company>> PutCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            if (company.AdminId is { Length: 0 })
            {
                company.AdminId = null;
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return _context.Companies.Include(c => c.Admin).Include(c => c.Jobs).First(c => c.Id == company.Id);
        }

        // POST: api/Companies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
            }

            if (company.AdminId is { Length: 0 })
            {
                company.AdminId = null;
            }
            _context.Companies.Add(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyExists(company.Id))
                {
                    return Conflict();
                }

                throw;
            }

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
