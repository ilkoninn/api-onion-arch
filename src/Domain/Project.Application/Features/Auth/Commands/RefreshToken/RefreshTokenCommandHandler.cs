namespace Project.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
    public async Task<RefreshTokenCommandResponse> Handle(
        RefreshTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<RefreshTokenDto>(request);
        var result = await service.RefreshTokenAsync(dto, cancellationToken);

        return mapper.Map<RefreshTokenCommandResponse>(result);
    }
}