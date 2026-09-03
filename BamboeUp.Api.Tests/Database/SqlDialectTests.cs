using Repository.Database;
using Shared.Settings;

namespace BamboeUp.Api.Tests.Database;

public class SqlDialectTests
{
    [Theory]
    [InlineData(DatabaseProvider.SqlServer)]
    [InlineData(DatabaseProvider.PostgreSql)]
    public void Factory_ReturnsDialectMatchingProvider(DatabaseProvider provider)
    {
        var dialect = SqlDialectFactory.Create(provider);
        Assert.Equal(provider, dialect.Provider);
    }

    [Fact]
    public void SqlServerDialect_QualifiesSchemaAndObject()
    {
        var dialect = new SqlServerDialect();
        Assert.Equal("[app].[Asset]", dialect.Qualify("app", "Asset"));
    }

    [Fact]
    public void SqlServerDialect_UsesTopAndScopeIdentity()
    {
        var dialect = new SqlServerDialect();
        Assert.Equal("TOP (50) ", dialect.InsertTop(50));
        Assert.Equal(string.Empty, dialect.AppendLimit(50));
        Assert.Contains("SCOPE_IDENTITY", dialect.ReturnIdentitySuffix("AssetId"));
    }

    [Fact]
    public void PostgreSqlDialect_QualifiesSchemaAndObject()
    {
        var dialect = new PostgreSqlDialect();
        Assert.Equal("\"app\".\"Asset\"", dialect.Qualify("app", "Asset"));
    }

    [Fact]
    public void PostgreSqlDialect_UsesLimitAndReturning()
    {
        var dialect = new PostgreSqlDialect();
        Assert.Equal(string.Empty, dialect.InsertTop(50));
        Assert.Equal(" LIMIT 50", dialect.AppendLimit(50));
        Assert.Equal(" RETURNING \"AssetId\"", dialect.ReturnIdentitySuffix("AssetId"));
    }

    [Fact]
    public void SqlServerDialect_UsesNewGuidAndLockHints()
    {
        var dialect = new SqlServerDialect();
        Assert.Equal("NEWID()", dialect.NewGuidExpression);
        Assert.Contains("UPDLOCK", dialect.TableLockHint);
        Assert.Equal(string.Empty, dialect.ForUpdateSuffix);
    }

    [Fact]
    public void PostgreSqlDialect_UsesGenRandomUuidAndForUpdate()
    {
        var dialect = new PostgreSqlDialect();
        Assert.Equal("gen_random_uuid()", dialect.NewGuidExpression);
        Assert.Equal(string.Empty, dialect.TableLockHint);
        Assert.Equal(" FOR UPDATE", dialect.ForUpdateSuffix);
    }

    [Fact]
    public void PostgreSqlDialect_UsesIlikeForContains()
    {
        var dialect = new PostgreSqlDialect();
        Assert.Contains("ILIKE", dialect.LikeContains("Name", "@Name"));
    }
}
