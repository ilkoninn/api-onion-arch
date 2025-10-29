namespace Project.Application.Abstractions.Repositories;

/// <summary>
/// Generic write repository interface for command operations
/// </summary>
public interface IWriteRepository<T> where T : class
{
    // Create operations
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    // Update operations
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    
    // Delete operations
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Soft delete (if entity supports it)
    void SoftDelete(T entity);
    void SoftDeleteRange(IEnumerable<T> entities);
    
    // Save changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}