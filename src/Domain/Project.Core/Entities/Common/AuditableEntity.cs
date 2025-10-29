namespace Project.Core.Entities.Common;

/// <summary>
/// Base entity with audit tracking capabilities
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class AuditableEntity<TKey> : BaseEntity<TKey> where TKey : notnull
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Auditable entity with Guid primary key
/// </summary>
public abstract class AuditableEntity : AuditableEntity<Guid>
{
    protected AuditableEntity()
    {
        Id = Guid.CreateVersion7();
    }
}