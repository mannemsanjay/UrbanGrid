using UrbanGrid.Application.DTOs.Auth;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Interfaces;

namespace UrbanGrid.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    // ✅ Login
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        if (user.Status != "ACTIVE")
            throw new UnauthorizedAccessException("Account is not active");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            User = MapToDto(user)
        };
    }

    // ✅ Public self-registration — CITIZEN only
    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email.ToLower().Trim(),
            Phone = request.Phone,
            Role = request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            MustChangePassword = false
        };

        var created = await _userRepository.CreateAsync(user);
        return MapToDto(created);
    }

    // ✅ Admin creates staff — auto password + email sent
    public async Task<UserDto> CreateStaffAsync(CreateStaffRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var tempPassword = GenerateTempPassword();

        var user = new User
        {
            Name = request.Name,
            Email = request.Email.ToLower().Trim(),
            Phone = request.Phone,
            Role = request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
            MustChangePassword = true,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userRepository.CreateAsync(user);
        await _emailService.SendWelcomeEmailAsync(created.Email, created.Name, tempPassword);

        return MapToDto(created);
    }

    // ✅ Change password — clears MustChangePassword flag
    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.MustChangePassword = false;

        await _userRepository.UpdateAsync(user);
    }

    // ✅ Get own profile
    public async Task<UserDto> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");
        return MapToDto(user);
    }

    // ✅ Get all users — Admin only
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var (users, _) = await _userRepository.GetAllAsync(page: 1, limit: 1000);
        return users.Select(MapToDto);
    }

    // ✅ NEW — Toggle user status
    public async Task<UserDto> ToggleStatusAsync(Guid userId, string status)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        if (status != "ACTIVE" && status != "INACTIVE")
            throw new ArgumentException("Status must be ACTIVE or INACTIVE.");

        user.Status = status;
        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    // ✅ NEW — Delete user
    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        await _userRepository.DeleteAsync(userId);
    }

    private static string GenerateTempPassword()
    {
        var suffix = Guid.NewGuid().ToString("N")[..5].ToUpper();
        return $"Urb@{DateTime.UtcNow.Year}{suffix}";
    }

    private static UserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Phone = u.Phone,
        Role = u.Role.ToString(),
        Status = u.Status,
        MustChangePassword = u.MustChangePassword,
        CreatedAt = u.CreatedAt
    };
}
