namespace Project.Application.Features.Auth.Commands.RevokeToken;

public sealed record RevokeTokenCommand(
    string RefreshToken,
    string IpAddress
) : IRequest<RevokeTokenCommandResponse>;

public sealed record RevokeTokenCommandResponse(
    bool Success,
    string Message
);