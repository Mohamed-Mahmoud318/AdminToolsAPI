using Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {


            if (request.Username == "admin" && request.Password == "yourpassword")
            {
                var user = new User
                {
                    userName ="admin",
                    password= "yourpassword",
                    IsAdmin = true
                };
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }


        private string GenerateJwtToken(User user)
        {
            // إنشاء مفتاح الأمان
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // إنشاء قائمة الـ Claims بناءً على بيانات المستخدم
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.userName),
            new Claim("IsAdmin", user.IsAdmin.ToString()) // تخزين حالة الأدمن كـ Claim
        };

            // إضافة Role إذا كان المستخدم أدمن
            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            // إنشاء التوكن
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
