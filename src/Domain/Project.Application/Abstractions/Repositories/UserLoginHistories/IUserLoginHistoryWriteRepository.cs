namespace Project.Application.Abstractions.Repositories.UserLoginHistories;

/// <summary>
/// UserLoginHistory write repository interface
/// </summary>
public interface IUserLoginHistoryWriteRepository : IWriteRepository<UserLoginHistory>
{
    /// <summary>
    /// Deletes login history older than specified date
    /// </summary>
    Task DeleteOlderThanAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}