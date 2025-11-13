namespace Project.Infrastructure.Extensions;

/// <summary>
/// Infrastructure layer dependency injection - External concerns (JWT, Password Hashing, Claims)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // HTTP context accessor (required by ClaimService)
        services.AddHttpContextAccessor();

        // Register infrastructure services
        RegisterServices(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Use Scrutor for auto-registration of all classes implementing interfaces
        services.RegisterAssemblyPublicNonGenericClasses()
             .Where(c => c.Name.EndsWith("Service"))
             .AsPublicImplementedInterfaces();
    }
}
