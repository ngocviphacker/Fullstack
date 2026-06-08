using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaymentService.Data;
using PaymentService.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(PaymentDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || user.PasswordHash != request.Password) // Simple validation for testing
            {
                return BadRequest(new { message = "Tài khoản hoặc mật khẩu không chính xác!" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _config["Jwt:Key"] ?? "DNU_Finance_Super_Secret_JWT_Key_2026_FIT_DNU";
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim("StudentCode", user.StudentCode ?? ""),
                new Claim("TeacherCode", user.TeacherCode ?? ""),
                new Claim("TeacherId", user.TeacherCode ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"] ?? "DNUFinance",
                Audience = _config["Jwt:Audience"] ?? "DNUAudience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    username = user.Username,
                    role = user.Role,
                    fullName = user.FullName,
                    studentCode = user.StudentCode
                }
            });
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string StudentCode { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại trên hệ thống!" });
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password, // Plain text validation for testing
                FullName = request.FullName,
                Role = "Student",
                StudentCode = request.StudentCode
            };

            _context.Users.Add(user);

            // Also add to Student replica table if not exists
            if (!string.IsNullOrEmpty(request.StudentCode) && !await _context.Students.AnyAsync(s => s.StudentCode == request.StudentCode))
            {
                var student = new Student
                {
                    StudentCode = request.StudentCode,
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    ClassId = 1, // Default class
                    CourseId = 1 // Default course
                };
                _context.Students.Add(student);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đăng ký tài khoản học viên thành công!" });
        }
    }
}
