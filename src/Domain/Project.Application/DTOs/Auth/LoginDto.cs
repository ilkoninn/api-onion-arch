namespace Project.Application.DTOs.Auth;

public sealed record LoginDto(
    string Email,
    string Password,
    string IpAddress
);

public sealed record LoginResponseDto(
    User User,
    string Token,
    string RefreshToken
);
