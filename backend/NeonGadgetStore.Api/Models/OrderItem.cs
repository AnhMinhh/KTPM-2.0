using System.ComponentModel.DataAnnotations;

namespace NeonGadgetStore.Api.Models;

public class OrderItem
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string OrderId { get; set; } = string.Empty;

    public string? ProductId { get; set; }

    [Required]
    public string ProductName { get; set; } = string.Empty;

    public string? ProductImage { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Order? Order { get; set; }
}
