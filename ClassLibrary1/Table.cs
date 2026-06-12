using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int SupplierId { get; set; }
        public User User { get; set; }
        public int Stock {  get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }

  
    
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Deal> Deals { get; set; }
            public DbSet<DealItem> DealItems { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class User 
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    public class RegisterDTO 
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class AdminDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDTO 
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //trade
    public class Deal
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User Buyer { get; set; }

        [DeleteBehavior(DeleteBehavior.Restrict)]
        public User Supplier { get; set; }

        public List<DealItem> Items { get; set; }
    }

    public class DealItem
    {
        public int Id { get; set; }
        public int DealId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }

        [DeleteBehavior(DeleteBehavior.Cascade)]
        public Deal Deal { get; set; }

        public Product Product { get; set; }
    }


    public class DealDTO
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DealItemDTO> Items { get; set; }
    }

    public class DealItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; }
        public decimal Total => PriceAtMoment * Quantity; 
    }

    public class CreateDealItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateDealDto
    {
        public List<CreateDealItemDto> Items { get; set; }
    }


}


