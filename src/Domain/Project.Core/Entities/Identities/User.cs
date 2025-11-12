using Project.Core.Entities.Common;
using Project.Core.Enums;

namespace Project.Core.Entities.Identities;

/// <summary>
/// User entity for Onion Architecture (anemic model - no domain behavior)
/// </summary>
public sealed class User : SoftDeletableEntity
{
    // Basic Information
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfileImageUrl { get; set; }

    // Security
    public string PasswordHash { get; set; } = string.Empty;
    public string? SecurityStamp { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }

    // Account Status
    public EUserStatus Status { get; set; } = EUserStatus.Active;
    public bool IsLocked { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }

    // Activity Tracking
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime? LastPasswordChangedAt { get; set; }

    // Navigations
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();

    // Computed (read-only)
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsActive => Status == EUserStatus.Active && !IsDeleted && !IsLocked;
}