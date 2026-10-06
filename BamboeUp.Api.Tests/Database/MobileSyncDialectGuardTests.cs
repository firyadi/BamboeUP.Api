namespace BamboeUp.Api.Tests.Database;

public class MobileSyncDialectGuardTests
{
    private static string TableDir => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "Table"));

    [Fact]
    public void SqlServerMigration_HasDurableTenantScopedKeysAndConcurrencyToken()
    {
        var sql = File.ReadAllText(Path.Combine(TableDir, "MobileSync.sql"));

        Assert.Contains("[sync].[MobileDevice]", sql);
        Assert.Contains("[sync].[MobileSyncBatch]", sql);
        Assert.Contains("[sync].[MobileSyncOperation]", sql);
        Assert.Contains("UNIQUE ([OperationGuid], [CompanyId], [UserId])", sql);
        Assert.Contains("[SyncVersion] BIGINT NOT NULL", sql);
        Assert.Contains("ISJSON([ReceiptJson])", sql);
        Assert.DoesNotContain("gen_random_uuid()", sql);
    }

    [Fact]
    public void PostgreSqlMigration_HasDurableTenantScopedKeysAndConcurrencyToken()
    {
        var sql = File.ReadAllText(Path.Combine(TableDir, "MobileSync.PostgreSQL.sql"));

        Assert.Contains("sync.\"MobileDevice\"", sql);
        Assert.Contains("sync.\"MobileSyncBatch\"", sql);
        Assert.Contains("sync.\"MobileSyncOperation\"", sql);
        Assert.Contains("UNIQUE (\"OperationGuid\", \"CompanyId\", \"UserId\")", sql);
        Assert.Contains("\"SyncVersion\" BIGINT NOT NULL", sql);
        Assert.Contains("\"ReceiptJson\" JSONB", sql);
        Assert.Contains("ON sync.\"MobileSyncOperation\"", sql);
        Assert.DoesNotContain("IDENTITY(1,1)", sql);
    }
}
