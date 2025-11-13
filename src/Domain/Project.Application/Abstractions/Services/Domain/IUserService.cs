namespace Project.Application.Abstractions.Services.Domain;

/// <summary>
/// User domain service for business logic and orchestration
/// </summary>
public interface IUserService
{
    // Authentication
    Task<RegisterUserResponseDto> RegisterAsync(
        RegisterUserDto dto,
        CancellationToken cancellationToken = default);

    Task<LoginResponseDto> LoginAsync(
        LoginDto dto,
        CancellationToken cancellationToken = default);

    Task<RefreshTokenResponseDto> RefreshTokenAsync(
        RefreshTokenDto dto,
        CancellationToken cancellationToken = default);

    Task RevokeTokenAsync(
        RevokeTokenDto dto,
        CancellationToken cancellationToken = default);

    // User Management
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);

    // Password Management
    Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(
        string email,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default);

  // Role Management
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    // Account Management
    Task LockUserAsync(Guid userId, DateTime? lockoutEnd, CancellationToken cancellationToken = default);
    Task UnlockUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);
}
