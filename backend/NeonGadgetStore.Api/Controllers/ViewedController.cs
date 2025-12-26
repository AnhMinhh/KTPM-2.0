using System.Security.Claims;
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
public class ViewedController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ViewedController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyViewedHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var entries = await _db.ViewedHistory
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.ViewedAt)
            .Take(50)
            .ToListAsync();

        var productIds = entries.Select(e => e.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        var result = entries
            .Where(e => products.ContainsKey(e.ProductId))
            .Select(e =>
            {
                var p = products[e.ProductId];
                return new
                {
                    id = e.Id,
                    product_id = e.ProductId,
                    viewed_at = e.ViewedAt,
                    product = new
                    {
                        id = p.Id,
                        name = p.Name,
                        price = p.Price,
                        images = p.Images,
                        category_id = p.CategoryId
                    }
                };
            });

        return Ok(result);
    }

    public sealed class TrackViewRequest
    {
        [JsonPropertyName("product_id")]
        public required string ProductId { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> TrackView([FromBody] TrackViewRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var existing = await _db.ViewedHistory
            .FirstOrDefaultAsync(v => v.UserId == userId && v.ProductId == request.ProductId);

        if (existing != null)
        {
            existing.ViewedAt = DateTime.UtcNow;
        }
        else
        {
            var entry = new ViewedEntry
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                ProductId = request.ProductId,
                ViewedAt = DateTime.UtcNow
            };
            _db.ViewedHistory.Add(entry);
        }

        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearViewedHistory()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var entries = await _db.ViewedHistory
            .Where(v => v.UserId == userId)
            .ToListAsync();

        _db.ViewedHistory.RemoveRange(entries);
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }
}
