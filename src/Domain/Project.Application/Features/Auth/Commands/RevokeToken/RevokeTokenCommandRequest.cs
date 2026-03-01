namespace Project.Application.Features.Auth.Commands.RevokeToken;

public sealed record RevokeTokenCommandRequest(
    string RefreshToken,
    string IpAddress
) : IRequest<RevokeTokenCommandResponse>;

public sealed record RevokeTokenCommandResponse(
    bool Success,
    string Message
);