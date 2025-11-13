namespace Project.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string IpAddress
) : IRequest<LoginCommandResponse>;

public sealed record LoginCommandResponse(
    User User,
    string Token,
    string RefreshToken
);