namespace Project.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
{
    public async Task<LoginCommandResponse> Handle(
        LoginCommandRequest request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<LoginDto>(request);
        var result = await service.LoginAsync(dto, cancellationToken);

        return mapper.Map<LoginCommandResponse>(result);
    }
}