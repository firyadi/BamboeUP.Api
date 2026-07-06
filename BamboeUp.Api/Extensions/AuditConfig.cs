using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Services;

namespace BamboeUp.Api.Extensions
{
    public static class AuditConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuditService, AuditService>(provider =>
            {
                var auditService = new AuditService();
                var connStr = configuration.GetConnectionString("AuditConnection");
                var mainConnStr = configuration.GetConnectionString("sqlConnection");
                auditService.ConfigureAudit(connStr);
                auditService.ConfigureMain(mainConnStr);
                return auditService;
            });
        }
    }
}
