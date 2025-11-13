namespace Project.Persistance.Repositories.UserLoginHistories;

/// <summary>
/// UserLoginHistory write repository implementation
/// </summary>
public sealed class UserLoginHistoryWriteRepository : WriteRepository<UserLoginHistory>, IUserLoginHistoryWriteRepository
{
    public UserLoginHistoryWriteRepository(AppDbContext context) : base(context) { }

    public async Task DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var oldRecords = await DbSet
            .Where(lh => lh.AttemptedAt < olderThan)
            .ToListAsync(cancellationToken);

        DbSet.RemoveRange(oldRecords);
    }
}