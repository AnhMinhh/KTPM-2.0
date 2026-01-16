using Microsoft.AspNetCore.Mvc;
using NeonGadgetStore.Api.Models;
using NeonGadgetStore.Api.Services;

namespace NeonGadgetStore.Api.Controllers;

/// <summary>
/// CartsController - Quản lý giỏ hàng
/// Theo DFD: Process 5.0 - Buy Item / Manage Shopping Cart
/// Các endpoint xử lý thêm, xóa, cập nhật sản phẩm trong giỏ hàng
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartsController> _logger;

    public CartsController(ICartService cartService, ILogger<CartsController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    /// <summary>
    /// GET: api/carts/{userId}
    /// Lấy giỏ hàng của người dùng
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<ActionResult<ShoppingCart>> GetUserCart(string userId)
    {
        try
        {
            var cart = await _cartService.GetUserCartAsync(userId);
            if (cart == null)
            {
                // Tạo giỏ hàng mới nếu chưa có
                cart = await _cartService.CreateCartAsync(userId);
            }
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// GET: api/carts/{userId}/items
    /// Lấy danh sách sản phẩm trong giỏ hàng
    /// </summary>
    [HttpGet("{userId}/items")]
    public async Task<ActionResult<List<CartItem>>> GetCartItems(string userId)
    {
        try
        {
            var items = await _cartService.GetCartItemsAsync(userId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi lấy sản phẩm trong giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// GET: api/carts/{userId}/total
    /// Tính tổng giá trị giỏ hàng
    /// </summary>
    [HttpGet("{userId}/total")]
    public async Task<ActionResult<decimal>> GetCartTotal(string userId)
    {
        try
        {
            var total = await _cartService.GetCartTotalAsync(userId);
            return Ok(new { total });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi tính tổng giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// POST: api/carts/{userId}/add
    /// Thêm sản phẩm vào giỏ hàng
    /// Request body: { "productId": "...", "quantity": 1 }
    /// </summary>
    [HttpPost("{userId}/add")]
    public async Task<IActionResult> AddToCart(string userId, [FromBody] AddToCartRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ProductId) || request.Quantity <= 0)
                return BadRequest("ProductId và Quantity phải hợp lệ");

            await _cartService.AddToCartAsync(userId, request.ProductId, request.Quantity);
            return Ok("Thêm sản phẩm vào giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi thêm sản phẩm vào giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// DELETE: api/carts/{userId}/remove/{productId}
    /// Xóa sản phẩm khỏi giỏ hàng
    /// </summary>
    [HttpDelete("{userId}/remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(string userId, string productId)
    {
        try
        {
            await _cartService.RemoveFromCartAsync(userId, productId);
            return Ok("Xóa sản phẩm khỏi giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi xóa sản phẩm khỏi giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// PATCH: api/carts/{userId}/update/{productId}
    /// Cập nhật số lượng sản phẩm trong giỏ hàng
    /// Request body: { "quantity": 2 }
    /// </summary>
    [HttpPatch("{userId}/update/{productId}")]
    public async Task<IActionResult> UpdateCartItemQuantity(string userId, string productId, [FromBody] UpdateQuantityRequest request)
    {
        try
        {
            if (request.Quantity <= 0)
                return BadRequest("Số lượng phải lớn hơn 0");

            await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);
            return Ok("Cập nhật số lượng thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi cập nhật số lượng sản phẩm: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }

    /// <summary>
    /// DELETE: api/carts/{userId}/clear
    /// Làm trống giỏ hàng
    /// </summary>
    [HttpDelete("{userId}/clear")]
    public async Task<IActionResult> ClearCart(string userId)
    {
        try
        {
            await _cartService.ClearCartAsync(userId);
            return Ok("Giỏ hàng đã được làm trống");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Lỗi làm trống giỏ hàng: {ex.Message}");
            return StatusCode(500, "Lỗi server");
        }
    }
}

/// <summary>
/// Request model cho thêm sản phẩm vào giỏ hàng
/// </summary>
public class AddToCartRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// Request model cho cập nhật số lượng
/// </summary>
public class UpdateQuantityRequest
{
    public int Quantity { get; set; }
}
