using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _db.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] string id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return Ok(category);
    }
}
