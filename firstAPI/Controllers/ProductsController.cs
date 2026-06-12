using ClassLibrary1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    //to get all products
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name
            })
            .ToListAsync();

        return Ok(products);
    }

    //удаление
    [Authorize(Roles = "Supplier")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0) { return NotFound("Неверный Id!"); }
        var tovar = await _context.Products.FindAsync(id);

        if (tovar != null)
        {
            _context.Products.Remove(tovar);
            await _context.SaveChangesAsync();
            return Ok($"{tovar.Name} успешно удалён!");
        }
        else
        {
            return BadRequest("Ошибка! Товар не найден!");
        }
    }

    [Authorize(Roles = "Supplier")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product product)
    {
        if (product.Price <= 0)
        {
            return BadRequest("Неверная цена!");
        }

        if (product.Name == null) { return BadRequest("Неверное имя!"); }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();
        var category_name = product.Category.Name;

        var ProductDTO = new ProductDto
        {
            Id = product.Id,
            Price = product.Price,
            Name = product.Name,
            CategoryName = product.Category.Name
        };
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, ProductDTO);



    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
        .Include(p => p.Category)
        .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) { return NotFound("Ошибка! Товар не найден!"); }

        var ProductDTO = new ProductDto
        {
            Id = product.Id,
            Price = product.Price,
            Name = product.Name,
            CategoryName = product.Category.Name
        };

        return Ok(ProductDTO);
    }

    [Authorize(Roles = "Supplier")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) { return NotFound("Товар не найден!"); }

        product.Price = updatedProduct.Price;
        product.Name = updatedProduct.Name;

        await _context.SaveChangesAsync();
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();

        var productDTO = new ProductDto
        {
            Id = product.Id,
            Price = product.Price,
            Name = product.Name,
            CategoryName = product.Category.Name
        };
        return Ok(productDTO);

    }


    




}