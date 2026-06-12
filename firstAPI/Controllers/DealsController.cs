using System.Security.Claims;
using ClassLibrary1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace firstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DealsController(AppDbContext context)
        {
            _context = context;
        }

        //create deal
      
        [Authorize(Roles = "Buyer")]
        [HttpPost]
        public async Task<IActionResult> CreateDeal([FromBody] CreateDealDto dto)
        {
            var buyerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int? supplierId = null;
            decimal totalAmount = 0;
            var dealItems = new List<DealItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Товар {item.ProductId} не найден");

                if (product.Stock < item.Quantity)
                    return BadRequest($"Недостаточно товара {product.Name}");

                if (supplierId == null)
                    supplierId = product.SupplierId;
                else if (supplierId != product.SupplierId)
                    return BadRequest("Все товары должны быть от одного поставщика");

                totalAmount += product.Price * item.Quantity;

                dealItems.Add(new DealItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtMoment = product.Price
                });

                product.Stock -= item.Quantity;
            }

            var deal = new Deal
            {
                BuyerId = buyerId,
                SupplierId = supplierId.Value,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Items = dealItems
            };

            _context.Deals.Add(deal);
            await _context.SaveChangesAsync();

            return Ok(deal);
        }

    }






    }

