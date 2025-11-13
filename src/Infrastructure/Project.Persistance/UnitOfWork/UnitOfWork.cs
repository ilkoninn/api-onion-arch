namespace Project.Persistance.UnitOfWork;

/// <summary>
/// Unit of Work pattern for transaction management
/// </summary>
public sealed class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    private readonly Lazy<IUserReadRepository> _userReadRepository;
    private readonly Lazy<IUserWriteRepository> _userWriteRepository;
    private readonly Lazy<IRefreshTokenReadRepository> _refreshTokenReadRepository;
    private readonly Lazy<IRefreshTokenWriteRepository> _refreshTokenWriteRepository;
    private readonly Lazy<IUserLoginHistoryReadRepository> _userLoginHistoryReadRepository;
    private readonly Lazy<IUserLoginHistoryWriteRepository> _userLoginHistoryWriteRepository;

    public UnitOfWork(
        AppDbContext context,
        Lazy<IUserReadRepository> userReadRepository,
        Lazy<IUserWriteRepository> userWriteRepository,
        Lazy<IRefreshTokenReadRepository> refreshTokenReadRepository,
        Lazy<IRefreshTokenWriteRepository> refreshTokenWriteRepository,
        Lazy<IUserLoginHistoryReadRepository> userLoginHistoryReadRepository,
        Lazy<IUserLoginHistoryWriteRepository> userLoginHistoryWriteRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userReadRepository = userReadRepository ?? throw new ArgumentNullException(nameof(userReadRepository));
        _userWriteRepository = userWriteRepository ?? throw new ArgumentNullException(nameof(userWriteRepository));
        _refreshTokenReadRepository = refreshTokenReadRepository ?? throw new ArgumentNullException(nameof(refreshTokenReadRepository));
        _refreshTokenWriteRepository = refreshTokenWriteRepository ?? throw new ArgumentNullException(nameof(refreshTokenWriteRepository));
        _userLoginHistoryReadRepository = userLoginHistoryReadRepository ?? throw new ArgumentNullException(nameof(userLoginHistoryReadRepository));
        _userLoginHistoryWriteRepository = userLoginHistoryWriteRepository ?? throw new ArgumentNullException(nameof(userLoginHistoryWriteRepository));
    }

    public bool HasActiveTransaction => _currentTransaction != null;

    public IUserReadRepository UserReadRepository => _userReadRepository.Value;
    public IUserWriteRepository UserWriteRepository => _userWriteRepository.Value;
    public IRefreshTokenReadRepository RefreshTokenReadRepository => _refreshTokenReadRepository.Value;
    public IRefreshTokenWriteRepository RefreshTokenWriteRepository => _refreshTokenWriteRepository.Value;
    public IUserLoginHistoryReadRepository UserLoginHistoryReadRepository => _userLoginHistoryReadRepository.Value;
    public IUserLoginHistoryWriteRepository UserLoginHistoryWriteRepository => _userLoginHistoryWriteRepository.Value;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Log concurrency exception
            throw new InvalidOperationException("Concurrency conflict occurred while saving changes.", ex);
        }
        catch (DbUpdateException ex)
        {
            // Log database update exception
            throw new InvalidOperationException("Database update failed.", ex);
        }
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to rollback.");
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await using var transaction = await BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await operation(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await using var transaction = await BeginTransactionAsync(cancellationToken);

        try
        {
            await operation(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnitOfWork));
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _currentTransaction?.Dispose();
        _currentTransaction = null;
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        _disposed = true;
    }
}