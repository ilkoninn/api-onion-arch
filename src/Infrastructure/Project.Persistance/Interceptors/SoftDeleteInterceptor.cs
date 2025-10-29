using Project.Application.Abstractions.Services.Infrastructure;

namespace Project.Persistance.Interceptors;

/// <summary>
/// Interceptor for automatically handling soft delete operations
/// </summary>
public sealed class SoftDeleteInterceptor(
    IClaimService claimService) : SaveChangesInterceptor
{

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var currentUser = claimService?.GetUserId() ?? "System";
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<SoftDeletableEntity<Guid>>())
        {
            if (entry.State == EntityState.Deleted)
            {
                // Convert hard delete to soft delete
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = utcNow;
                entry.Entity.DeletedBy = currentUser;
            }
        }
    }
}