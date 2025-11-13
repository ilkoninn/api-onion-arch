namespace Project.Application.Abstractions.Repositories.RefreshTokens;

/// <summary>
/// RefreshToken read repository interface
/// </summary>
public interface IRefreshTokenReadRepository : IReadRepository<RefreshToken>
{
    /// <summary>
    /// Gets a refresh token by its token string
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active refresh tokens for a specific user
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all refresh tokens for a specific user
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired tokens that need to be cleaned up
    /// </summary>
    Task<IReadOnlyList<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
}