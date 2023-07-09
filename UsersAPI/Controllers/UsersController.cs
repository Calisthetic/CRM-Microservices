using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersAPI.Models.DB;
using UsersAPI.Models.Incoming.Users;
using UsersAPI.Models.Outgoing;
using UsersAPI.Models.Outgoing.Users;

namespace UsersAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CrmContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UsersController(CrmContext context, IMapper mapper, 
            IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        // GET: api/user
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "sus,Ad,AddUsers")]
        public async Task<ActionResult<IList<UserInfoDto>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return Ok(_mapper.From(_context.Users.Include(x => x.UsersTimeOffs.Where(xx => xx.EndTimeOff > DateTime.Now))
                .Include(x => x.UpperUser).ThenInclude(x => x.Division)
                .Include(x => x.UpperUser).ThenInclude(x => x.Division)
                .ThenInclude(x => x.DivisionPrefix)).ProjectToType<UserInfoDto>());
        }

        [HttpGet("upper/{division_id}")]
        public async Task<IActionResult> GetUpperUsersByDivision(int division_id)
        {
            if (_context.Users == null)
                return NotFound();

            var currentDivision = await _context.Divisions.FirstOrDefaultAsync(x => x.DivisionId == division_id);
            if (currentDivision == null)
                return NotFound();
            if (currentDivision.UpperDivisionId == null)
                return Ok(new List<string>());

            var resultDivision = await _context.Divisions.Include(x => x.UpperDivision).ThenInclude(x => x.Users)
                .Include(x => x.Users).Include(x => x.UpperDivision).ThenInclude(x => x.InverseUpperDivision).ThenInclude(x => x.Users)
                .FirstOrDefaultAsync(x => x.DivisionId == currentDivision.UpperDivisionId);
            if (resultDivision == null)
                return NotFound();

            // transform to users list
            var result = ConvertToUsers(resultDivision);

            return Ok(result);
        }

        private List<UpperUsersListDto> ConvertToUsers(Division? divisions)
        {
            if (divisions == null)
                return new List<UpperUsersListDto>();

            var result = new List<UpperUsersListDto>();

            foreach (var user in divisions.Users)
            {
                result.Add(user.Adapt<UpperUsersListDto>());
            }
            result.AddRange(ConvertToUsers(divisions.UpperDivision));

            return result;
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "sus,Ad,SddUsedrs")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Include(x => x.ProfileImages)
                .Include(x => x.Division).ThenInclude(x => x.UpperDivision)
                .Include(x => x.Division).ThenInclude(x => x.Company)
                .Include(x => x.Division).ThenInclude(x => x.DivisionPrefix)
                .Include(x => x.UsersTimeOffs.Where(xx => xx.EndTimeOff > DateTime.Now)).FirstOrDefaultAsync(x => x.UserId == id);

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


        // PATCH: api/users/5 ---
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/users
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

        // POST: api/users/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserAuthResultDto>> LoginUser(UserLoginRequestDto user)
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
            return Ok(await GenerateToken(existUser));
        }

        // POST: api/users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserAuthResultDto>> RegisterUser(UserLoginRequestDto user)
        {
            if (_context.Users == null)
                return Problem("Entity set 'CrmContext.Users' is null.");
            if (!ModelState.IsValid)
                return new JsonResult("Something went wrong") { StatusCode = 500 };

            var existUser = await _context.Users.FirstOrDefaultAsync(x => x.Login == user.Login && x.Password == user.Password);
            if (existUser == null)
                return NotFound();

            return Ok(await GenerateToken(existUser));
        }

        // DELETE: api/users/5
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

        private async Task<UserAuthResultDto> GenerateToken(User user)
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
                Expires = DateTime.UtcNow.Add(TimeSpan.Parse(_configuration.GetSection("JwtConfig:ExpiryTimeFrame").Value!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = RandomStringGeneration(23),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsRevoked = false,
                IsUsed = false,
                UserId = user.UserId
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new UserAuthResultDto()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        [HttpGet("error")]
        public async Task<IActionResult> GetError()
        {
            throw new DivideByZeroException();
        }

        [HttpPost("refresh_token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorDto() { Error = "Invalid parameters" });
            }

            var result = await VerifyAndGenerateToken(tokenRequest);
            if (result == null)
            {
                return BadRequest(new ErrorDto() { Error = "Invalid tokens" });
            }

            return Ok(result);
        }
        private async Task<object?> VerifyAndGenerateToken(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false; // for testing

                var tokenInVerification = jwtTokenHandler
                    .ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                        return null;
                }

                var utcExpiryDate = long.Parse(tokenInVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                    new ErrorDto() { Error = "ExpiredToken" };

                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedToken == null)
                    return new ErrorDto() { Error = "Invalid tokens" };

                if (storedToken.IsUsed)
                    return new ErrorDto() { Error = "Invalid tokens" };
                if (storedToken.IsRevoked)
                    return new ErrorDto() { Error = "Invalid tokens" };

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                    return new ErrorDto() { Error = "Invalid tokens" };

                if (storedToken.ExpiryDate < DateTime.UtcNow)
                    return new ErrorDto() { Error = "Expired tokens" };

                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                var dbUser = await _context.Users.FindAsync(storedToken.UserId);
                if (dbUser == null)
                    return new ErrorDto() { Error = "Expired tokens" };

                return await GenerateToken(dbUser);
            }
            catch
            {
                return new ErrorDto() { Error = "Server error" } ;
            }
        }
        private DateTime UnixTimeStampToDateTime(long unixTimaStamp) 
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimaStamp).ToUniversalTime();
            return dateTimeVal;
        }

        private string RandomStringGeneration(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890abcdefghijklmnoprstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
