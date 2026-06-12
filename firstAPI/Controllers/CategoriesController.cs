using ClassLibrary1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firstAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
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
                return BadRequest("Неверный Id");

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
                return BadRequest("Неверное имя!");

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

            return CreatedAtAction(nameof(GetIdCategory), new { id = category.Id }, categoryDTO);
        }

        //Update category
        [Authorize(Roles ="Admin")]
        [HttpPut ("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDTO categoryUpd)
        {
            if (id <= 0) { return NotFound("Ошибка! Неверный Id"); }

            var category = await _context.Categories.FindAsync(id);
            if (category == null) { return NotFound("категория не найдена!"); }

            category.Name = categoryUpd.Name;
            await _context.SaveChangesAsync();

            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
                
            };

            return Ok(categoryDTO);



        }

        //delete the category
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            if (id <= 0) { return NotFound("Не найдено по Id"); }
            var category = await _context.Categories.FindAsync(id);
            if (category == null) { return NotFound("Категория не найдена!"); }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Категория удалена!");


        }
 
    }
}