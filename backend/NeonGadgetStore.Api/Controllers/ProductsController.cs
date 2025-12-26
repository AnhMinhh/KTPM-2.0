using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? categoryId = null)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] string id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();

        return Ok(product);
    }
}
