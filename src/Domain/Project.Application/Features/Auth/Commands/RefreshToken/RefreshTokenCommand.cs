namespace Project.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken,
    string IpAddress
) : IRequest<RefreshTokenCommandResponse>;

public sealed record RefreshTokenCommandResponse(
    string Token,
    string RefreshToken
);