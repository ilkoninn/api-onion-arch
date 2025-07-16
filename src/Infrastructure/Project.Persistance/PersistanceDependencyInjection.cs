using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Core.Entities.Identities;
using Project.Persistance.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Persistance
{
    public static class PersistanceDependencyInjection
    {
        public static IServiceCollection AddPersistanceDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddIdentity();
            services.AddRepositories();
            services.AddServices();

            return services;
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // SQL Database connection string
            var connectionString = Environment.GetEnvironmentVariable("CloudConnection")
                                         ?? configuration.GetConnectionString("DefaultConnection");

            //services.AddDbContext<AppDbContext>(options => options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
            //    new MySqlServerVersion(new Version(8, 0, 21)),
            //    mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()));

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IDesignTimeDbContextFactory<AppDbContext>, AppDbContextFactory>();
        }

        private static void AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            // Repositories here !!!
        }

        private static void AddServices(this IServiceCollection services)
        {
            // Services here !!!
        }
    }
}
