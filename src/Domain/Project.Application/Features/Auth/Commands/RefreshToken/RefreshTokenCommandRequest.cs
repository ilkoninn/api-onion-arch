namespace Project.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommandRequest(
    string RefreshToken,
    string IpAddress
) : IRequest<RefreshTokenCommandResponse>;

public sealed record RefreshTokenCommandResponse(
    string AccessToken,
    string RefreshToken
);