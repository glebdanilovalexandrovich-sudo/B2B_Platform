using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OptPlatform.Application;
using OptPlatform.Domain;
using OptPlatform.Infrastructure;
using Microsoft.Extensions.Logging;

namespace firstAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var usersDTO = await _context.Users
        .Select(u => new UserDTO
        {
            Id = u.Id,
            Email = u.Email,
            Role = u.Role
        })
        .ToListAsync();

            return Ok(usersDTO);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserId(int id)
        {
            if (id <= 0) { return BadRequest("Неверный Id!"); }

            var user = await _context.Users.FirstOrDefaultAsync(p => id == p.Id);
            if (user == null) { return NotFound("Пользователь не найден!"); }

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] UpdateRoleDTO dto)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Редактирование роли отклонено, неверный Id.");
                return BadRequest("Неверный Id!");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Редактирование роли отклонено, пользователь {id} не найден.", id);
                return NotFound("Пользователь не найден!");
            }

            // Проверка, что новая роль допустима
            if (dto.Role != "Admin" && dto.Role != "Supplier" && dto.Role != "Buyer")
            {
                _logger.LogWarning("Редактирование роли отклонено, неверная роль.");
                return BadRequest("Роль должна быть: Admin, Supplier или Buyer"); 
            }

            user.Role = dto.Role;
            await _context.SaveChangesAsync();

            var userDto = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            };

            _logger.LogInformation("Редактирование роли пользователя {id} успешно.", id);
            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id) 
        {
            if (id <= 0) 
            {
                _logger.LogWarning("Удаление пользователя {id} отклонено, неверный id.", id);
                return BadRequest("Неверный Id!"); 
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("Удаление пользователя {id} отклонено, пользователь не найден.", id);
                return NotFound("Пользователь не найден!");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Удаление пользователя {id} успешно.", id);
            return Ok("Пользователь удалён!");

        }

        [HttpGet("stat")]
        public async Task<IActionResult> UserStat() 
        {
            _logger.LogInformation("Запрос статистики пользователей");
            var userCount = await _context.Users.CountAsync();
            return Ok(new { totalUsers = userCount });
                
        }




    }
}
