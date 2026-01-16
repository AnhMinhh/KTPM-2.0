namespace NeonGadgetStore.Api.Models;

/// <summary>
/// Review Class
/// Đánh giá và bình luận sản phẩm từ người dùng
/// </summary>
public class Review
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// ID sản phẩm được đánh giá
    /// </summary>
    public string? ProductId { get; set; }
    
    public Product? Product { get; set; }
    
    /// <summary>
    /// ID người dùng đánh giá
    /// </summary>
    public string? UserId { get; set; }
    
    public ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Điểm đánh giá (1-5 sao)
    /// </summary>
    public int? Rating { get; set; }
    
    /// <summary>
    /// Tiêu đề đánh giá
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Nội dung đánh giá chi tiết
    /// </summary>
    public string? Content { get; set; }
    
    /// <summary>
    /// Có phải mua thực tế không
    /// </summary>
    public bool IsVerified { get; set; }
    
    /// <summary>
    /// Số người cảm thấy đánh giá hữu ích
    /// </summary>
    public int HelpfulCount { get; set; }
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
}
