using Project.Core.Entities.Common;

namespace Project.Core.Entities.Identities;

/// <summary>
/// Join entity for User and Role many-to-many relationship
/// </summary>
public sealed class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime AssignedAt { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}