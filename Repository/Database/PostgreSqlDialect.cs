using Shared.Settings;

namespace Repository.Database;

public sealed class PostgreSqlDialect : ISqlDialect
{
    public DatabaseProvider Provider => DatabaseProvider.PostgreSql;

    public string InsertTop(int maxRows) => string.Empty;

    public string AppendLimit(int maxRows) => $" LIMIT {maxRows}";

    public string Qualify(string schema, string objectName) => $"\"{schema}\".\"{objectName}\"";

    public string ReturnIdentitySuffix(string columnName, string castType = "bigint") =>
        $" RETURNING \"{columnName}\"";

    public string LikeContains(string columnExpression, string parameterName) =>
        $"{columnExpression} ILIKE {parameterName}";

    public string UtcNowExpression => "(NOW() AT TIME ZONE 'UTC')";

    public string NewGuidExpression => "gen_random_uuid()";

    public string TableLockHint => string.Empty;

    public string ForUpdateSuffix => " FOR UPDATE";
}
