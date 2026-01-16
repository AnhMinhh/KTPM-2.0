using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class PasswordResetController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<PasswordResetController> _logger;

    public PasswordResetController(UserManager<ApplicationUser> userManager, ILogger<PasswordResetController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Reset password cho admin (Development only)
    /// POST: /api/auth/reset-admin-password
    /// </summary>
    [HttpPost("reset-admin-password")]
    public async Task<IActionResult> ResetAdminPassword([FromBody] ResetAdminPasswordRequest request)
    {
        try
        {
            _logger.LogInformation($"Reset request received with password: {request?.NewPassword}");
            
            if (request == null || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new { error = "Password không được để trống" });

            var admin = await _userManager.FindByEmailAsync("admin@example.com");
            if (admin == null)
            {
                _logger.LogError("Admin account not found");
                return NotFound(new { error = "Admin account không tìm thấy" });
            }

            _logger.LogInformation($"Found admin: {admin.Email}");

            // Remove old password if exists
            var hasPassword = await _userManager.HasPasswordAsync(admin);
            if (hasPassword)
            {
                var removeResult = await _userManager.RemovePasswordAsync(admin);
                if (!removeResult.Succeeded)
                {
                    _logger.LogError($"Failed to remove old password: {string.Join(",", removeResult.Errors.Select(e => e.Description))}");
                }
            }

            // Set new password
            var result = await _userManager.AddPasswordAsync(admin, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                _logger.LogError($"Failed to set password: {errors}");
                return BadRequest(new { error = $"Lỗi set password: {errors}" });
            }

            _logger.LogInformation("Admin password reset successfully");
            return Ok(new { message = "Admin password đã được reset thành công", email = "admin@example.com", password = request.NewPassword });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in ResetAdminPassword: {ex.Message}\n{ex.StackTrace}");
            return StatusCode(500, new { error = $"Lỗi server: {ex.Message}" });
        }
    }
}

public class ResetAdminPasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}
