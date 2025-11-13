using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Project.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IMapper mapper,
    IUserService service)
    : IRequestHandler<RegisterCommand, RegisterCommandResponse>
{
    public async Task<RegisterCommandResponse> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var dto = mapper.Map<RegisterUserDto>(request);
        var result = await service.RegisterAsync(dto, cancellationToken);

        return mapper.Map<RegisterCommandResponse>(result);
    }
}