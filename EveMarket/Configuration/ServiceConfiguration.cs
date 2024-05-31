using System.Reflection;

namespace EveMarket.Configuration
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(ServiceConfiguration).Assembly));

        }
    }
}
