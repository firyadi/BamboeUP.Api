using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Services;
using Repository;

namespace BamboeUp.Api.Extensions
{
    public static class AuditConfig
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var auditConn = configuration.GetConnectionString("AuditConnection") 
                ?? throw new InvalidOperationException("Connection string 'AuditConnection' not found.");
            var mainConn = configuration.GetConnectionString("sqlConnection") 
                ?? throw new InvalidOperationException("Connection string 'sqlConnection' not found.");

            services.AddScoped<AuditRepositoryContext>(_ =>
                new AuditRepositoryContext(auditConn));

            services.AddScoped<IAuditService, AuditService>(provider =>
            {
                var auditService = new AuditService();
                auditService.ConfigureAudit(auditConn);
                auditService.ConfigureMain(mainConn);
                return auditService;
            });
        }
    }
}
