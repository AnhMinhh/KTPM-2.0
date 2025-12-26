using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeonGadgetStore.Api.Data;
using NeonGadgetStore.Api.Models;
using NeonGadgetStore.Api.Services;

namespace NeonGadgetStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        AppDbContext db,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        JwtTokenService jwtTokenService)
    {
        _db = db;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
    }

    public sealed class SignUpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
    }

    public sealed class SignInRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        var usernameTaken = await _db.Profiles.AnyAsync(p => p.Username.ToLower() == normalizedUsername);
        if (usernameTaken)
        {
            return BadRequest(new { message = "Username already taken" });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
        }

        if (!await _roleManager.RoleExistsAsync("user"))
        {
            await _roleManager.CreateAsync(new IdentityRole("user"));
        }

        await _userManager.AddToRoleAsync(user, "user");

        var profile = new Profile
        {
            UserId = user.Id,
            Email = request.Email,
            Username = normalizedUsername,
            FullName = request.FullName,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        var token = await _jwtTokenService.CreateAccessTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            token,
            user = new { id = user.Id, email = user.Email },
            profile,
            roles
        });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        var token = await _jwtTokenService.CreateAccessTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new
        {
            token,
            user = new { id = user.Id, email = user.Email },
            profile,
            roles
        });
    }
}
