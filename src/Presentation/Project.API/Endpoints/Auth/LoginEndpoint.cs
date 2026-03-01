namespace Project.API.Endpoints.Auth;

public sealed class LoginEndpoint(IMediator mediator) : Endpoint<LoginCommandRequest, LoginCommandResponse>
{
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Options(opt => opt
           .WithName("Login")
           .WithOpenApi()
            .WithSummary("User login")
            .WithDescription("Login with email and password"));
    }

    public override async Task HandleAsync(LoginCommandRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}