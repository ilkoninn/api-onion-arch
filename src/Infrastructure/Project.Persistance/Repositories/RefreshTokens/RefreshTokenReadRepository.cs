namespace Project.Persistance.Repositories.RefreshTokens;

/// <summary>
/// RefreshToken read repository implementation
/// </summary>
public sealed class RefreshTokenReadRepository : ReadRepository<RefreshToken>, IRefreshTokenReadRepository
{
    public RefreshTokenReadRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveTokensByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(rt => rt.UserId == userId 
                && rt.RevokedAt == null 
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetTokensByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetExpiredTokensAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}