using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.SignalR
{
    public static class SignalRDependencyInjection
    {
        public static IServiceCollection AddSignalRDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();

            return services;
        }
    }
}
