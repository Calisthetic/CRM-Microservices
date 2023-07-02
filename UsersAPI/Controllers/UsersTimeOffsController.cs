using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.DB;

namespace UsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersTimeOffsController : ControllerBase
    {
        private readonly CrmContext _context;

        public UsersTimeOffsController(CrmContext context)
        {
            _context = context;
        }

        // GET: api/UsersTimeOffs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersTimeOff>>> GetUsersTimeOffs()
        {
            if (_context.UsersTimeOffs == null)
            {
                return NotFound();
            }
            return await _context.UsersTimeOffs.ToListAsync();
        }

        // GET: api/UsersTimeOffs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsersTimeOff>> GetUsersTimeOff(int id)
        {
          if (_context.UsersTimeOffs == null)
          {
              return NotFound();
          }
            var usersTimeOff = await _context.UsersTimeOffs.FindAsync(id);

            if (usersTimeOff == null)
            {
                return NotFound();
            }

            return usersTimeOff;
        }

        // PUT: api/UsersTimeOffs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersTimeOff(int id, UsersTimeOff usersTimeOff)
        {
            if (id != usersTimeOff.UserTimeOffId)
            {
                return BadRequest();
            }

            _context.Entry(usersTimeOff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersTimeOffExists(id))
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

        // POST: api/UsersTimeOffs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UsersTimeOff>> PostUsersTimeOff(UsersTimeOff usersTimeOff)
        {
          if (_context.UsersTimeOffs == null)
          {
              return Problem("Entity set 'CrmContext.UsersTimeOffs'  is null.");
          }

            // check if already exust for now

            _context.UsersTimeOffs.Add(usersTimeOff);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsersTimeOff", new { id = usersTimeOff.UserTimeOffId }, usersTimeOff);
        }

        // DELETE: api/UsersTimeOffs/5
        [HttpDelete("{user_id}")]
        public async Task<IActionResult> DeleteUsersTimeOff(int user_id)
        {
            if (_context.UsersTimeOffs == null)
            {
                return NotFound();
            }
            var usersTimeOff = await _context.UsersTimeOffs.Where(x => x.UserId == user_id && x.EndTimeOff > DateTime.Now).FirstOrDefaultAsync();
            if (usersTimeOff == null)
            {
                return NotFound();
            }

            _context.UsersTimeOffs.Remove(usersTimeOff);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersTimeOffExists(int id)
        {
            return (_context.UsersTimeOffs?.Any(e => e.UserTimeOffId == id)).GetValueOrDefault();
        }
    }
}
