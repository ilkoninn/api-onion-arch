using Project.Core.Entities.Common;

namespace Project.Core.Entities.Identities;

/// <summary>
/// Join entity for Role and Permission many-to-many relationship
/// </summary>
public sealed class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime AssignedAt { get; set; }

    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}