using Shared.Settings;

namespace Repository.Database;

public static class SqlDialectFactory
{
    public static ISqlDialect Create(DatabaseProvider provider) =>
        provider switch
        {
            DatabaseProvider.SqlServer => new SqlServerDialect(),
            DatabaseProvider.PostgreSql => new PostgreSqlDialect(),
            _ => throw new NotSupportedException($"Database provider '{provider}' is not supported.")
        };
}
