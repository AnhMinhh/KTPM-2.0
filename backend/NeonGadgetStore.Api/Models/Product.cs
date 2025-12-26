using System.ComponentModel.DataAnnotations;

namespace NeonGadgetStore.Api.Models;

public class Product
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Slug { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int StockQuantity { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;

    public List<string> Images { get; set; } = new();

    public string? CategoryId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
