using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Project.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
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
                               assembly.GetTypes().Contains(i) && 
                               i.Name.StartsWith("I"))
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
