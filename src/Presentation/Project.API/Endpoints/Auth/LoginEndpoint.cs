namespace Project.API.Endpoints.Auth;

public sealed class LoginEndpoint : Endpoint<LoginCommand, LoginCommandResponse>
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginEndpoint(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

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

    public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}