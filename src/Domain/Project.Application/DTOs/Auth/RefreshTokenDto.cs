namespace Project.Application.DTOs.Auth;

public sealed record RefreshTokenDto(
    string RefreshToken,
    string IpAddress
);

public sealed record RefreshTokenResponseDto(
    string Token,
    string RefreshToken
);