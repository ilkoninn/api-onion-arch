namespace Project.Application.Abstractions.Repositories.RefreshTokens;

/// <summary>
/// RefreshToken write repository interface
/// </summary>
public interface IRefreshTokenWriteRepository : IWriteRepository<RefreshToken>
{
    /// <summary>
    /// Revokes all active tokens for a specific user
    /// </summary>
    Task RevokeAllUserTokensAsync(Guid userId, string revokedByIp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens older than specified date
    /// </summary>
    Task DeleteExpiredTokensAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}