using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.DB;
using UsersAPI.Models.DTOs.Incoming.Companies;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public CompaniesController(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Companies
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CompanyInfoDto>>> GetCompanies()
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }

            return Ok(await _mapper.From(_context.Companies).ProjectToType<CompanyInfoDto>().ToListAsync());
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyInfoDto>> GetCompany(int id)
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

            return company.Adapt<CompanyInfoDto>();
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, CompanyUpdateDto company)
        {
            if (id != company.CompanyId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentCompany = await _context.Companies.FindAsync(id);
            if (currentCompany == null)
            {
                return NotFound();
            }

            if (company.LowerCompanyId.HasValue && company.UpperCompanyId.HasValue)
            {
                var lowerCompany = await _context.Companies.Include(x => x.UpperCompany).FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (lowerCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower company don't exists" });
                }

                var upperCompany = await _context.Companies.FirstOrDefaultAsync(e => e.CompanyId == company.UpperCompanyId);
                if (upperCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper company don't exists" });
                }

                // Если нижний ссылается на высший
                if (lowerCompany.UpperCompanyId == upperCompany.CompanyId ||
                    lowerCompany.UpperCompany?.UpperCompanyId == upperCompany.CompanyId)
                { // Либо нижний -> высший -> высший
                    currentCompany.CompanyName = company.CompanyName;

                    lowerCompany.UpperCompanyId = company.CompanyId;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                return BadRequest(new ErrorDto() { Error = "Incorrect upper and lower companies" });
            }
            else if (company.LowerCompanyId.HasValue)
            {
                var lowerCompany = await _context.Companies.Include(x => x.UpperCompany).FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (lowerCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower company don't exists" });
                }

                if (lowerCompany.UpperCompany != null)
                { // Если у подчинённой есть главенствующая роль
                    var upperCompanyOfLower = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyId == lowerCompany.UpperCompanyId);
                    if (upperCompanyOfLower == null)
                    {
                        return BadRequest(new ErrorDto() { Error = "Database error" });
                    }

                    if (upperCompanyOfLower.UpperCompanyId.HasValue)
                    {
                        return BadRequest(new ErrorDto() { Error = "Please specify upper company" });
                    }
                }

                currentCompany.CompanyName = company.CompanyName;

                lowerCompany.UpperCompanyId = company.CompanyId;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else if (company.UpperCompanyId.HasValue)
            {
                var upperCompany = await _context.Companies.FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (upperCompany != null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper company don't exists" });
                }

                currentCompany.CompanyName = company.CompanyName;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                currentCompany.CompanyName = company.CompanyName;

                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        // POST: api/Companies
        [HttpPost]
        public async Task<ActionResult<Company>> CreateCompany(CompanyAddNewDto company)
        {
            if (_context.Companies == null)
            {
                return Problem("Entity set 'CrmContext.Companies'  is null.");
            }

            if (string.IsNullOrEmpty(company.CompanyName))
            {
                return BadRequest(new ErrorDto() { Error = "Incorrect" });
            }

            if (company.LowerCompanyId.HasValue && company.UpperCompanyId.HasValue)
            {
                var lowerCompany = await _context.Companies.Include(x => x.UpperCompany).FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (lowerCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower company don't exists" });
                }

                var upperCompany = await _context.Companies.FirstOrDefaultAsync(e => e.CompanyId == company.UpperCompanyId);
                if (upperCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper company don't exists" });
                }

                // Если нижний ссылается на высший
                if (lowerCompany.UpperCompanyId == upperCompany.CompanyId ||
                    lowerCompany.UpperCompany?.UpperCompanyId == upperCompany.CompanyId ||
                    lowerCompany.UpperCompanyId == null)
                { // Либо нижний -> высший -> высший
                    var newCompany = company.Adapt<Company>();
                    await _context.Companies.AddAsync(newCompany);
                    await _context.SaveChangesAsync();
                    lowerCompany.UpperCompanyId = newCompany.CompanyId;
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("CreateCompany", new { id = newCompany.CompanyId }, newCompany);
                }
                return BadRequest(new ErrorDto() { Error = "Incorrect upper and lower companies" });
            }
            else if (company.LowerCompanyId.HasValue)
            {
                var lowerCompany = await _context.Companies.Include(x => x.UpperCompany).FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (lowerCompany == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower company don't exists" });
                }

                if (lowerCompany.UpperCompanyId != null)
                { // Если у подчинённой есть главенствующая компания
                    var upperCompanyOfLower = await _context.Companies.FirstOrDefaultAsync(x => x.CompanyId == lowerCompany.UpperCompanyId);
                    if (upperCompanyOfLower == null)
                    {
                        return BadRequest(new ErrorDto() { Error = "Database error" });
                    }

                    if (upperCompanyOfLower.UpperCompanyId.HasValue)
                    {
                        return BadRequest(new ErrorDto() { Error = "Please specify upper company" });
                    }
                }

                var newCompany = company.Adapt<Company>();
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();
                lowerCompany.UpperCompanyId = newCompany.CompanyId;
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateCompany", new { id = newCompany.CompanyId }, newCompany);
            }
            else if (company.UpperCompanyId.HasValue)
            {
                var upperCompany = await _context.Companies.FirstOrDefaultAsync(e => e.CompanyId == company.LowerCompanyId);
                if (upperCompany != null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper company don't exists" });
                }

                var newCompany = company.Adapt<Company>();
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateCompany", new { id = newCompany.CompanyId }, newCompany);
            }
            else
            {
                var newCompany = company.Adapt<Company>();
                await _context.Companies.AddAsync(newCompany);
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateCompany", new { id = newCompany.CompanyId }, newCompany);
            }
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
            return (_context.Companies?.Any(e => e.CompanyId == id)).GetValueOrDefault();
        }
    }
}
