using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrbanGrid.Application.DTOs.Auth;
using UrbanGrid.Application.DTOs.Common;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) =>
        _authService = authService;

    // ✅ Public — all roles can login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<AuthResponse>.Success(result));
    }

    // ✅ Public — CITIZEN role forced
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        request.Role = UserRole.CITIZEN;
        var result = await _authService.RegisterAsync(request);
        return Ok(ApiResponse<UserDto>.Success(result, "Registered successfully"));
    }

    // ✅ Admin only — create staff account
    [HttpPost("create-user")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateUser([FromBody] CreateStaffRequest dto)
    {
        var result = await _authService.CreateStaffAsync(dto);
        return Ok(ApiResponse<UserDto>.Success(
            result, $"Account created. Credentials sent to {dto.Email}"));
    }

    // ✅ Authenticated — change password
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _authService.ChangePasswordAsync(userId, request);
        return Ok(ApiResponse<object>.Success(null, "Password changed successfully"));
    }

    // ✅ Authenticated — get own profile
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _authService.GetProfileAsync(userId);
        return Ok(ApiResponse<object>.Success(new { user = result }));
    }

    // ✅ Admin only — get all users
    [HttpGet("users")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _authService.GetAllUsersAsync();
        return Ok(ApiResponse<object>.Success(new { users = result }));
    }

    // ✅ Admin only — toggle user status (ACTIVE / INACTIVE)
    [HttpPatch("users/{id}/status")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ToggleStatus(Guid id, [FromBody] ToggleStatusRequest request)
    {
        var result = await _authService.ToggleStatusAsync(id, request.Status);
        return Ok(ApiResponse<UserDto>.Success(result, "Status updated successfully"));
    }

    // ✅ Admin only — delete user
    [HttpDelete("users/{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _authService.DeleteUserAsync(id);
        return Ok(ApiResponse<object>.Success(null, "User deleted successfully"));
    }
}
