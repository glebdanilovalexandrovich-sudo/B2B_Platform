using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OptPlatform.Domain;
using OptPlatform.Infrastructure;
using OptPlatform.Application;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.Logging;

namespace firstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DealsController> _logger;

        public DealsController(AppDbContext context, ILogger<DealsController> logger)
        {
            _context = context;
            _logger = logger;
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

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
            try
            {

                foreach (var item in dto.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Создание сделки отклонено, товар не найден.");
                        return BadRequest($"Товар {item.ProductId} не найден"); 
                    }

                    if (product.Stock < item.Quantity)
                    {
                        _logger.LogWarning("Создание сделки отклонено, недостаточно товара.");
                        return BadRequest($"Недостаточно товара {product.Name}");
                    }

                    if (supplierId == null)
                        supplierId = product.SupplierId;
                    else if (supplierId != product.SupplierId)
                    {
                        _logger.LogWarning("Создание сделки отклонено, неверный поставщик.");
                        return BadRequest("Все товары должны быть от одного поставщика"); 
                    }

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
                await transaction.CommitAsync();

                _logger.LogInformation("Создание сделки успешно.");
                return Ok(deal);
            }

            catch
            {
                await transaction.RollbackAsync();
                _logger.LogError("Ошибка при создании сделки.");
                return StatusCode(500, "Ошибка при создании сделки!");
            }

        }

        [HttpGet("deals")]
        public async Task<IActionResult> GetUserDeal()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var query = _context.Deals
                .Include(d => d.Items)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (role == "Buyer")
                query = query.Where(d => d.BuyerId == userId);
            else if (role == "Supplier")
                query = query.Where(d => d.SupplierId == userId);


            var deals = await query.ToListAsync();

            if (!deals.Any())
                return NotFound("Сделки не найдены");

            return Ok(deals);

        }

        [Authorize(Roles = "Supplier")]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmDeal(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value); //get user id from jwt-token

            if (id <= 0) 
            {
                _logger.LogWarning("Подтверждение сделки отклонено, неверный Id.");
                return BadRequest("Неверный Id!"); 
            }

            var deal = await _context.Deals.FindAsync(id); //find deal in DB
            if (deal == null) 
            {
                _logger.LogWarning("Подтверждение сделки отклонено, сделка не найдена.");
                return NotFound("Сделка не найдена!");
            }

            if (deal.SupplierId != userId) { return Forbid(); } //check user its supplier?

            deal.Status = "Confirmed";
            await _context.SaveChangesAsync();

            _logger.LogInformation("Подтверждение сделки успешно.");
            return Ok(deal);
        }

        [Authorize(Roles = "Supplier")]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectDeal(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (id <= 0) 
            {
                _logger.LogWarning("Отклонение сделки отменено, неверный Id.");
                return BadRequest("Неверный id!"); 
            }


            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead); //isolation deal 
            try
            {
                var deal = await _context.Deals.FindAsync(id);
                if (deal == null) 
                {
                    _logger.LogWarning("Отклонение сделки отменено, сделка не найдена.");
                    return NotFound("Сделка не найдена!"); 
                }
                if (deal.SupplierId != userId) { return Forbid(); } //check our deal or not

                if (deal.Status != "Pending") 
                {
                    _logger.LogWarning("Отклонение сделки отменено, неверный статус.");
                    return BadRequest("Отмена сделки невозможна!"); 
                }

                var dealItems = await _context.DealItems
                     .Where(di => di.DealId == deal.Id)
                     .ToListAsync();

                foreach (var item in dealItems) // return product from fale-deal
                {

                    var product = await _context.Products.FindAsync(item.ProductId);
                    product.Stock += item.Quantity;
                }

                deal.Status = "Rejected";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Отклонение сделки успешно.");
                return Ok(new { deal.Id, deal.Status });
            }

            catch
            {
                _logger.LogError("Ошибка при отклонении сделки.");
                return StatusCode(500, "Ошибка при отклонении сделки!");
            }
        }

        [Authorize(Roles = "Buyer")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelDeal(int id) 
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value); 
           
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

            try 
            {
                var deal = await _context.Deals.FindAsync(id);
                if (deal == null) 
                {
                    _logger.LogWarning("Отмена сделки отклонена, сделка не найдена.");
                    return NotFound("Сделка не найдена!"); 
                }
                if (deal.BuyerId != userId) 
                {
                    _logger.LogWarning("Отмена сделки отклонена, неправильный пользователь.");
                    return Forbid(); 
                }
                if (deal.Status != "Pending") 
                {
                    _logger.LogWarning("Отмена сделки отклонена, неверный статус сделки.");
                    return BadRequest("Отменить сделку нельзя!");
                }

                var dealItems = await _context.DealItems
                    .Where(p => p.DealId == deal.Id)
                    .ToListAsync();

                foreach (var item in dealItems) 
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    product.Stock += item.Quantity;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Отмена сделки успешна.");
                return Ok(new {deal.Id, deal.Status });
            }

            catch 
            { 
                await transaction.RollbackAsync();
                _logger.LogError("Ошибка при отмене сделки!");
                return StatusCode(500, "Ошибка при отмене сделки!");
            }
        }
    }
    }

