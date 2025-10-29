namespace Project.Core.Entities.Common;

/// <summary>
/// Base entity with primary key support
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class BaseEntity<TKey> where TKey : notnull
{
    public TKey Id { get; set; } = default!;
}

/// <summary>
/// Base entity with Guid primary key
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity()
    {
        Id = Guid.CreateVersion7();
    }
}
