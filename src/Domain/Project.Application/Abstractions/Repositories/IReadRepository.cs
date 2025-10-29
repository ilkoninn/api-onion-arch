namespace Project.Application.Abstractions.Repositories;

/// <summary>
/// Generic read-only repository interface for query operations
/// </summary>
public interface IReadRepository<T> where T : class
{
    // Single entity queries
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, bool asNoTracking, CancellationToken cancellationToken = default);
    
    // Collection queries
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(bool asNoTracking, CancellationToken cancellationToken = default);
    
    // Filtered queries
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking,
        CancellationToken cancellationToken = default);
    
    // Single or default
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    // Existence checks
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    // Count operations
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    // Pagination
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    // Queryable for complex scenarios
    IQueryable<T> AsQueryable();
    IQueryable<T> AsQueryable(bool asNoTracking);
}