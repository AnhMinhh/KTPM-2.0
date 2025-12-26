using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "admin")]
public class AdminOverviewController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminOverviewController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetOverview()
    {
        var totalUsers = await _userManager.Users.CountAsync();
        var totalOrders = await _db.Orders.CountAsync();
        var totalProducts = await _db.Products.CountAsync();
        var totalRevenue = await _db.Orders
            .Where(o => o.Status == "completed")
            .SumAsync(o => o.TotalAmount);

        var last30Days = DateTime.UtcNow.AddDays(-30);
        var recentOrders = await _db.Orders
            .Where(o => o.CreatedAt >= last30Days)
            .CountAsync();

        var topProducts = await _db.OrderItems
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                product_id = g.Key,
                total_quantity = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(g => g.total_quantity)
            .Take(5)
            .ToListAsync();

        var productIds = topProducts.Select(p => p.product_id).ToList();
        var productNames = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name);

        var topProductsWithNames = topProducts.Select(p => new
        {
            product_id = p.product_id,
            product_name = productNames.GetValueOrDefault(p.product_id, "Unknown"),
            total_quantity = p.total_quantity
        });

        return Ok(new
        {
            total_users = totalUsers,
            total_orders = totalOrders,
            total_products = totalProducts,
            total_revenue = totalRevenue,
            recent_orders_30d = recentOrders,
            top_products = topProductsWithNames
        });
    }
}
