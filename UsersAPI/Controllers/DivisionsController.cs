﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.DB;
using UsersAPI.Models.DTOs.Outgoing;

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

        // GET: api/Divisions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Division>>> GetDivisions()
        {
            if (_context.Divisions == null)
            {
                return NotFound();
            }
            //return Ok(_context.Divisions.Where(x => x.UpperDivisionId == null).Include(x => x.InverseUpperDivision));
            return Ok(_mapper.From(_context.Divisions.Where(x => x.UpperDivisionId == null).Include(x => x.InverseUpperDivision)).ProjectToType<DivisionsThreeDto>());
        }

        // GET: api/Divisions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Division>> GetDivision(int id)
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

            return division;
        }

        // PUT: api/Divisions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDivision(int id, Division division)
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

        // POST: api/Divisions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Division>> PostDivision(Division division)
        {
          if (_context.Divisions == null)
          {
              return Problem("Entity set 'CrmContext.Divisions'  is null.");
          }
            _context.Divisions.Add(division);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDivision", new { id = division.DivisionId }, division);
        }

        // DELETE: api/Divisions/5
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