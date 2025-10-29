namespace Project.Core.Entities.Common;

/// <summary>
/// Base entity with multi-tenancy support
/// </summary>
/// <typeparam name="TKey">Type of the primary key</typeparam>
public abstract class MultiTenantEntity<TKey> : SoftDeletableEntity<TKey> where TKey : notnull
{
    public Guid TenantId { get; set; }
}

/// <summary>
/// Multi-tenant entity with Guid primary key
/// </summary>
public abstract class MultiTenantEntity : MultiTenantEntity<Guid>
{
    protected MultiTenantEntity()
    {
        Id = Guid.CreateVersion7();
    }
}