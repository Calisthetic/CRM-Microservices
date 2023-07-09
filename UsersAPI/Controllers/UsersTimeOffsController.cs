using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAPI.Models.DB;
using UsersAPI.Models.Incoming;
using UsersAPI.Models.Outgoing;

namespace UsersAPI.Controllers
{
    [Route("api/time_off")]
    [ApiController]
    public class UsersTimeOffsController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;

        public UsersTimeOffsController(CrmContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/time_off
        [HttpGet]
        public async Task<ActionResult<List<TimeOffWithUserDto>>> GetUsersTimeOffs()
        {
            if (_context.UsersTimeOffs == null)
            {
                return NotFound();
            }
            return Ok(await _mapper.From(_context.UsersTimeOffs.Include(x => x.User)
                .Where(x => x.EndTimeOff > DateTime.Now)).ProjectToType<TimeOffWithUserDto>().ToListAsync());
        }

        // GET: api/time_off/5
        [HttpGet("{user_id}")]
        public async Task<ActionResult<TimeOffInfoDto>> GetUsersTimeOff(int user_id)
        {
            if (_context.UsersTimeOffs == null)
            {
                return NotFound();
            }
            var usersTimeOff = await _context.UsersTimeOffs.Where(x => x.EndTimeOff > DateTime.Now && x.UserId == user_id).FirstOrDefaultAsync();

            if (usersTimeOff == null)
            {
                return NotFound();
            }

            return usersTimeOff.Adapt<TimeOffInfoDto>();
        }

        // PUT: api/time_off/5
        [HttpPut("{user_id}")]
        public async Task<IActionResult> PutUsersTimeOff(int user_id, TimeOffAddUpdateDto timeOff)
        {
            if (user_id != timeOff.UserId)
            {
                return BadRequest();
            }

            var existTimeOff = await _context.UsersTimeOffs.FirstOrDefaultAsync(x => x.EndTimeOff > DateTime.Now && x.UserId == user_id);
            if (existTimeOff == null)
                return NotFound();

            if (DateTime.TryParse(timeOff.StartTimeOff, out DateTime resultStart) == false)
                return BadRequest(new ErrorDto() { Error = "Unable to parse start time off. Use 01/01/2000 format instead" });
            if (DateTime.TryParse(timeOff.EndTimeOff, out DateTime resultEnd) == false)
                return BadRequest(new ErrorDto() { Error = "Unable to parse end time off. Use 01/01/2000 format instead" });

            if (resultEnd < DateTime.Now)
                return BadRequest(new ErrorDto() { Error = "End of time off must be after the current date" });
            if (resultStart > DateTime.Now.AddDays(31))
                return BadRequest(new ErrorDto() { Error = "Time off can't start after 30 days from current date" });
            if (resultStart >= resultEnd)
                return BadRequest(new ErrorDto() { Error = "The beginning of the time off can be no later than the end" });
            if (resultStart.AddDays(28) < resultEnd)
                return BadRequest(new ErrorDto() { Error = "Time off can last no more than 4 weeks" });

            var newTimeOff = timeOff.Adapt<UsersTimeOff>();
            newTimeOff.UserTimeOffId = existTimeOff.UserId;
            _context.Entry(newTimeOff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new ErrorDto() { Error = "An error occurred while saving changes" });
            }

            return NoContent();
        }

        // POST: api/time_off
        [HttpPost]
        public async Task<ActionResult<UsersTimeOff>> PostUsersTimeOff(TimeOffAddUpdateDto timeOff)
        {
            if (_context == null || _context.UsersTimeOffs == null)
            {
                return Problem("Entity set 'CrmContext.UsersTimeOffs' is null.");
            }

            if ((_context.Users?.Any(x => x.UserId == timeOff.UserId)).GetValueOrDefault() == false)
                return BadRequest(new ErrorDto() { Error = "User doesn't exest" });
            if ((_context.UsersTimeOffs?.Any(x => x.UserId == timeOff.UserId && x.EndTimeOff > DateTime.Now)).GetValueOrDefault() == true)
                return BadRequest(new ErrorDto() { Error = "User's time off already exist. Change the existing" });

            if (DateTime.TryParse(timeOff.StartTimeOff, out DateTime resultStart) == false)
                return BadRequest(new ErrorDto() { Error = "Unable to parse start time off. Use 01/01/2000 format instead" });
            if (DateTime.TryParse(timeOff.EndTimeOff, out DateTime resultEnd) == false)
                return BadRequest(new ErrorDto() { Error = "Unable to parse end time off. Use 01/01/2000 format instead" });

            if (resultEnd < DateTime.Now) 
                return BadRequest(new ErrorDto() { Error = "End of time off must be after the current date" });
            if (resultStart > DateTime.Now.AddDays(31))
                return BadRequest(new ErrorDto() { Error = "Time off can't start after 30 days from current date" });
            if (resultStart >= resultEnd)
                return BadRequest(new ErrorDto() { Error = "The beginning of the time off can be no later than the end" });
            if (resultStart.AddDays(28) < resultEnd)
                return BadRequest(new ErrorDto() { Error = "Time off can last no more than 4 weeks" });

            var newTimeOff = timeOff.Adapt<UsersTimeOff>();
            await _context.UsersTimeOffs.AddAsync(newTimeOff);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostUsersTimeOff", new { id = newTimeOff.UserTimeOffId }, newTimeOff);
        }

        // DELETE: api/time_off/5
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
    }
}
