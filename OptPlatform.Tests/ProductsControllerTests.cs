using firstAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OptPlatform.Api;
using OptPlatform.Application;
using OptPlatform.Domain;
using OptPlatform.Infrastructure;
using Xunit;

namespace OptPlatform.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task Create_ValidData_ReturnsCreated()
        {
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

           
            var category = new Category { Id = 1, Name = "Электроника" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

           
            var dto = new Product
            {
                Name = "Тестовый товар",
                Price = 1000,
                Stock = 10,
                CategoryId = 1 
            };

            var mockLogger = new Mock<ILogger<ProductsController>>();
            var controller = new ProductsController(context, mockLogger.Object);

            
            var result = await controller.Create(dto);

            
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<ProductDTO>(actionResult.Value);

            Assert.Equal("Тестовый товар", returnValue.Name);
            Assert.Equal(1000, returnValue.Price);
            Assert.Equal("Электроника", returnValue.CategoryName); 

            
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Тестовый товар");
            Assert.NotNull(savedProduct);
            Assert.Equal(1000, savedProduct.Price);
        }




    }
}
