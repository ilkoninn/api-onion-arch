namespace Project.Infrastructure.Services;

/// <summary>
/// JWT token service - External infrastructure concern
/// </summary>
public sealed class TokenService(
    IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(User user, IEnumerable<string> roles)
    {
        // JWT generation logic
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };
        
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public Task<Guid?> ValidateRefreshTokenAsync(
        string refreshToken, 
        CancellationToken cancellationToken = default)
    {
        // Validation logic
        throw new NotImplementedException();
    }
}