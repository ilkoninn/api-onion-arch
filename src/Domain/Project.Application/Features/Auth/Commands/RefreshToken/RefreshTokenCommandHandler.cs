namespace Project.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    public async Task<RefreshTokenCommandResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<RefreshTokenDto>(request);
        var result = await service.RefreshTokenAsync(dto, cancellationToken);

        return mapper.Map<RefreshTokenCommandResponse>(result);
    }
}