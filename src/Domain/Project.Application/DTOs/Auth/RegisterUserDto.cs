namespace Project.Application.DTOs.Auth;

public sealed record RegisterUserDto(
    string Email,
    string Password,
    string? FirstName,
    string? LastName
);

public sealed record RegisterUserResponseDto
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}