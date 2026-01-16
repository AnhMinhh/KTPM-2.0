namespace NeonGadgetStore.Api.Models;

/// <summary>
/// CartItem Class
/// Chi tiết sản phẩm trong giỏ hàng
/// </summary>
public class CartItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// ID giỏ hàng chứa item này
    /// </summary>
    public string? CartId { get; set; }
    
    public ShoppingCart? Cart { get; set; }
    
    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public string? ProductId { get; set; }
    
    public Product? Product { get; set; }
    
    /// <summary>
    /// Số lượng sản phẩm trong giỏ
    /// </summary>
    public int Quantity { get; set; }
    
    public DateTime? AddedAt { get; set; } = DateTime.UtcNow;
}
