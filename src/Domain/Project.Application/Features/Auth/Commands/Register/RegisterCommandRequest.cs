using Microsoft.AspNetCore.Http;

namespace Project.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommandRequest(
    string Email,
    string Password,
    string? FirstName,
    string? LastName
) : IRequest<RegisterCommandResponse>;