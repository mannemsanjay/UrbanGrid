using UrbanGrid.Application.DTOs.Auth;

namespace UrbanGrid.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<UserDto> RegisterAsync(RegisterRequest request);
    Task<UserDto> GetProfileAsync(Guid userId);
    Task<UserDto> CreateStaffAsync(CreateStaffRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    // ✅ NEW
    Task<UserDto> ToggleStatusAsync(Guid userId, string status);
    Task DeleteUserAsync(Guid userId);
}
