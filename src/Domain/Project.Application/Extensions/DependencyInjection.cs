namespace Project.Application.Extensions;

/// <summary>
/// Application layer dependency injection - Business logic orchestration and MediatR handlers
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register MediatR (if not already done)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register AutoMapper profiles and application services/repositories
        RegisterMappings(services);
        RegisterServices(services);
        RegisterRepositories(services);

        return services;
    }

    private static void RegisterMappings(IServiceCollection services)
    {
        // Example: Register AutoMapper profiles if using AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Use Scrutor for auto-registration of all classes implementing interfaces
        services.RegisterAssemblyPublicNonGenericClasses()
             .Where(c => c.Name.EndsWith("Service"))
             .AsPublicImplementedInterfaces();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // This method can be used to register repositories if needed
        services.RegisterAssemblyPublicNonGenericClasses()
              .Where(c => c.Name.EndsWith("Repository"))
              .AsPublicImplementedInterfaces();
    }
}
