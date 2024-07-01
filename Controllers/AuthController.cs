using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagmentSystem_Dot.Net8.Models;

namespace UserManagmentSystem_Dot.Net8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static List<UserAuth> users = new List<UserAuth>();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<UserAuth> Register(UserAuthDto request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            UserAuth userAuth = new UserAuth
            {
                UserName = request.Username,
                PasswordHash = passwordHash
            };

            users.Add(userAuth);
            return Ok(true);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserAuthDto request)
        {
            var userAuth = users.FirstOrDefault(u => u.UserName == request.Username);
            if (userAuth == null)
            {
                return BadRequest("User Not Found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, userAuth.PasswordHash))
            {
                return BadRequest("Wrong Password");
            }

            string jwtToken = CreateJwtToken(userAuth);
            return Ok($"{{ \"jwt\": \"{jwtToken}\" }}");
        }

        private string CreateJwtToken(UserAuth user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


