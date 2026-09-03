using Repository.Database;
using Repository.Extensions;

namespace BamboeUp.Api.Tests.Database;

public class SqlDialectQueryHelperTests
{
    [Fact]
    public void CoalesceEmpty_SqlServer_UsesIsNull()
    {
        var result = SqlDialectQueryHelper.CoalesceEmpty(new SqlServerDialect(), "a.Name");
        Assert.Equal("ISNULL(a.Name, '')", result);
    }

    [Fact]
    public void CoalesceEmpty_PostgreSql_UsesCoalesce()
    {
        var result = SqlDialectQueryHelper.CoalesceEmpty(new PostgreSqlDialect(), "a.Name");
        Assert.Equal("COALESCE(a.Name, '')", result);
    }

    [Fact]
    public void StandardReferenceLookup_SqlServer_UsesLeftJoinNotUdf()
    {
        var result = SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(
            new SqlServerDialect(), "v_Category", "SrCategory", "CertificationCategory");
        Assert.Contains("LEFT JOIN", result);
        Assert.Contains("[app].[StandardReference]", result);
        Assert.DoesNotContain("OUTER APPLY", result);
        Assert.DoesNotContain("fn_GetStandardReferenceItems", result);
    }

    [Fact]
    public void StandardReferenceLookup_PostgreSql_UsesLeftJoin()
    {
        var result = SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(
            new PostgreSqlDialect(), "v_Category", "SrCategory", "CertificationCategory");
        Assert.Contains("LEFT JOIN", result);
        Assert.Contains("\"app\".\"StandardReference\"", result);
        Assert.DoesNotContain("OUTER APPLY", result);
    }

    [Fact]
    public void StandardReferenceItemsQuery_DoesNotCallUdf()
    {
        var sql = SqlDialectQueryHelper.BuildStandardReferenceItemsQuery(new PostgreSqlDialect());
        Assert.Contains("WITH ReferenceData", sql);
        Assert.Contains("SelectedScope", sql);
        Assert.DoesNotContain("fn_GetStandardReferenceItems", sql);
        Assert.Contains(" LIMIT 1", sql);
    }
}
