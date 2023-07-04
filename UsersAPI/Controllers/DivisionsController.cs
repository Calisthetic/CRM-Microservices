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
using UsersAPI.Models.DTOs.Incoming.Divisions;
using UsersAPI.Models.DTOs.Outgoing;
using UsersAPI.Models.DTOs.Outgoing.Divisions;

namespace UsersAPI.Controllers
{
    [Route("api/user/division")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public DivisionsController(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/user/division/tree
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<DivisionsTreeDto>>> GetDivisions()
        {
            if (_context.Divisions == null)
            {
                return NotFound();
            }

            return Ok(await _mapper.From(_context.Divisions.Where(x => x.UpperDivisionId == null)
                .Include(x => x.InverseUpperDivision)
                .ThenInclude(x => x.InverseUpperDivision)
                .ThenInclude(x => x.InverseUpperDivision)
                .ThenInclude(x => x.InverseUpperDivision)
            ).ProjectToType<DivisionsTreeDto>().ToListAsync());
        }

        // GET: api/user/division/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DivisionInfoWithAddsDto>> GetDivision(int id)
        {
          if (_context.Divisions == null)
          {
              return NotFound();
          }
            var division = await _context.Divisions
                .Include(x => x.DivisionPrefix).Include(x => x.Company)
                .Include(x => x.UpperDivision).Include(x => x.InverseUpperDivision)
                .FirstOrDefaultAsync(x => x.DivisionId == id);

            if (division == null)
            {
                return NotFound();
            }

            return division.Adapt<DivisionInfoWithAddsDto>();
        }

        // PUT: api/user/division/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDivision(int id, DivisionUpdateDto division)
        {
            if (id != division.DivisionId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var currentDivision = await _context.Divisions.FindAsync(id);
            if (currentDivision == null)
            {
                return NotFound();
            }

            if (division.LowerDivisionId.HasValue && division.UpperDivisionId.HasValue)
            {
                var lowerDivision = await _context.Divisions.Include(x => x.UpperDivision).FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower division don't exists" });
                }

                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.UpperDivisionId);
                if (upperDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper division don't exists" });
                }

                // Если нижний ссылается на высший
                if (lowerDivision.UpperDivisionId == upperDivision.DivisionId ||
                    lowerDivision?.UpperDivision?.UpperDivisionId == upperDivision.DivisionId)
                { // Либо нижний -> высший -> высший
                    currentDivision.DivisionName = division.DivisionName;
                    currentDivision.CompanyId = division.CompanyId;
                    currentDivision.DivisionPrefixId = division.DivisionPrefixId;

                    lowerDivision.UpperDivisionId = division.DivisionId;
                    await _context.SaveChangesAsync();

                    return NoContent();
                }
                return BadRequest(new ErrorDto() { Error = "Incorrect upper and lower divisions" });
            }
            else if (division.LowerDivisionId.HasValue)
            {
                var lowerDivision = await _context.Divisions.Include(x => x.UpperDivision).FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower division don't exists" });
                }

                if (lowerDivision.UpperDivisionId != null)
                { // Если у подчинённой есть главенствующая роль
                    var upperDivisionOfLower = await _context.Divisions.FirstOrDefaultAsync(x => x.DivisionId == lowerDivision.UpperDivisionId);
                    if (upperDivisionOfLower == null)
                    {
                        return BadRequest(new ErrorDto() { Error = "Database error" });
                    }

                    if (upperDivisionOfLower.UpperDivisionId.HasValue)
                    {
                        return BadRequest(new ErrorDto() { Error = "Please specify upper division" });
                    }
                }

                currentDivision.DivisionName = division.DivisionName;
                currentDivision.CompanyId = division.CompanyId;
                currentDivision.DivisionPrefixId = division.DivisionPrefixId;

                lowerDivision.UpperDivisionId = division.DivisionId;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else if (division.UpperDivisionId.HasValue)
            {
                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (upperDivision != null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper division don't exists" });
                }

                currentDivision.DivisionName = division.DivisionName;
                currentDivision.CompanyId = division.CompanyId;
                currentDivision.DivisionPrefixId = division.DivisionPrefixId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                currentDivision.DivisionName = division.DivisionName;
                currentDivision.CompanyId = division.CompanyId;
                currentDivision.DivisionPrefixId = division.DivisionPrefixId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        //POST: api/user/division
        [HttpPost]
        public async Task<ActionResult<Division>> PostDivision(DivisionAddNewDto division)
        {
            if (_context.Divisions == null)
            {
                return Problem("Entity set 'CrmContext.Divisions' is null.");
            }

            if (string.IsNullOrEmpty(division.DivisionName))
            {
                return BadRequest(new ErrorDto() { Error = "Incorrect" });
            }

            if (division.LowerDivisionId.HasValue && division.UpperDivisionId.HasValue)
            {
                var lowerDivision = await _context.Divisions.Include(x => x.UpperDivision).FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower division don't exists" });
                }

                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.UpperDivisionId);
                if (upperDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper division don't exists" });
                }

                // Если нижний ссылается на высший
                if (lowerDivision.UpperDivisionId == upperDivision.DivisionId ||
                    lowerDivision?.UpperDivision?.UpperDivisionId == upperDivision.DivisionId)
                { // Либо нижний -> высший -> высший
                    var newDivision = division.Adapt<Division>();
                    await _context.Divisions.AddAsync(newDivision);
                    await _context.SaveChangesAsync();
                    lowerDivision.UpperDivisionId = newDivision.DivisionId;
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("CreateDivision", new { id = newDivision.DivisionId }, newDivision);
                }
                return BadRequest(new ErrorDto() { Error = "Incorrect upper and lower divisions" });
            }
            else if (division.LowerDivisionId.HasValue)
            {
                var lowerDivision = await _context.Divisions.Include(x => x.UpperDivision).FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new ErrorDto() { Error = "Lower division don't exists" });
                }

                if (lowerDivision.UpperDivisionId != null)
                { // Если у подчинённой есть главенствующая роль
                    var upperDivisionOfLower = await _context.Divisions.FirstOrDefaultAsync(x => x.DivisionId == lowerDivision.UpperDivisionId);
                    if (upperDivisionOfLower == null)
                    {
                        return BadRequest(new ErrorDto() { Error = "Database error" });
                    }

                    if (upperDivisionOfLower.UpperDivisionId.HasValue)
                    {
                        return BadRequest(new ErrorDto() { Error = "Please specify upper division" });
                    }
                }

                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                await _context.SaveChangesAsync();
                lowerDivision.UpperDivisionId = newDivision.DivisionId;
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateDivision", new { id = newDivision.DivisionId }, newDivision);
            }
            else if (division.UpperDivisionId.HasValue)
            {
                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (upperDivision != null)
                {
                    return BadRequest(new ErrorDto() { Error = "Upper division don't exists" });
                }

                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateDivision", new { id = newDivision.DivisionId }, newDivision);
            }
            else
            {
                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                await _context.SaveChangesAsync();

                return CreatedAtAction("CreateDivision", new { id = newDivision.DivisionId }, newDivision);
            }
        }

        // DELETE: api/user/division/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDivision(int id)
        {
            if (_context.Divisions == null)
            {
                return NotFound();
            }
            var division = await _context.Divisions.FindAsync(id);
            if (division == null)
            {
                return NotFound();
            }

            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DivisionExists(int id)
        {
            return (_context.Divisions?.Any(e => e.DivisionId == id)).GetValueOrDefault();
        }
    }
}
