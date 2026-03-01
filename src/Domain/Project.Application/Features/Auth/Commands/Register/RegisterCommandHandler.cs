using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Project.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<RegisterCommandRequest, RegisterCommandResponse>
{
    public async Task<RegisterCommandResponse> Handle(
        RegisterCommandRequest request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<RegisterUserDto>(request);
        var result = await service.RegisterAsync(dto, cancellationToken);

        return mapper.Map<RegisterCommandResponse>(result);
    }
}