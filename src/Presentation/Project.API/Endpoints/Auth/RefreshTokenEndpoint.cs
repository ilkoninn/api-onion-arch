namespace Project.API.Endpoints.Auth;

public sealed class RefreshTokenEndpoint(IMediator mediator) : Endpoint<RefreshTokenCommandRequest, RefreshTokenCommandResponse>
{
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

    public override async Task HandleAsync(RefreshTokenCommandRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}