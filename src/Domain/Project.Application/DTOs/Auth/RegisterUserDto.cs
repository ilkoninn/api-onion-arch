namespace Project.Application.DTOs.Auth;

public sealed record RegisterUserDto(
    string Email,
    string Password,
    string? FirstName,
    string? LastName
);

public sealed record RegisterUserResponseDto(
    User User,
    string Token,
    string RefreshToken
);