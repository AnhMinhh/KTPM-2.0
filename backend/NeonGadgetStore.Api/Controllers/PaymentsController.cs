using Microsoft.AspNetCore.Mvc;
using NeonGadgetStore.Api.Models;
using NeonGadgetStore.Api.Services;

namespace NeonGadgetStore.Api.Controllers;

/// <summary>
/// PaymentController - Quản lý thanh toán đơn hàng
/// Theo DFD: Process 7.0 - Make Payment
/// Các endpoint xử lý việc thanh toán, cập nhật trạng thái, và lịch sử thanh toán
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// GET: api/payments/{id}
    /// Lấy thông tin thanh toán theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetPayment(string id)
    {
        try
        {
            var payment = await _paymentService.GetPaymentAsync(id);
            if (payment == null)
                return NotFound("Thanh toán không tìm thấy");

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy thanh toán: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// GET: api/payments/order/{orderId}
    /// Lấy tất cả thanh toán của một đơn hàng
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<List<Payment>>> GetOrderPayments(string orderId)
    {
        try
        {
            var payments = await _paymentService.GetOrderPaymentsAsync(orderId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy thanh toán đơn hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// POST: api/payments
    /// Tạo ghi nhận thanh toán mới
    /// Gọi từ process: 7.0 Make Payment
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Payment>> CreatePayment([FromBody] Payment payment)
    {
        try
        {
            if (string.IsNullOrEmpty(payment.ShoppingName))
                return BadRequest("Tên thanh toán không thể trống");

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            return CreatedAtAction(nameof(GetPayment), new { id = createdPayment.Id }, createdPayment);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi tạo thanh toán: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// PATCH: api/payments/{id}/status
    /// Cập nhật trạng thái thanh toán
    /// Status: Pending, Success, Failed, Cancelled
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] Dictionary<string, string> request)
    {
        try
        {
            if (!request.ContainsKey("status"))
                return BadRequest("Status không thể trống");

            var status = request["status"];
            var validStatuses = new[] { "Pending", "Success", "Failed", "Cancelled" };
            
            if (!validStatuses.Contains(status))
                return BadRequest($"Trạng thái không hợp lệ. Chỉ nhận: {string.Join(", ", validStatuses)}");

            await _paymentService.UpdatePaymentStatusAsync(id, status);
            return Ok("Cập nhật trạng thái thanh toán thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi cập nhật trạng thái thanh toán: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// DELETE: api/payments/{id}
    /// Xóa ghi nhận thanh toán (chỉ admin)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePayment(string id)
    {
        try
        {
            await _paymentService.DeletePaymentAsync(id);
            return Ok("Xóa thanh toán thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi xóa thanh toán: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// GET: api/payments/search
    /// Tìm kiếm thanh toán theo tiêu chí
    /// Query params: status, fromDate, toDate
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<Payment>>> SearchPayments(
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        try
        {
            var payments = await _paymentService.SearchPaymentsAsync(status, fromDate, toDate);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi tìm kiếm thanh toán: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }
}
