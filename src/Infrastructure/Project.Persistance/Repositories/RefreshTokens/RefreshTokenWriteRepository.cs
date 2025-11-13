namespace Project.Persistance.Repositories.RefreshTokens;

/// <summary>
/// RefreshToken write repository implementation
/// </summary>
public sealed class RefreshTokenWriteRepository : WriteRepository<RefreshToken>, IRefreshTokenWriteRepository
{
    public RefreshTokenWriteRepository(AppDbContext context) : base(context) { }

    public async Task RevokeAllUserTokensAsync(
        Guid userId, 
        string revokedByIp, 
        CancellationToken cancellationToken = default)
    {
        var activeTokens = await DbSet
            .Where(rt => rt.UserId == userId 
                && rt.RevokedAt == null 
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);

        var revokedAt = DateTime.UtcNow;
        foreach (var token in activeTokens)
        {
            token.RevokedAt = revokedAt;
            token.RevokedByIp = revokedByIp;
        }

        DbSet.UpdateRange(activeTokens);
    }

    public async Task DeleteExpiredTokensAsync(
        DateTime olderThan, 
        CancellationToken cancellationToken = default)
    {
        var expiredTokens = await DbSet
            .Where(rt => rt.ExpiresAt < olderThan)
            .ToListAsync(cancellationToken);

        DbSet.RemoveRange(expiredTokens);
    }
}