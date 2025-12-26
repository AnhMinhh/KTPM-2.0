using System.ComponentModel.DataAnnotations;

namespace NeonGadgetStore.Api.Models;

public class WishlistEntry
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string ProductId { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
