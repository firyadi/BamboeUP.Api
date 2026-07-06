using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Modules;
using Service.Modules;

namespace Service
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();

            // + GENERATED_SERVICE_REGISTRATION
                        services.AddScoped<IBankService, BankService>();
            // + GENERATED_SERVICE_REGISTRATION_END
        }
    }
}
