namespace Project.API.Endpoints.Auth;

public sealed class RevokeTokenEndpoint(IMediator mediator) : Endpoint<RevokeTokenCommandRequest>
{
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

    public override async Task HandleAsync(RevokeTokenCommandRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);
        await Send.OkAsync(result, ct);
    }
}