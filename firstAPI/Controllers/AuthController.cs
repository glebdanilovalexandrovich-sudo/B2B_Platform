using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OptPlatform.Domain;
using OptPlatform.Application;
using OptPlatform.Infrastructure;


namespace firstAPI.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        [HttpPost("registerAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminDTO register) 
        {
            if (string.IsNullOrWhiteSpace(register.Email))
                return BadRequest("Введите Email!");

            if (string.IsNullOrWhiteSpace(register.Password) || register.Password.Length < 6)
                return BadRequest("Пароль должен быть 6 или более символов!");

            var checkAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Email == register.Email);
            if (checkAdmin != null) { return BadRequest("Пользовтель с таким логином уже существует!"); }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

            var admin = new User
            {
                Email = register.Email,
                PasswordHash = passwordHash,
                Role = "Admin"
               
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            return Ok("Регистрация прошла успешно!");
        }

        

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO) 
        {

            //check parameters
            if (string.IsNullOrWhiteSpace(registerDTO.Email))
                return BadRequest("Введите Email!");

            if (string.IsNullOrWhiteSpace(registerDTO.Password) || registerDTO.Password.Length < 6)
                return BadRequest("Пароль должен быть 6 или более символов!");

            if (registerDTO.Role != "Supplier" && registerDTO.Role != "Buyer")
                return BadRequest("Роль должна быть Supplier, Buyer");

            var checkUser = await _context.Users.FirstOrDefaultAsync(p => p.Email == registerDTO.Email);
            if (checkUser != null) { return BadRequest("Пользователь с таким логином уже есть!"); }

            //Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            //create new user
            var user = new User
            {
                Email = registerDTO.Email,
                PasswordHash = passwordHash,
                Role = registerDTO.Role,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("Регистрация прошла успешно!");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {

            //check
            if (string.IsNullOrWhiteSpace(login.Email)) 
            {
                return BadRequest("Неверный Email!");
            }
            if (string.IsNullOrWhiteSpace(login.Password)) { return BadRequest("Введите пароль!"); }

            //take user from DB
            var user = await _context.Users.FirstOrDefaultAsync(p=> p.Email == login.Email);

            if (user == null) { return Unauthorized("Пользователь не найден!"); }

            //take user password from user
            var passwordCheck = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
            if (passwordCheck != true) 
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token });

        }

        //hash password
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //return id
        new Claim(ClaimTypes.Email, user.Email), //return email into claim
        new Claim(ClaimTypes.Role, user.Role) //return role into claim
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], //who was return
                audience: _configuration["Jwt:Audience"], //who was take
                claims: claims,
                expires: DateTime.Now.AddHours(1), //time of life
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
