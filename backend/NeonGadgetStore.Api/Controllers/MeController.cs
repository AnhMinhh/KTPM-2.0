using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MeController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return Unauthorized();
        }

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            user = new { id = user.Id, email = user.Email },
            profile,
            roles
        });
    }

    public sealed class UpdateProfileRequest
    {
        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile is null)
        {
            return NotFound(new { message = "Profile not found" });
        }

        if (request.FullName is not null)
        {
            profile.FullName = request.FullName;
        }

        if (request.Phone is not null)
        {
            profile.Phone = request.Phone;
        }

        if (request.AvatarUrl is not null)
        {
            profile.AvatarUrl = request.AvatarUrl;
        }

        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { profile });
    }
}
