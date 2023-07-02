using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using UsersAPI.Configurations;
using UsersAPI.Models.DB;
using UsersAPI.Models.DTOs.Incoming;
using UsersAPI.Models.DTOs.Outgoing;

namespace UsersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private List<int> _users = new List<int>();

        public UsersController(CrmContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: api/Users
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "sus,Ad,Adn"), Authorize(Roles = "Admidn")]
        public async Task<ActionResult<IList<UserInfoDto>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            //return Ok(await _context.Users.Include(x => x.ProfileImages)
            //    .Include(x => x.Division).ThenInclude(x => x.UpperDivision)
            //    .Include(x => x.Division).ThenInclude(x => x.Company)
            //    //.Include(x => x.Division).ThenInclude(x => x.PermissionsOfDivisions).ThenInclude(x => x.Permission)
            //    .Include(x => x.Division).ThenInclude(x => x.DivisionPrefix)
            //    .Include(x => x.UsersTimeOffs.Where(xx => xx.EndTimeOff > DateTime.Now)).ToListAsync());
            //return Ok(_context.Users);
            return Ok(_mapper.From(_context.Users.Include(x => x.UsersTimeOffs).Include(x => x.UpperUser).ThenInclude(x => x.Division)
                .Include(x => x.UpperUser).ThenInclude(x => x.Division).ThenInclude(x => x.DivisionPrefix)).ProjectToType<UserInfoDto>());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Include(x => x.ProfileImages)
                .Include(x => x.Division).ThenInclude(x => x.UpperDivision)
                .Include(x => x.Division).ThenInclude(x => x.Company)
                //.Include(x => x.Division).ThenInclude(x => x.PermissionsOfDivisions).ThenInclude(x => x.Permission)
                .Include(x => x.Division).ThenInclude(x => x.DivisionPrefix)
                .Include(x => x.UsersTimeOffs.Where(xx => xx.EndTimeOff > DateTime.Now)).FirstAsync(x => x.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("canAddTasks/{id}")]
        public async Task<ActionResult<User>> GetTaskedUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            if (!UserExists(id))
            {
                return NotFound();
            }
            
            var currentUser = await _context.Users.Include(x => x.Division)
                .Include(x => x.Division).ThenInclude(x => x.DivisionPrefix).FirstAsync(x => x.UserId == id);
            if (currentUser == null)
                return NotFound();

            List<User> userss = await HelpPls(currentUser);
            return Ok(userss);
        }

        private async Task<List<User>> HelpPls(User user)
        {
            var foundedUsers = await _context.Users.Include(x => x.Division).ThenInclude(x => x.DivisionPrefix).Include(x => x.Division)
                .Where(x => ((x.Division.DivisionPrefix.UpperDivisionPrefixId == null ? -1 : x.Division.DivisionPrefix.UpperDivisionPrefixId) == (user.Division == null ? 0 : user.Division.DivisionPrefixId))
                    && x.Division.CompanyId == (user.Division == null ? 0 : user.Division.CompanyId)
                    && x.UserId != user.UserId
                    && x.Division.DivisionName == (user.Division == null ? "null" : user.Division.DivisionName)).ToListAsync();

            if (foundedUsers == null)
                return new List<User>();
            if (foundedUsers.Count == 0)
                return new List<User>();

            var result = new List<User>();
            result.AddRange(foundedUsers);
            foreach (var foundedUser in foundedUsers) 
            {
                result.AddRange(await HelpPls(foundedUser));
            }
            return result;
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserAddNewDto user)
        {
            if (_context.Users == null)
                return Problem("Entity set 'CrmContext.Users' is null.");
            if (!ModelState.IsValid) 
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var newUser = user.Adapt<User>();
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = newUser.UserId }, newUser);
        }

        // POST: api/Users
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<SuccessLoginDto>> LoginUser(UserLoginRequestDto user)
        {
            if (_context.Users == null)
                return Problem("Entity set 'CrmContext.Users' is null.");
            if (!ModelState.IsValid)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var existUser = await _context.Users
                .Include(x => x.Division).ThenInclude(x => x.PermissionsOfDivisions).ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Login == user.Login && x.Password == user.Password);
            if (existUser == null)
                return NotFound();

            //return Ok(existUser);
            return Ok(new SuccessLoginDto() { token = GenerateToken(existUser) });
        }
        
        // POST: api/Users
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserLoginRequestDto user)
        {
            if (_context.Users == null)
                return Problem("Entity set 'CrmContext.Users' is null.");
            if (!ModelState.IsValid)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var existUser = await _context.Users.FirstOrDefaultAsync(x => x.Login == user.Login && x.Password == user.Password);
            if (existUser == null)
                return NotFound();

            return Ok(new SuccessLoginDto() { token = GenerateToken(existUser) });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!);

            var claims = new List<Claim>()
            {
                new Claim(type:"Id", value: user.UserId.ToString()),
                new Claim(ClaimTypes.Role, "Guest"),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

            };

            // Add permissions
            if (user.Division != null)
            {
                foreach (var permission in user.Division.PermissionsOfDivisions)
                {
                    if (permission.Permission.PermissionName != null)
                        claims.Add(new Claim(ClaimTypes.Role, permission.Permission.PermissionName));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}
