namespace NeonGadgetStore.Api.Models;

/// <summary>
/// ShoppingCart Class - theo báo cáo DFD
/// Quản lý giỏ hàng của người dùng
/// </summary>
public class ShoppingCart
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// shopping_cart_id - ID giỏ hàng
    /// </summary>
    public int? ShoppingCartId { get; set; }
    
    /// <summary>
    /// shopping_name - Tên giỏ hàng
    /// </summary>
    public string? ShoppingName { get; set; }
    
    /// <summary>
    /// shopping_description - Mô tả giỏ hàng
    /// </summary>
    public string? ShoppingDescription { get; set; }
    
    /// <summary>
    /// shopping_cart_type - Loại giỏ hàng (temp, active, abandoned)
    /// </summary>
    public string? ShoppingCartType { get; set; }
    
    /// <summary>
    /// ID người dùng sở hữu giỏ hàng
    /// </summary>
    public string? UserId { get; set; }
    
    public ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Danh sách sản phẩm trong giỏ hàng
    /// </summary>
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Các phương thức cơ bản (từ báo cáo)
    public void AddShoppingCart() { }
    public void EditShoppingCart() { }
    public void DeleteShoppingCart() { }
    public List<ShoppingCart> SearchShoppingCart() => new List<ShoppingCart>();
}
