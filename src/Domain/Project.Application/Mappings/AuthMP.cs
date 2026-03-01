namespace Project.Application.Mappings;

public class AuthMP : Profile
{
    public AuthMP()
    {
        // Register Command -> DTO mappings
        CreateMap<RegisterCommandRequest, RegisterUserDto>();
        
        // Register Response DTO -> Command Response
        CreateMap<RegisterUserResponseDto, RegisterCommandResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.Token));

        // Login Command -> DTO mappings
        CreateMap<LoginCommandRequest, LoginDto>();
        
        // Login Response DTO -> Command Response
        CreateMap<LoginResponseDto, LoginCommandResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.Token));

        // RefreshToken Command -> DTO mappings
        CreateMap<RefreshTokenCommandRequest, RefreshTokenDto>();
        
        // RefreshToken Response DTO -> Command Response
        CreateMap<RefreshTokenResponseDto, RefreshTokenCommandResponse>()
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.AccessToken ?? string.Empty))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken ?? string.Empty));

        // RevokeToken Command -> DTO mappings
        CreateMap<RevokeTokenCommandRequest, RevokeTokenDto>();

        // User Entity -> RegisterUserResponseDto (RegisterAsync üçün)
        CreateMap<User, RegisterUserResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());

        // User Entity -> LoginResponseDto (LoginAsync üçün)
        CreateMap<User, LoginResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore());
    }
}
