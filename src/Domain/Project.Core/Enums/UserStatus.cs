namespace Project.Core.Enums;

/// <summary>
/// User account status enumeration
/// </summary>
public enum UserStatus
{
    Pending = 0,      // Email not confirmed
    Active = 1,       // Active and can login
    Suspended = 2,    // Temporarily suspended
    Banned = 3        // Permanently banned
}