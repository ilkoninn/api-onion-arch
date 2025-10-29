using Project.Core.Entities.Common;

namespace Project.Core.Entities.Identities;

/// <summary>
/// Tracks user login attempts for security auditing
/// </summary>
public sealed class UserLoginHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime AttemptedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }

    public User User { get; set; } = null!;
}