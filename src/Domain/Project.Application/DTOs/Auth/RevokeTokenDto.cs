using Azure.Core;

namespace Project.Application.DTOs.Auth;

public sealed record RevokeTokenDto(
    string RefreshToken,
    string IpAddress
);

