using Shared.Settings;

namespace Repository.Database;

public sealed class SqlServerDialect : ISqlDialect
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    public string InsertTop(int maxRows) => $"TOP ({maxRows}) ";

    public string AppendLimit(int maxRows) => string.Empty;

    public string Qualify(string schema, string objectName) => $"[{schema}].[{objectName}]";

    public string ReturnIdentitySuffix(string columnName, string castType = "bigint") =>
        $"; SELECT CAST(SCOPE_IDENTITY() as {castType});";

    public string LikeContains(string columnExpression, string parameterName) =>
        $"{columnExpression} LIKE {parameterName}";

    public string UtcNowExpression => "SYSUTCDATETIME()";

    public string NewGuidExpression => "NEWID()";

    public string TableLockHint => " WITH (UPDLOCK, ROWLOCK)";

    public string ForUpdateSuffix => string.Empty;
}
