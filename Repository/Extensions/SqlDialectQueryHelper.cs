using Repository.Database;
using Shared.Settings;

namespace Repository.Extensions;

public static class SqlDialectQueryHelper
{
    public static string SelectLimited(ISqlDialect dialect, int maxRows, string columns) =>
        $"SELECT {dialect.InsertTop(maxRows)}{columns}";

    public static string CoalesceEmpty(ISqlDialect dialect, string expression) =>
        dialect.Provider switch
        {
            DatabaseProvider.SqlServer => $"ISNULL({expression}, '')",
            DatabaseProvider.PostgreSql => $"COALESCE({expression}, '')",
            _ => throw new NotSupportedException($"Database provider '{dialect.Provider}' is not supported.")
        };

    /// <summary>
    /// Resolves a standard-reference display name for global (template) scope.
    /// Uses LEFT JOIN on both providers so PostgreSQL does not need fn_GetStandardReferenceItems.
    /// </summary>
    public static string BuildGlobalStandardReferenceLookup(
        ISqlDialect dialect,
        string resultAlias,
        string fkColumn,
        string referenceInitial)
    {
        var standardReference = dialect.Qualify("app", "StandardReference");
        var standardReferenceItem = dialect.Qualify("app", "StandardReferenceItem");
        var srAlias = $"sr_{resultAlias}";
        return $@"LEFT JOIN {standardReference} {srAlias}
    ON {srAlias}.StandardReferenceInitial = '{referenceInitial}'
   AND {srAlias}.StatusId > 0
LEFT JOIN {standardReferenceItem} {resultAlias}
    ON {resultAlias}.StandardReferenceId = {srAlias}.StandardReferenceId
   AND {resultAlias}.StandardReferenceItemId = a.{fkColumn}
   AND {resultAlias}.StatusId > 0";
    }

    /// <summary>
    /// Dialect-aware replacement for app.fn_GetStandardReferenceItems.
    /// Picks the highest-priority scope (office → company → global), then returns
    /// scope items if a scope exists, otherwise template items.
    /// </summary>
    public static string BuildStandardReferenceItemsQuery(ISqlDialect dialect)
    {
        var sr = dialect.Qualify("app", "StandardReference");
        var scope = dialect.Qualify("app", "StandardReferenceScope");
        var scopeItem = dialect.Qualify("app", "StandardReferenceScopeItem");
        var item = dialect.Qualify("app", "StandardReferenceItem");

        return $@"
WITH ReferenceData AS (
    SELECT
        SR.StandardReferenceId,
        SR.StandardReferenceGuid,
        SR.StandardReferenceInitial,
        SR.StandardReferenceName
    FROM {sr} SR
    WHERE SR.StatusId > 0
      AND SR.StandardReferenceInitial = @StandardReferenceInitial
),
CandidateScope AS (
    SELECT
        S.StandardReferenceScopeId,
        S.StandardReferenceGuid,
        CASE
            WHEN S.CompanyId = @CompanyId AND S.CompanyOfficeId = @CompanyOfficeId THEN 1
            WHEN S.CompanyId = @CompanyId AND S.CompanyOfficeId IS NULL THEN 2
            WHEN S.CompanyId IS NULL THEN 99
        END AS Priority
    FROM {scope} S
    INNER JOIN ReferenceData R ON R.StandardReferenceId = S.StandardReferenceId
    WHERE S.StatusId > 0
      AND (
            (S.CompanyId = @CompanyId AND S.CompanyOfficeId = @CompanyOfficeId)
         OR (S.CompanyId = @CompanyId AND S.CompanyOfficeId IS NULL)
         OR (S.CompanyId IS NULL)
      )
),
SelectedScope AS (
    {SelectLimited(dialect, 1, "*")}
    FROM CandidateScope
    ORDER BY Priority{dialect.AppendLimit(1)}
)
SELECT
    R.StandardReferenceId,
    R.StandardReferenceGuid,
    R.StandardReferenceInitial,
    R.StandardReferenceName,
    COALESCE(SI.StandardReferenceScopeItemId, TI.StandardReferenceItemId) AS StandardReferenceItemId,
    COALESCE(SI.StandardReferenceScopeItemGuid, TI.StandardReferenceItemGuid) AS StandardReferenceItemGuid,
    COALESCE(SI.StandardReferenceScopeItemInitial, TI.StandardReferenceItemInitial) AS StandardReferenceItemInitial,
    COALESCE(SI.StandardReferenceScopeItemName, TI.StandardReferenceItemName) AS StandardReferenceItemName,
    COALESCE(SI.StandardReferenceScopeItemValue, TI.StandardReferenceItemValue) AS StandardReferenceItemValue,
    COALESCE(SI.DisplayOrder, TI.DisplayOrder) AS DisplayOrder,
    CASE WHEN SS.StandardReferenceScopeId IS NULL THEN 'Template' ELSE 'Scope' END AS DataSource
FROM ReferenceData R
LEFT JOIN SelectedScope SS ON 1 = 1
LEFT JOIN {scopeItem} SI
    ON SS.StandardReferenceScopeId = SI.StandardReferenceScopeId
   AND SI.StatusId > 0
LEFT JOIN {item} TI
    ON SS.StandardReferenceScopeId IS NULL
   AND TI.StandardReferenceId = R.StandardReferenceId
   AND TI.StatusId > 0
ORDER BY DisplayOrder";
    }
}
