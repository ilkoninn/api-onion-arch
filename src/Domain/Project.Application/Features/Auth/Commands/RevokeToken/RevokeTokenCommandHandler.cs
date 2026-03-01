namespace Project.Application.Features.Auth.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<RevokeTokenCommandRequest, RevokeTokenCommandResponse>
{
    public async Task<RevokeTokenCommandResponse> Handle(
        RevokeTokenCommandRequest request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<RevokeTokenDto>(request);
        await service.RevokeTokenAsync(
            dto,
            cancellationToken);

        return new(
            Success: true,
            Message: "Token successfully revoked");
    }
}