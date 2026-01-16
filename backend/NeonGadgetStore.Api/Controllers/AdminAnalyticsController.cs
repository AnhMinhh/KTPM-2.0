using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminAnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminAnalyticsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAnalytics()
    {
        var now = DateTime.UtcNow;
        var last30Days = now.AddDays(-30);
        var last7Days = now.AddDays(-7);

        // Orders stats
        var totalOrders = await _db.Orders.CountAsync();
        var orders30d = await _db.Orders
            .Where(o => o.CreatedAt >= last30Days)
            .CountAsync();
        var orders7d = await _db.Orders
            .Where(o => o.CreatedAt >= last7Days)
            .CountAsync();

        // Revenue stats
        var totalRevenue = await _db.Orders
            .Where(o => o.Status == "completed")
            .SumAsync(o => o.TotalAmount);
        var revenue30d = await _db.Orders
            .Where(o => o.Status == "completed" && o.CreatedAt >= last30Days)
            .SumAsync(o => o.TotalAmount);
        var revenue7d = await _db.Orders
            .Where(o => o.Status == "completed" && o.CreatedAt >= last7Days)
            .SumAsync(o => o.TotalAmount);

        // Users stats
        var totalUsers = await _userManager.Users.CountAsync();
        var users30d = await _userManager.Users
            .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.UtcNow) // fallback: no simple CreatedAt in IdentityUser
            .CountAsync();

        // Products stats
        var totalProducts = await _db.Products.CountAsync();
        var featuredProducts = await _db.Products.CountAsync(p => p.IsFeatured);
        var outOfStockProducts = await _db.Products.CountAsync(p => p.StockQuantity <= 0);

        // Conversion rate (simple approximation: orders / users)
        var conversionRate = totalUsers > 0 ? (double)totalOrders / totalUsers * 100 : 0;

        // Top categories by orders
        var topCategories = await _db.OrderItems
            .Join(_db.Products, oi => oi.ProductId, p => p.Id, (oi, p) => new { p.CategoryId, oi.Quantity })
            .GroupBy(x => x.CategoryId)
            .Select(g => new
            {
                category_id = g.Key,
                total_quantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(g => g.total_quantity)
            .Take(5)
            .ToListAsync();

        var categoryIds = topCategories.Select(c => c.category_id).ToList();
        var categoryNames = await _db.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var topCategoriesWithNames = topCategories.Select(c => new
        {
            category_id = c.category_id,
            category_name = categoryNames.GetValueOrDefault(c.category_id, "Unknown"),
            total_quantity = c.total_quantity
        });

        // Abandoned carts approximation (orders with status pending older than 1 day)
        var abandonedCarts = await _db.Orders
            .Where(o => o.Status == "pending" && o.CreatedAt < now.AddDays(-1))
            .CountAsync();

        return Ok(new
        {
            orders = new
            {
                total = totalOrders,
                last_30d = orders30d,
                last_7d = orders7d
            },
            revenue = new
            {
                total = totalRevenue,
                last_30d = revenue30d,
                last_7d = revenue7d
            },
            users = new
            {
                total = totalUsers,
                last_30d = users30d
            },
            products = new
            {
                total = totalProducts,
                featured = featuredProducts,
                out_of_stock = outOfStockProducts
            },
            conversion_rate = Math.Round(conversionRate, 2),
            top_categories = topCategoriesWithNames,
            abandoned_carts = abandonedCarts
        });
    }
}
