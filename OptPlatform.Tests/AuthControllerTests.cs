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
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ValidData_ReturnsOk()
        {
           
            var dto = new RegisterDTO
            {
                Email = "test@mail.com",
                Password = "123456",
                Role = "Buyer"
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns("test_key_1234567890");
            mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("test");
            mockConfig.Setup(x => x["Jwt:Audience"]).Returns("test");

            
            var mockLogger = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(context, mockConfig.Object, mockLogger.Object);

            var result = await controller.Register(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_InvalidEmail_ReturnsBadRequest()
        {
            
            var dto = new RegisterDTO
            {
                Email = null,
                Password = "123456",
                Role = "Buyer"
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns("test_key_1234567890");
            mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("test");
            mockConfig.Setup(x => x["Jwt:Audience"]).Returns("test");

            
            var mockLogger = new Mock<ILogger<AuthController>>();

            var controller = new AuthController(context, mockConfig.Object, mockLogger.Object);

            
            var result = await controller.Register(dto);

            
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}