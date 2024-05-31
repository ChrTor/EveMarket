using EveMarket.HttpClients;
using System.Reflection;

namespace EveMarket.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(ServiceConfiguration).Assembly))
                .AddHttpClient()
                .AddTransient<EveClient>();

            return services;

        }
    }
}
