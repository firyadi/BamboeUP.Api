namespace BamboeUp.Api.Tests.Database;

public class PhysicalInventoryDialectGuardTests
{
    private static string TableDir => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "Table"));

    [Fact]
    public void SqlServerMigration_HasInventoryCapturePermitAndUniqueKeys()
    {
        var sql = File.ReadAllText(Path.Combine(TableDir, "PhysicalInventory.sql"));
        Assert.Contains("[sync].[OfflineAccessPermit]", sql);
        Assert.Contains("[app].[InventoryItem]", sql);
        Assert.Contains("[app].[PhysicalInventoryDocument]", sql);
        Assert.Contains("[app].[PhysicalInventoryDocumentPage]", sql);
        Assert.Contains("[app].[PhysicalInventoryExpectedLine]", sql);
        Assert.Contains("[app].[PhysicalInventoryCapture]", sql);
        Assert.Contains("[app].[PhysicalInventoryCaptureLine]", sql);
        Assert.Contains("[ConcurrencyToken]", sql);
        Assert.Contains("UQ_PhysicalInventoryCapture_PhysicalInventoryCaptureGuid", sql);
        Assert.Contains("UQ_PhysicalInventoryCapture_OperationGuid", sql);
        Assert.Contains("UQ_PhysicalInventoryCaptureLine_CaptureBarcode_Active", sql);
        Assert.DoesNotContain("gen_random_uuid()", sql);
    }

    [Fact]
    public void PostgreSqlMigration_HasInventoryCapturePermitAndUniqueKeys()
    {
        var sql = File.ReadAllText(Path.Combine(TableDir, "PhysicalInventory.PostgreSQL.sql"));
        Assert.Contains("sync.\"OfflineAccessPermit\"", sql);
        Assert.Contains("app.\"InventoryItem\"", sql);
        Assert.Contains("app.\"PhysicalInventoryCapture\"", sql);
        Assert.Contains("app.\"PhysicalInventoryCaptureLine\"", sql);
        Assert.Contains("\"ConcurrencyToken\"", sql);
        Assert.Contains("UQ_PhysicalInventoryCapture_PhysicalInventoryCaptureGuid", sql);
        Assert.Contains("UQ_PhysicalInventoryCapture_OperationGuid", sql);
        Assert.Contains("UQ_PhysicalInventoryCaptureLine_CaptureBarcode_Active", sql);
        Assert.DoesNotContain("IDENTITY(1,1)", sql);
    }

    [Fact]
    public void MobileSyncMigration_KeepsDeviceAndOperationInbox()
    {
        var sql = File.ReadAllText(Path.Combine(TableDir, "MobileSync.sql"));
        Assert.Contains("[sync].[MobileDevice]", sql);
        Assert.Contains("[sync].[MobileSyncOperation]", sql);
    }
}
