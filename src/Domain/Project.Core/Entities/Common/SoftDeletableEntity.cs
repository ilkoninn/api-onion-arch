namespace Project.Core.Entities.Common;

/// <summary>
/// Base entity with soft delete support
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class SoftDeletableEntity<TKey> : AuditableEntity<TKey> where TKey : notnull
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

/// <summary>
/// Soft deletable entity with Guid primary key
/// </summary>
public abstract class SoftDeletableEntity : SoftDeletableEntity<Guid>
{
    protected SoftDeletableEntity()
    {
        Id = Guid.CreateVersion7();
    }
}