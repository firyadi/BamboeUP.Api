using Shared.Settings;

namespace Repository.Database;

public interface ISqlDialect
{
    DatabaseProvider Provider { get; }

    /// <summary>After SELECT — e.g. "TOP (100) " for SQL Server, empty for PostgreSQL.</summary>
    string InsertTop(int maxRows);

    /// <summary>End of query — e.g. empty for SQL Server, " LIMIT 100" for PostgreSQL.</summary>
    string AppendLimit(int maxRows);

    /// <summary>Schema-qualified object — e.g. [app].[Asset] or "app"."Asset".</summary>
    string Qualify(string schema, string objectName);

    /// <summary>Suffix after INSERT for returning new identity — SCOPE_IDENTITY vs RETURNING.</summary>
    string ReturnIdentitySuffix(string columnName, string castType = "bigint");

    /// <summary>Case-sensitive contains filter — LIKE vs ILIKE.</summary>
    string LikeContains(string columnExpression, string parameterName);

    string UtcNowExpression { get; }

    /// <summary>NEWID() vs gen_random_uuid().</summary>
    string NewGuidExpression { get; }

    /// <summary>After FROM table — UPDLOCK hint (SQL Server) or empty (PostgreSQL).</summary>
    string TableLockHint { get; }

    /// <summary>End of SELECT — empty (SQL Server) or FOR UPDATE (PostgreSQL).</summary>
    string ForUpdateSuffix { get; }
}
