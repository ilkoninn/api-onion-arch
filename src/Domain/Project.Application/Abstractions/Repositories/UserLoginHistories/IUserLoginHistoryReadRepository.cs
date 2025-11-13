namespace Project.Application.Abstractions.Repositories.UserLoginHistories;

/// <summary>
/// UserLoginHistory read repository interface
/// </summary>
public interface IUserLoginHistoryReadRepository : IReadRepository<UserLoginHistory>
{
    /// <summary>
    /// Gets login history for a specific user
    /// </summary>
    Task<IReadOnlyList<UserLoginHistory>> GetByUserIdAsync(
        Guid userId, 
        int? limit = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent failed login attempts for a user
    /// </summary>
    Task<IReadOnlyList<UserLoginHistory>> GetRecentFailedAttemptsAsync(
        Guid userId, 
        DateTime since, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets login history by IP address
    /// </summary>
    Task<IReadOnlyList<UserLoginHistory>> GetByIpAddressAsync(
        string ipAddress, 
        CancellationToken cancellationToken = default);
}