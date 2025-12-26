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
public class WishlistController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var entries = await _db.Wishlist
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
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
                    created_at = e.CreatedAt,
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

    public sealed class AddWishlistRequest
    {
        [JsonPropertyName("product_id")]
        public required string ProductId { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var exists = await _db.Wishlist
            .AnyAsync(w => w.UserId == userId && w.ProductId == request.ProductId);

        if (exists)
        {
            return BadRequest(new { message = "Product already in wishlist" });
        }

        var entry = new WishlistEntry
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Wishlist.Add(entry);
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist([FromRoute] string productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var entry = await _db.Wishlist
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (entry == null) return NotFound();

        _db.Wishlist.Remove(entry);
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }
}
