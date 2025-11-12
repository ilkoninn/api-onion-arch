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

        return services;
    }
}