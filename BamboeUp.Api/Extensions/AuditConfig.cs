using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Services;
using Repository.Extensions;

namespace BamboeUp.Api.Extensions;

public static class AuditConfig
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var options = DatabaseServiceCollectionExtensions.ResolveOptions(configuration);

        services.AddScoped<IAuditService, AuditService>(_ =>
        {
            var auditService = new AuditService();
            auditService.ConfigureProvider(options.Provider.ToString());
            auditService.ConfigureAudit(options.AuditConnectionString);
            auditService.ConfigureMain(options.ConnectionString);
            return auditService;
        });
    }
}
