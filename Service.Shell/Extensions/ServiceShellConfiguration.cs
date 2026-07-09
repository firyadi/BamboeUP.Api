using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Shell;

namespace Service.Shell.Extensions
{
    public static class ServiceShellConfiguration
    {
        public static void ConfigureShellServices(this IServiceCollection services)
        {
            services.AddSingleton<IAdminRegistryService, AdminRegistryService>();
            services.AddScoped<IServiceShellManager, ServiceShellManager>();
        }
    }
}
