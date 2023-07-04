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

            _context.Entry(division).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DivisionExists(id))
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

        //POST: api/user/division
        [HttpPost]
        public async Task<ActionResult<Division>> PostDivision(DivisionAddNewDto division)
        {
            if (_context.Divisions == null)
            {
                return Problem("Entity set 'CrmContext.Divisions' is null.");
            }

            if (division.LowerDivisionId != null && division.UpperDivisionId != null)
            {
                var lowerDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new BadResponseDto() { Error = "Lower division don't exists" });
                }

                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (upperDivision == null)
                {
                    return BadRequest(new BadResponseDto() { Error = "Upper division don't exists" });
                }

                if (lowerDivision.UpperDivisionId != upperDivision.DivisionId)
                {
                    return BadRequest(new BadResponseDto() { Error = "Incorrect upper and lower divisions" });
                }

                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                lowerDivision.UpperDivisionId = newDivision.DivisionId;
                await _context.SaveChangesAsync();

                return Ok(); // Created at
            }
            else if (division.LowerDivisionId.HasValue)
            {
                // Подчинённая роль существует
                var lowerDivision = await _context.Divisions.Include(x => x.UpperDivision).FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (lowerDivision == null)
                {
                    return BadRequest(new BadResponseDto() { Error = "Lower division don't exists" });
                }

                // Если у подчинённой есть главенствующая роль
                if (lowerDivision.UpperDivision != null)
                {
                    var upperDivisionOfLower = await _context.Divisions.FirstOrDefaultAsync(x => x.DivisionId == lowerDivision.UpperDivisionId);
                    if (upperDivisionOfLower == null)
                    {
                        return BadRequest(new BadResponseDto() { Error = "Database error" });
                    }

                    if (upperDivisionOfLower.UpperDivisionId.HasValue)
                    {
                        return BadRequest(new BadResponseDto() { Error = "Please specify upper division" });
                    }
                }

                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                lowerDivision.UpperDivisionId = newDivision.DivisionId;
                await _context.SaveChangesAsync();

                return Ok(); // Created at
            }
            else if (division.UpperDivisionId.HasValue)
            {
                var upperDivision = await _context.Divisions.FirstOrDefaultAsync(e => e.DivisionId == division.LowerDivisionId);
                if (upperDivision != null)
                {
                    return BadRequest(new BadResponseDto() { Error = "Upper division don't exists" });
                }

                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                await _context.SaveChangesAsync();

                return Ok(); // Created at
            }
            else
            {
                var newDivision = division.Adapt<Division>();
                await _context.Divisions.AddAsync(newDivision);
                await _context.SaveChangesAsync();

                return Ok(); // Created at
            }
            //return CreatedAtAction("GetDivision", new { id = division.DivisionId }, division);
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
