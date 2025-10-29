namespace Project.Application.Abstractions.Services.Domain;

/// <summary>
/// User domain service for business logic and orchestration
/// </summary>
public interface IUserService
{
    // Authentication
    Task<(User User, string Token, string RefreshToken)> RegisterAsync(
        string email,
        string password,
        string? firstName = null,
        string? lastName = null,
        CancellationToken cancellationToken = default);

    Task<(User User, string Token, string RefreshToken)> LoginAsync(
        string email,
        string password,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<(string Token, string RefreshToken)> RefreshTokenAsync(
        string refreshToken,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task RevokeTokenAsync(
        string refreshToken,
        string ipAddress,
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
