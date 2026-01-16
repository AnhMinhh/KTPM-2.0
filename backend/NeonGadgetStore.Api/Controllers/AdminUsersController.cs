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
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUsersController(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Profiles
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var result = new List<object>(users.Count);

        foreach (var p in users)
        {
            var identityUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == p.UserId);
            var roles = identityUser is null ? new List<string>() : new List<string>(await _userManager.GetRolesAsync(identityUser));

            result.Add(new
            {
                id = p.Id,
                user_id = p.UserId,
                email = p.Email,
                username = p.Username,
                full_name = p.FullName,
                created_at = p.CreatedAt,
                roles = roles.Count == 0 ? new List<string> { "user" } : roles
            });
        }

        return Ok(result);
    }

    public sealed class SetAdminRequest
    {
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }
    }

    [HttpPut("{userId}/admin")]
    public async Task<IActionResult> SetAdmin([FromRoute] string userId, [FromBody] SetAdminRequest request)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(actorId) && actorId == userId)
        {
            return BadRequest(new { message = "You cannot change your own admin role" });
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (!await _roleManager.RoleExistsAsync("admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
        }

        if (!await _roleManager.RoleExistsAsync("user"))
        {
            await _roleManager.CreateAsync(new IdentityRole("user"));
        }

        if (request.IsAdmin)
        {
            await _userManager.AddToRoleAsync(user, "admin");
        }
        else
        {
            await _userManager.RemoveFromRoleAsync(user, "admin");
        }

        return Ok(new { ok = true });
    }
}
