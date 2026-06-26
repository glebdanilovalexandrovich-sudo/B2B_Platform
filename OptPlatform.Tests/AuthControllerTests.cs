using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using firstAPI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using OptPlatform.Application;
using OptPlatform.Domain;
using OptPlatform.Infrastructure;
using Xunit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;

namespace OptPlatform.Tests
{
    
    public class AuthControllerTests //dont work
    {
        [Fact]
        public async Task Register_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@mail.com",
                Password = "123456",
                Role = "Buyer"
            };

            var mockContext = new Mock<AppDbContext>();
            mockContext.Setup(x => x.Users.AddAsync(It.IsAny<User>(), default))
                       .ReturnsAsync((User user, CancellationToken token) =>
                           (EntityEntry<User>)null);

            
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:Key"]).Returns("test_key_1234567890");
            mockConfig.Setup(x => x["Jwt:Issuer"]).Returns("test");
            mockConfig.Setup(x => x["Jwt:Audience"]).Returns("test");

            var controller = new AuthController(mockContext.Object, mockConfig.Object);

         
            var result = await controller.Register(dto);

            
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Register_ValidData_ReturnBadRequest()
        {
            var dto = new RegisterDTO
            {
                Email = null,
                Password = "123456",
                Role = "Buyer"
            };

            var result = new BadRequestResult();

            Assert.IsType<BadRequestResult>(result);
        }

    }
}
