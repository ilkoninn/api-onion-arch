namespace Project.Application.Mappings;

public class AuthMP : Profile
{
    public AuthMP()
    {
        CreateMap<RegisterCommand, RegisterUserDto>();
        CreateMap<RegisterUserResponseDto, RegisterCommandResponse>();

        CreateMap<LoginCommand, LoginDto>();
        CreateMap<LoginResponseDto, LoginCommandResponse>();

        CreateMap<RefreshTokenCommand, RefreshTokenDto>();
        CreateMap<RefreshTokenResponseDto, RefreshTokenCommandResponse>();

        CreateMap<RevokeTokenCommand, RevokeTokenDto>();
    }
}
