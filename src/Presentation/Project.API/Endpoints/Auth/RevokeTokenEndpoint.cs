namespace Project.API.Endpoints.Auth;

public sealed class RevokeTokenEndpoint : Endpoint<RevokeTokenCommand>
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RevokeTokenEndpoint(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void Configure()
    {
        Post("/api/auth/revoke-token");
        AllowAnonymous();
        Options(opt => opt
            .WithName("Revoke Token")
            .WithOpenApi()
            .WithSummary("Revoke refresh token")
            .WithDescription("Revoke a refresh token"));
    }

    public override async Task HandleAsync(RevokeTokenCommand req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}