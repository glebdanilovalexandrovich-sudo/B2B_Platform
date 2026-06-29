using OptPlatform.Domain;
using OptPlatform.Application;
using OptPlatform.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace firstAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(AppDbContext context, ILogger<CategoriesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCategory()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIdCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Неверный Id");
            }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Категория не найдена!");

            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
                
            };

            return Ok(categoryDTO);
        }

        //create the category
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)  
        {
            if (string.IsNullOrWhiteSpace(categoryDto.Name))
            {
                _logger.LogWarning("Создание категории отменено, неверное имя.");
                return BadRequest("Неверное имя!"); 
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };

            _logger.LogInformation("Создание категории {Name} успешно.", categoryDTO.Name);
            return CreatedAtAction(nameof(GetIdCategory), new { id = category.Id }, categoryDTO);
        }

        //Update category
        [Authorize(Roles ="Admin")]
        [HttpPut ("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO categoryUpd)
        {
            if (id <= 0) 
            {
                _logger.LogWarning("Изменение категории отменено, неверный Id.");
                return NotFound("Ошибка! Неверный Id");
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null) 
            {
                _logger.LogWarning("Изменение категории отменено, категория не найдена.");
                return NotFound("категория не найдена!"); 
            }

            category.Name = categoryUpd.Name;
            await _context.SaveChangesAsync();

            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
                
            };

            _logger.LogInformation("Изменение категории {Name} успешно.", categoryDTO.Name);
            return Ok(categoryDTO);



        }

        //delete the category
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            if (id <= 0) 
            {
                _logger.LogWarning("Удаление категории отклонено, неверный Id.");
                return NotFound("Не найдено по Id"); 
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null) 
            {
                _logger.LogWarning("Удаление категории отклонено, категория не найдена.");
                return NotFound("Категория не найдена!"); 
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Удаление категории успешно.");
            return Ok("Категория удалена!");


        }
 
    }
}