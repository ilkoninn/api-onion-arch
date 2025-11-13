namespace Project.API.Endpoints.Auth;

public sealed class RegisterEndpoint : Endpoint<RegisterCommand, RegisterCommandResponse>
{
    private readonly IMediator _mediator;

    public RegisterEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Options(opt => opt
            .WithName("Register")
            .WithOpenApi()
            .WithSummary("User registration")
            .WithDescription("Register a new user"));
            
    }

    public override async Task HandleAsync(RegisterCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}