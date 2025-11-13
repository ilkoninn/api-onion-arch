using NetCore.AutoRegisterDi;

namespace Project.Persistance.Extensions;

/// <summary>
/// Persistence layer dependency injection - Database and transaction management
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register interceptors as scoped services (needed by DbContext)
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

        // Register repositories
        RegisterRepositories(services);

        // Register Lazy<T> wrappers for repositories (required by UnitOfWork)
        services.AddScoped(sp => new Lazy<IUserReadRepository>(() => sp.GetRequiredService<IUserReadRepository>()));
        services.AddScoped(sp => new Lazy<IUserWriteRepository>(() => sp.GetRequiredService<IUserWriteRepository>()));
        services.AddScoped(sp => new Lazy<IRefreshTokenReadRepository>(() => sp.GetRequiredService<IRefreshTokenReadRepository>()));
        services.AddScoped(sp => new Lazy<IRefreshTokenWriteRepository>(() => sp.GetRequiredService<IRefreshTokenWriteRepository>()));
        services.AddScoped(sp => new Lazy<IUserLoginHistoryReadRepository>(() => sp.GetRequiredService<IUserLoginHistoryReadRepository>()));
        services.AddScoped(sp => new Lazy<IUserLoginHistoryWriteRepository>(() => sp.GetRequiredService<IUserLoginHistoryWriteRepository>()));

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, U.UnitOfWork>();

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // This method can be used to register repositories if needed
        services.RegisterAssemblyPublicNonGenericClasses()
              .Where(c => c.Name.EndsWith("Repository"))
              .AsPublicImplementedInterfaces();
    }
}