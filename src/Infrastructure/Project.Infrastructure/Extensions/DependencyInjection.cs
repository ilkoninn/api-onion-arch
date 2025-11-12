namespace Project.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register HttpContextAccessor (required by ClaimService)
        services.AddHttpContextAccessor();

        return services;
    }
}
