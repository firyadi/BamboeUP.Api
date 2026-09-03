using System.Data;
using Microsoft.Extensions.Options;
using Repository.Database;
using Shared.Settings;

namespace Repository;

public class AuditRepositoryContext
{
    private readonly DatabaseOptions _options;

    public AuditRepositoryContext(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public DatabaseProvider Provider => _options.Provider;

    public IDbConnection CreateConnection() =>
        DbConnectionFactory.CreateConnection(_options.Provider, _options.AuditConnectionString);
}
