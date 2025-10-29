namespace Project.Application.Abstractions.Services.Infrastructure;

/// <summary>
/// JWT token generation and validation service
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
