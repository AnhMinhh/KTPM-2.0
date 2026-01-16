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
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminOrdersController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _db.Orders
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
                user_id = order.UserId,
                status = order.Status,
                total_amount = order.TotalAmount,
                payment_method = order.PaymentMethod,
                shipping_address = JsonSerializer.Deserialize<OrdersController.ShippingInfo>(order.ShippingAddressJson ?? "{}"),
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

    public sealed class UpdateStatusRequest
    {
        [JsonPropertyName("status")]
        public required string Status { get; set; }
    }

    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] string orderId, [FromBody] UpdateStatusRequest request)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order == null) return NotFound();

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }
}
