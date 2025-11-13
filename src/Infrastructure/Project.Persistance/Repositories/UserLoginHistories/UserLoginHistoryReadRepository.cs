namespace Project.Persistance.Repositories.UserLoginHistories;

/// <summary>
/// UserLoginHistory read repository implementation
/// </summary>
public sealed class UserLoginHistoryReadRepository : ReadRepository<UserLoginHistory>, IUserLoginHistoryReadRepository
{
    public UserLoginHistoryReadRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<UserLoginHistory>> GetByUserIdAsync(
        Guid userId, 
        int? limit = null, 
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(lh => lh.UserId == userId)
            .OrderByDescending(lh => lh.AttemptedAt);

        if (limit.HasValue)
            query = (IOrderedQueryable<UserLoginHistory>)query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserLoginHistory>> GetRecentFailedAttemptsAsync(
        Guid userId, 
        DateTime since, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(lh => lh.UserId == userId 
                && !lh.IsSuccessful 
                && lh.AttemptedAt >= since)
            .OrderByDescending(lh => lh.AttemptedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserLoginHistory>> GetByIpAddressAsync(
        string ipAddress, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(lh => lh.IpAddress == ipAddress)
            .OrderByDescending(lh => lh.AttemptedAt)
            .ToListAsync(cancellationToken);
    }
}