using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

namespace WepJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly AddDbContext _context;

        public AuthController(IConfiguration configuration, AddDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSald);
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSald = passwordSald;
            user.Key = Guid.NewGuid().ToString()[..10];

            _context?.AddAsync(user);
            _context?.SaveChangesAsync();

            return Ok(user.Key);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User Not Found");
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSald))
            {
                return BadRequest("Wrong Password");
            }
            string token = CreateToken(user);
            
            return Ok(token);
        }
        [HttpPost("SignUp")]
        public IActionResult GetUserByKey(string key)
        {
            var user = _context.Users.FirstOrDefault(u => u.Key == key);
            
            if (user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
        [HttpGet("password")]
        public IActionResult GetUserKey(string password)
        {
            var userpassword = _context.UsersDto.FirstOrDefault(u => u.Password == password);
            if (userpassword is null || userpassword.Password != password)
            {
                return BadRequest("password xato");
            }
            return Ok(userpassword);
        }
        [HttpGet]
        public async Task<ActionResult<string>> GetByUser(string password)
        {
            var request = new UserDto();
            if (request.Password != password)
            {
                return Unauthorized();
            }

            return Ok(request);
        }
        [HttpGet("data")]
        public IActionResult Getdata(string key)
        {
            var data = _context.Users.FirstOrDefault(u => u.Key == key);

            if (data is null)
            {
                return BadRequest("Bunday user mavjud emas");
            }
            return Ok("user data");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

             
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
             
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSald)
        {
            using (var hmac = new HMACSHA512(passwordSald))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
            
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSald)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSald = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
