namespace Project.Persistance.Repositories;

/// <summary>
/// Generic write repository implementation for command operations
/// </summary>
public class WriteRepository<T> : IWriteRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public WriteRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await DbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void UpdateRange(IEnumerable<T> entities)
        => DbSet.UpdateRange(entities);

    public virtual void Delete(T entity)
        => DbSet.Remove(entity);

    public virtual void DeleteRange(IEnumerable<T> entities)
        => DbSet.RemoveRange(entities);

    public virtual async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        if (entity is not null)
            DbSet.Remove(entity);
    }

    public virtual void SoftDelete(T entity)
    {
        if (entity is SoftDeletableEntity<Guid> softDeletable)
        {
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
            DbSet.Update(entity);
        }
        else
        {
            throw new InvalidOperationException($"{typeof(T).Name} does not support soft delete");
        }
    }

    public virtual void SoftDeleteRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
            SoftDelete(entity);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await Context.SaveChangesAsync(cancellationToken);
}