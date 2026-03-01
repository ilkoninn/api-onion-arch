namespace Project.Application.Features.Auth.Commands.Login;

public sealed record LoginCommandRequest(
    string Email,
    string Password,
    string IpAddress
) : IRequest<LoginCommandResponse>;

public sealed record LoginCommandResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}