namespace Project.Application.Abstractions.Services.Infrastructure;

/// <summary>
/// Password hashing and validation service
/// </summary>
public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
