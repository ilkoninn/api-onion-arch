using System.Reflection;

namespace Project.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register HttpContextAccessor (required by ClaimService)
        services.AddHttpContextAccessor();

        // Register services
        RegisterServices(services, Assembly.GetExecutingAssembly());

        return services;
    }

    private static void RegisterServices(IServiceCollection services, Assembly assembly)
    {
        var serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Implementation = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsPublic && 
                               !i.IsGenericType &&
                               i.Name.StartsWith("I") &&
                               // Allow interfaces from any assembly, not just current assembly
                               i != typeof(IDisposable) &&
                               i != typeof(IAsyncDisposable))
            })
            .Where(x => x.Interfaces.Any());

        foreach (var serviceType in serviceTypes)
        {
            foreach (var interfaceType in serviceType.Interfaces)
            {
                services.AddScoped(interfaceType, serviceType.Implementation);
            }
        }
    }
}
