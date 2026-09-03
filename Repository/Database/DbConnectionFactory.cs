using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using Shared.Settings;

namespace Repository.Database;

public static class DbConnectionFactory
{
    public static IDbConnection CreateConnection(DatabaseOptions options) =>
        CreateConnection(options.Provider, options.ConnectionString);

    public static IDbConnection CreateConnection(DatabaseProvider provider, string connectionString) =>
        provider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(connectionString),
            DatabaseProvider.PostgreSql => new NpgsqlConnection(connectionString),
            _ => throw new NotSupportedException($"Database provider '{provider}' is not supported.")
        };
}
