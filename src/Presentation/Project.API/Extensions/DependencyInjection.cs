namespace Project.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddAPIServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Authentication
        services.AddAuthentication()
            .AddBearerToken();

        // Authorization
        services.AddAuthorization();

        // Global Exception Middleware
        services.AddTransient<GlobalExceptionMiddleware>();

        // FastEndpoints
        services.AddFastEndpoints();

        return services;
    }

    public static WebApplication UseAPIMiddlewares(
        this WebApplication app)
    {
        // Development-only Swagger/OpenAPI
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Global built-in exception routing
        app.UseExceptionHandler(_ => { });

        // Our custom global exception handling logic
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // FastEndpoints routing
        app.UseFastEndpoints();

        return app;
    }
}
