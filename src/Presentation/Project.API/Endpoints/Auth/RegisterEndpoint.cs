namespace Project.API.Endpoints.Auth;

public sealed class RegisterEndpoint(IMediator mediator) : Endpoint<RegisterCommandRequest, RegisterCommandResponse>
{
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

    public override async Task HandleAsync(RegisterCommandRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}