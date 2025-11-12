namespace Project.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            RegisterServices(services);
            return services;
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(
                    Assembly.Load("Project.Application"),
                    Assembly.Load("Project.Persistance"),
                    Assembly.Load("Project.Infrastructure"))
                .AddClasses()                     // All Classes
                .AsImplementedInterfaces()        // Mapping Interfaces
                .WithScopedLifetime());           // Scoped lifetime
        }
    }
}
