namespace Project.Application.Abstractions.Persistence;

/// <summary>
/// Unit of Work abstraction - belongs to APPLICATION layer
/// Application layer defines WHAT operations exist (contract)
/// Persistence layer defines HOW they work (implementation)
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets whether there is an active transaction
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// User read repository
    /// </summary>
    IUserReadRepository UserReadRepository { get; }

    /// <summary>
    /// User write repository
    /// </summary>
    IUserWriteRepository UserWriteRepository { get; }

    /// <summary>
    /// RefreshToken read repository
    /// </summary>
    IRefreshTokenReadRepository RefreshTokenReadRepository { get; }

    /// <summary>
    /// RefreshToken write repository
    /// </summary>
    IRefreshTokenWriteRepository RefreshTokenWriteRepository { get; }

    /// <summary>
    /// UserLoginHistory read repository
    /// </summary>
    IUserLoginHistoryReadRepository UserLoginHistoryReadRepository { get; }

    /// <summary>
    /// UserLoginHistory write repository
    /// </summary>
    IUserLoginHistoryWriteRepository UserLoginHistoryWriteRepository { get; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation within a transaction and returns a result
    /// </summary>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an operation within a transaction
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}