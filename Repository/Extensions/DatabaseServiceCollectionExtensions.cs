using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repository.Database;
using Shared.Settings;

namespace Repository.Extensions;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddBamboeUpDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var options = ResolveOptions(configuration);
        services.AddSingleton(options);
        services.Configure<DatabaseOptions>(o =>
        {
            o.Provider = options.Provider;
            o.ConnectionString = options.ConnectionString;
            o.AuditConnectionString = options.AuditConnectionString;
        });

        services.AddSingleton<ISqlDialect>(SqlDialectFactory.Create(options.Provider));
        services.AddScoped<RepositoryContext>();
        services.AddScoped<AuditRepositoryContext>();
        return services;
    }

    public static DatabaseOptions ResolveOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection(DatabaseOptions.SectionName);
        var options = section.Exists()
            ? section.Get<DatabaseOptions>() ?? new DatabaseOptions()
            : new DatabaseOptions();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            options.ConnectionString = configuration.GetConnectionString("sqlConnection")
                ?? throw new InvalidOperationException(
                    "Database connection not configured. Set Database:ConnectionString or ConnectionStrings:sqlConnection.");
        }

        if (string.IsNullOrWhiteSpace(options.AuditConnectionString))
        {
            options.AuditConnectionString = configuration.GetConnectionString("AuditConnection")
                ?? throw new InvalidOperationException(
                    "Audit connection not configured. Set Database:AuditConnectionString or ConnectionStrings:AuditConnection.");
        }

        return options;
    }
}
