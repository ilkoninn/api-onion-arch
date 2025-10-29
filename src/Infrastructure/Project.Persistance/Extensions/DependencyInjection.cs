namespace Project.Persistance.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register interceptors
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        // Register DbContext with interceptors
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });

            // Add interceptors
            var auditInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            var softDeleteInterceptor = serviceProvider.GetRequiredService<SoftDeleteInterceptor>();
            options.AddInterceptors(auditInterceptor, softDeleteInterceptor);

            // Enable sensitive data logging in development
            var enableSensitiveDataLogging = configuration["Logging:EnableSensitiveDataLogging"];
            if (bool.TryParse(enableSensitiveDataLogging, out var isEnabled) && isEnabled)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // Register all services in the assembly
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