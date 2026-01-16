namespace NeonGadgetStore.Api.Models;

/// <summary>
/// Payment Class - theo báo cáo DFD
/// Quản lý thông tin thanh toán của đơn hàng
/// </summary>
public class Payment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// payment_id - ID duy nhất của thanh toán
    /// </summary>
    public int? PaymentId { get; set; }
    
    /// <summary>
    /// Shopping_id - ID giỏ hàng/đơn hàng
    /// </summary>
    public int? ShoppingId { get; set; }
    
    /// <summary>
    /// Shopping_name - Tên giao dịch
    /// </summary>
    public string? ShoppingName { get; set; }
    
    /// <summary>
    /// Shopping_description - Mô tả giao dịch
    /// </summary>
    public string? ShoppingDescription { get; set; }
    
    /// <summary>
    /// payment_date - Ngày thực hiện thanh toán
    /// </summary>
    public DateTime? PaymentDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// payment_method - Phương thức thanh toán (Credit Card, PayPal, COD, Bank Transfer)
    /// </summary>
    public string? PaymentMethod { get; set; }
    
    /// <summary>
    /// payment_status - Trạng thái thanh toán (Pending, Success, Failed, Cancelled)
    /// </summary>
    public string? PaymentStatus { get; set; }
    
    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal? Amount { get; set; }
    
    /// <summary>
    /// Khóa ngoại - Liên kết đến Order
    /// </summary>
    public string? OrderId { get; set; }
    
    public Order? Order { get; set; }
    
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Các phương thức cơ bản (từ báo cáo)
    public void AddPayment() { }
    public void EditPayment() { }
    public void DeletePayment() { }
    public List<Payment> SearchPayments() => new List<Payment>();
}
