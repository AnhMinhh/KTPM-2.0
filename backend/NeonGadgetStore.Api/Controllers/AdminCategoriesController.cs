using System.Text.Json.Serialization;
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
public class AdminCategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminCategoriesController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    public sealed class CreateCategoryRequest
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = new Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Ok(category);
    }

    public sealed class UpdateCategoryRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] string id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        if (request.Name != null) category.Name = request.Name;
        if (request.Description != null) category.Description = request.Description;

        await _db.SaveChangesAsync();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] string id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }
}
