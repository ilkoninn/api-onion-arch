using Microsoft.AspNetCore.Http;

namespace Project.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string? FirstName,
    string? LastName
) : IRequest<RegisterCommandResponse>;

public sealed record RegisterCommandResponse(
    User User,
    string Token,
    string RefreshToken
);