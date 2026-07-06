using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Modules;

namespace Service.Modules.Extensions
{
    public static class ServiceModulesConfiguration
    {
        public static void ConfigureModulesServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceModulesManager, ServiceModulesManager>();
        }
    }
}
