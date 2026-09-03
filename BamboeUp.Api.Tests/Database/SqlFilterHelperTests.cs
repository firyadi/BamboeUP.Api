using Dapper;
using Repository.Extensions;
using Repository.Database;

namespace BamboeUp.Api.Tests.Database;

public class SqlFilterHelperTests
{
    [Fact]
    public void BuildFilter_PostgreSql_UsesIlikeForContains()
    {
        var parameters = new Dapper.DynamicParameters();
        var filter = SqlFilterHelper.BuildFilter(
            "a.Name", "@name", "contains", parameters, "name", "Test", new PostgreSqlDialect());

        Assert.Contains("ILIKE", filter);
    }

    [Fact]
    public void BuildFilter_SqlServer_UsesLikeForContains()
    {
        var parameters = new Dapper.DynamicParameters();
        var filter = SqlFilterHelper.BuildFilter(
            "a.Name", "@name", "contains", parameters, "name", "Test", new SqlServerDialect());

        Assert.Contains("LIKE", filter);
        Assert.DoesNotContain("ILIKE", filter);
    }
}
