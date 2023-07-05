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
using UsersAPI.Models.DTOs.Incoming;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public PermissionsController(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Permissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
          if (_context.Permissions == null)
          {
              return NotFound();
          }
            return await _mapper.From(_context.Permissions).ProjectToType<PermissionDto>().ToListAsync();
        }

        // PUT: api/Permissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermission(int id, Permission permission)
        {
            if (id != permission.PermissionId)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(id))
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

        // POST: api/Permissions
        [HttpPost]
        public async Task<ActionResult<Permission>> PostPermission(PermissionAddNew permission)
        {
            if (_context.Permissions == null)
            {
                return Problem("Entity set 'CrmContext.Permissions' is null.");
            }
            var newPermission = permission.Adapt<Permission>();
            _context.Permissions.Add(newPermission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPermission", new { id = newPermission.PermissionId }, newPermission);
        }

        // DELETE: api/Permissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            if (_context.Permissions == null)
            {
                return NotFound();
            }
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PermissionExists(int id)
        {
            return (_context.Permissions?.Any(e => e.PermissionId == id)).GetValueOrDefault();
        }
    }
}
