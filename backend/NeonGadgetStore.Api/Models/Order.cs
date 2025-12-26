using System.ComponentModel.DataAnnotations;

namespace NeonGadgetStore.Api.Models;

public class Order
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string? UserId { get; set; }

    [Required]
    public string OrderNumber { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = "cod";

    [Required]
    public string PaymentStatus { get; set; } = "pending";

    [Required]
    public string Status { get; set; } = "pending";

    public string ShippingAddressJson { get; set; } = "{}";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public string? TrackingNumber { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
