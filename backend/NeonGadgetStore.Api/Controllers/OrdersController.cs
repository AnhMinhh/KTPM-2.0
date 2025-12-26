using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public sealed class CreateOrderRequest
    {
        [JsonPropertyName("payment_method")]
        public required string PaymentMethod { get; set; }

        [JsonPropertyName("shipping_address")]
        public required ShippingInfo ShippingAddress { get; set; }

        [JsonPropertyName("items")]
        public List<CartItem> Items { get; set; } = new();
    }

    public sealed class ShippingInfo
    {
        [JsonPropertyName("full_name")]
        public required string FullName { get; set; }

        [JsonPropertyName("phone")]
        public required string Phone { get; set; }

        [JsonPropertyName("address_line1")]
        public required string AddressLine1 { get; set; }

        [JsonPropertyName("address_line2")]
        public string? AddressLine2 { get; set; }

        [JsonPropertyName("city")]
        public required string City { get; set; }

        [JsonPropertyName("state")]
        public required string State { get; set; }

        [JsonPropertyName("postal_code")]
        public required string PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; } = "US";
    }

    public sealed class CartItem
    {
        [JsonPropertyName("product_id")]
        public required string ProductId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Unauthorized();

        // Validate product existence and current price
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        foreach (var item in request.Items)
        {
            if (!products.ContainsKey(item.ProductId))
            {
                return BadRequest(new { message = $"Product {item.ProductId} not found" });
            }
        }

        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Status = "pending",
                PaymentMethod = request.PaymentMethod,
                ShippingAddressJson = JsonSerializer.Serialize(request.ShippingAddress),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];
                var unitPrice = product.Price;
                var totalPrice = unitPrice * item.Quantity;

                totalAmount += totalPrice;

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
            }

            order.TotalAmount = totalAmount;

            _db.Orders.Add(order);
            _db.OrderItems.AddRange(orderItems);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new { id = order.Id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var orders = await _db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var result = new List<object>();

        foreach (var order in orders)
        {
            var items = await _db.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();

            result.Add(new
            {
                id = order.Id,
                status = order.Status,
                total_amount = order.TotalAmount,
                payment_method = order.PaymentMethod,
                shipping_address = JsonSerializer.Deserialize<ShippingInfo>(order.ShippingAddressJson ?? "{}"),
                created_at = order.CreatedAt,
                updated_at = order.UpdatedAt,
                items = items.Select(i => new
                {
                    id = i.Id,
                    product_id = i.ProductId,
                    quantity = i.Quantity,
                    unit_price = i.UnitPrice
                })
            });
        }

        return Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder([FromRoute] string orderId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null) return NotFound();

        var items = await _db.OrderItems
            .Where(oi => oi.OrderId == order.Id)
            .ToListAsync();

        return Ok(new
        {
            order = new
            {
                id = order.Id,
                status = order.Status,
                total_amount = order.TotalAmount,
                payment_method = order.PaymentMethod,
                shipping_address = JsonSerializer.Deserialize<ShippingInfo>(order.ShippingAddressJson ?? "{}"),
                created_at = order.CreatedAt,
                updated_at = order.UpdatedAt
            },
            items = items.Select(i => new
            {
                id = i.Id,
                product_id = i.ProductId,
                quantity = i.Quantity,
                unit_price = i.UnitPrice
            })
        });
    }
}
