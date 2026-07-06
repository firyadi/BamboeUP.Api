using Microsoft.Extensions.DependencyInjection;
using Service.Contracts.Shell;

namespace Service.Shell.Extensions;

/// <summary>
/// Legacy configuration - kept only for backward compatibility with API Generator tooling.
/// The actual shell services are registered via ConfigureShellServices() in ServiceShellConfiguration.
/// </summary>
public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        // Services are registered via IServiceShellManager in ServiceShellConfiguration.
        // This method is kept as a placeholder for the API Generator tool.
        // + GENERATED_SERVICE_REGISTRATION
        // + GENERATED_SERVICE_REGISTRATION_END
    }
}
