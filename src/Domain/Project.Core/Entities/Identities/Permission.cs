using Project.Core.Entities.Common;

namespace Project.Core.Entities.Identities;

/// <summary>
/// Represents a permission/claim in the system
/// </summary>
public sealed class Permission : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}