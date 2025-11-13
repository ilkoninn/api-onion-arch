
namespace Project.API.Endpoints.Auth;

public sealed class RefreshTokenEndpoint : Endpoint<RefreshTokenCommand, RefreshTokenCommandResponse>
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshTokenEndpoint(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh-token");
        AllowAnonymous();
        Options(opt => opt
            .WithName("Refresh Token")
             .WithOpenApi()
            .WithSummary("Refresh access token")
            .WithDescription("Get a new access token using refresh token"));
    }

    public override async Task HandleAsync(RefreshTokenCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}