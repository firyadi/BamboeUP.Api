using Contracts;
using Dapper;
using Repository.Database;
using Shared.DataTransferObjects;
using Shared.Settings;
using System.Data;

namespace Repository;

public sealed class PhysicalInventoryRepository(RepositoryContext context, ISqlDialect dialect) : IPhysicalInventoryRepository
{
    private readonly string _permit = dialect.Qualify("sync", "OfflineAccessPermit");
    private readonly string _item = dialect.Qualify("app", "InventoryItem");
    private readonly string _document = dialect.Qualify("app", "PhysicalInventoryDocument");
    private readonly string _page = dialect.Qualify("app", "PhysicalInventoryDocumentPage");
    private readonly string _expected = dialect.Qualify("app", "PhysicalInventoryExpectedLine");
    private readonly string _capture = dialect.Qualify("app", "PhysicalInventoryCapture");
    private readonly string _line = dialect.Qualify("app", "PhysicalInventoryCaptureLine");

    private string Q(string identifier) =>
        dialect.Provider == DatabaseProvider.PostgreSql ? $"\"{identifier}\"" : $"[{identifier}]";

    public async Task UpsertPermitAsync(OfflineAccessPermitDto permit, string permitJson, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;
        var args = new
        {
            permit.DeviceGuid,
            permit.UserId,
            permit.UserGuid,
            permit.CompanyId,
            OfficeId = permit.OfficeId,
            permit.AppCode,
            permit.CanCapture,
            permit.CanVerify,
            permit.PermissionVersion,
            IssuedAt = permit.IssuedAt.UtcDateTime,
            ExpiresAt = permit.ExpiresAt.UtcDateTime,
            PermitJson = permitJson,
            Now = DateTime.UtcNow
        };

        var updateSql = $"""
            UPDATE {_permit}
            SET {Q("CanCapture")}=@CanCapture, {Q("CanVerify")}=@CanVerify, {Q("PermissionVersion")}=@PermissionVersion,
                {Q("IssuedAt")}=@IssuedAt, {Q("ExpiresAt")}=@ExpiresAt, {Q("PermitJson")}={(dialect.Provider == DatabaseProvider.PostgreSql ? "CAST(@PermitJson AS jsonb)" : "@PermitJson")},
                {Q("UpdatedTime")}=@Now, {Q("UpdatedById")}=@UserId
            WHERE {Q("DeviceGuid")}=@DeviceGuid AND {Q("UserId")}=@UserId AND {Q("AppCode")}=@AppCode
              AND {Q("DeletedTime")} IS NULL AND {Q("StatusId")} > 0 AND {Q("RevokedTime")} IS NULL
            """;
        var updated = await connection.ExecuteAsync(updateSql, args, transaction);
        if (updated > 0)
            return;

        var insertSql = dialect.Provider == DatabaseProvider.PostgreSql
            ? $"""
                INSERT INTO {_permit}
                    ("OfflineAccessPermitGuid","DeviceGuid","UserId","UserGuid","CompanyId","CompanyOfficeId","AppCode",
                     "CanCapture","CanVerify","PermissionVersion","IssuedAt","ExpiresAt","PermitJson","CreatedTime","CreatedById")
                VALUES
                    (gen_random_uuid(), @DeviceGuid, @UserId, @UserGuid, @CompanyId, @OfficeId, @AppCode,
                     @CanCapture, @CanVerify, @PermissionVersion, @IssuedAt, @ExpiresAt, CAST(@PermitJson AS jsonb), @Now, @UserId)
                """
            : $"""
                INSERT INTO {_permit}
                    ([OfflineAccessPermitGuid],[DeviceGuid],[UserId],[UserGuid],[CompanyId],[CompanyOfficeId],[AppCode],
                     [CanCapture],[CanVerify],[PermissionVersion],[IssuedAt],[ExpiresAt],[PermitJson],[CreatedTime],[CreatedById])
                VALUES
                    (NEWID(), @DeviceGuid, @UserId, @UserGuid, @CompanyId, @OfficeId, @AppCode,
                     @CanCapture, @CanVerify, @PermissionVersion, @IssuedAt, @ExpiresAt, @PermitJson, @Now, @UserId)
                """;
        await connection.ExecuteAsync(insertSql, args, transaction);
    }

    public Task<PhysicalInventoryCaptureRecord?> GetCaptureByOperationAsync(Guid operationGuid, long companyId, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? context.CreateConnection();
        var sql = $"""
            SELECT {Q("PhysicalInventoryCaptureId")} AS PhysicalInventoryCaptureId,
                   {Q("PhysicalInventoryCaptureGuid")} AS PhysicalInventoryCaptureGuid,
                   {Q("OperationGuid")} AS OperationGuid,
                   {Q("PayloadHash")} AS PayloadHash,
                   {Q("CaptureStatus")} AS CaptureStatus,
                   {Q("ConcurrencyToken")} AS ConcurrencyToken,
                   {Q("CounterUserId")} AS CounterUserId
            FROM {_capture}
            WHERE {Q("OperationGuid")}=@operationGuid AND {Q("CompanyId")}=@companyId
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return conn.QuerySingleOrDefaultAsync<PhysicalInventoryCaptureRecord>(sql, new { operationGuid, companyId }, transaction);
    }

    public Task<PhysicalInventoryCaptureRecord?> GetCaptureByGuidAsync(Guid captureGuid, long companyId, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? context.CreateConnection();
        var sql = $"""
            SELECT {Q("PhysicalInventoryCaptureId")} AS PhysicalInventoryCaptureId,
                   {Q("PhysicalInventoryCaptureGuid")} AS PhysicalInventoryCaptureGuid,
                   {Q("OperationGuid")} AS OperationGuid,
                   {Q("PayloadHash")} AS PayloadHash,
                   {Q("CaptureStatus")} AS CaptureStatus,
                   {Q("ConcurrencyToken")} AS ConcurrencyToken,
                   {Q("CounterUserId")} AS CounterUserId
            FROM {_capture}
            WHERE {Q("PhysicalInventoryCaptureGuid")}=@captureGuid AND {Q("CompanyId")}=@companyId
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return conn.QuerySingleOrDefaultAsync<PhysicalInventoryCaptureRecord>(sql, new { captureGuid, companyId }, transaction);
    }

    public async Task<bool> OtherCaptureExistsAsync(string documentNumber, int pageNumber, long companyId, Guid captureGuid, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT COUNT(1) FROM {_capture}
            WHERE {Q("CompanyId")}=@companyId AND {Q("DocumentNumber")}=@documentNumber AND {Q("PageNumber")}=@pageNumber
              AND {Q("PhysicalInventoryCaptureGuid")} <> @captureGuid
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return await transaction.Connection!.ExecuteScalarAsync<int>(
            sql, new { companyId, documentNumber, pageNumber, captureGuid }, transaction) > 0;
    }

    public async Task<(long? DocumentId, long? PageId)> ResolveDocumentPageAsync(
        string documentNumber, int pageNumber, long companyId, IDbTransaction transaction)
    {
        var documentSql = $"""
            SELECT {Q("PhysicalInventoryDocumentId")} FROM {_document}
            WHERE {Q("CompanyId")}=@companyId AND {Q("DocumentNumber")}=@documentNumber
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        var documentId = await transaction.Connection!.QuerySingleOrDefaultAsync<long?>(
            documentSql, new { companyId, documentNumber }, transaction);
        if (documentId is null)
            return (null, null);

        var pageSql = $"""
            SELECT {Q("PhysicalInventoryDocumentPageId")} FROM {_page}
            WHERE {Q("PhysicalInventoryDocumentId")}=@documentId AND {Q("PageNumber")}=@pageNumber
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        var pageId = await transaction.Connection!.QuerySingleOrDefaultAsync<long?>(
            pageSql, new { documentId, pageNumber }, transaction);
        return (documentId, pageId);
    }

    public Task<InventoryItemMatch?> FindItemByBarcodeAsync(string barcode, long companyId, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT {Q("InventoryItemId")} AS InventoryItemId, {Q("ItemCode")} AS ItemCode, {Q("ItemName")} AS ItemName
            FROM {_item}
            WHERE {Q("CompanyId")}=@companyId AND {Q("Barcode")}=@barcode
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return transaction.Connection!.QuerySingleOrDefaultAsync<InventoryItemMatch>(
            sql, new { companyId, barcode }, transaction);
    }

    public Task<decimal?> GetExpectedQuantityAsync(long pageId, long inventoryItemId, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT {Q("ExpectedQuantity")} FROM {_expected}
            WHERE {Q("PhysicalInventoryDocumentPageId")}=@pageId AND {Q("InventoryItemId")}=@inventoryItemId
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return transaction.Connection!.QuerySingleOrDefaultAsync<decimal?>(
            sql, new { pageId, inventoryItemId }, transaction);
    }

    public async Task<long> InsertCaptureAsync(PhysicalInventoryCaptureInsert insert, IDbTransaction transaction)
    {
        var sql = $"""
            INSERT INTO {_capture}
                ({Q("PhysicalInventoryCaptureGuid")},{Q("CompanyId")},{Q("CompanyOfficeId")},{Q("DeviceGuid")},{Q("OperationGuid")},
                 {Q("DocumentNumber")},{Q("PageNumber")},{Q("PhysicalInventoryDocumentId")},{Q("PhysicalInventoryDocumentPageId")},
                 {Q("CounterUserId")},{Q("CounterUserGuid")},{Q("CaptureStatus")},{Q("PayloadHash")},{Q("ConcurrencyToken")},
                 {Q("StartedAt")},{Q("FinishedAt")},{Q("UploadedAt")},{Q("CreatedById")},{Q("CreatedTime")})
            VALUES
                (@CaptureGuid,@CompanyId,@OfficeId,@DeviceGuid,@OperationGuid,
                 @DocumentNumber,@PageNumber,@DocumentId,@PageId,
                 @CounterUserId,@CounterUserGuid,@CaptureStatus,@PayloadHash,1,
                 @StartedAt,@FinishedAt,@Now,@UserId,@Now)
            {dialect.ReturnIdentitySuffix("PhysicalInventoryCaptureId")}
            """;
        return await transaction.Connection!.QuerySingleAsync<long>(sql, new
        {
            insert.CaptureGuid,
            insert.CompanyId,
            insert.OfficeId,
            insert.DeviceGuid,
            insert.OperationGuid,
            insert.DocumentNumber,
            insert.PageNumber,
            insert.DocumentId,
            insert.PageId,
            insert.CounterUserId,
            insert.CounterUserGuid,
            insert.CaptureStatus,
            insert.PayloadHash,
            insert.StartedAt,
            insert.FinishedAt,
            Now = DateTime.UtcNow,
            insert.UserId
        }, transaction);
    }

    public Task InsertCaptureLineAsync(PhysicalInventoryCaptureLineInsert insert, IDbTransaction transaction)
    {
        var sql = $"""
            INSERT INTO {_line}
                ({Q("PhysicalInventoryCaptureLineGuid")},{Q("PhysicalInventoryCaptureId")},{Q("Barcode")},{Q("CountedQuantity")},
                 {Q("ScanCount")},{Q("Source")},{Q("InventoryItemId")},{Q("ExpectedQuantity")},{Q("Variance")},{Q("ValidationResult")},
                 {Q("FirstCapturedAt")},{Q("LastCapturedAt")},{Q("CreatedById")},{Q("CreatedTime")})
            VALUES
                (@LineGuid,@CaptureId,@Barcode,@Quantity,@ScanCount,@Source,@InventoryItemId,@ExpectedQuantity,@Variance,
                 @ValidationResult,@FirstCapturedAt,@LastCapturedAt,@UserId,@Now)
            """;
        return transaction.Connection!.ExecuteAsync(sql, new
        {
            insert.LineGuid,
            insert.CaptureId,
            insert.Barcode,
            insert.Quantity,
            insert.ScanCount,
            insert.Source,
            insert.InventoryItemId,
            insert.ExpectedQuantity,
            insert.Variance,
            insert.ValidationResult,
            insert.FirstCapturedAt,
            insert.LastCapturedAt,
            insert.UserId,
            Now = DateTime.UtcNow
        }, transaction);
    }

    public async Task<IReadOnlyList<PhysicalInventoryCaptureDto>> GetQueueAsync(long companyId)
    {
        using var connection = context.CreateConnection();
        var sql = $"""
            SELECT {Q("PhysicalInventoryCaptureGuid")} AS PhysicalInventoryCaptureGuid,
                   {Q("DocumentNumber")} AS DocumentNumber,
                   {Q("PageNumber")} AS PageNumber,
                   {Q("CompanyId")} AS CompanyId,
                   {Q("CompanyOfficeId")} AS CompanyOfficeId,
                   {Q("DeviceGuid")} AS DeviceGuid,
                   {Q("CounterUserId")} AS CounterUserId,
                   {Q("CounterUserGuid")} AS CounterUserGuid,
                   {Q("CaptureStatus")} AS CaptureStatus,
                   {Q("PayloadHash")} AS PayloadHash,
                   {Q("ConcurrencyToken")} AS ConcurrencyToken,
                   {Q("StartedAt")} AS StartedAt,
                   {Q("FinishedAt")} AS FinishedAt,
                   {Q("UploadedAt")} AS UploadedAt,
                   {Q("VerifiedById")} AS VerifiedById,
                   {Q("VerifiedAt")} AS VerifiedAt,
                   {Q("VerificationNote")} AS VerificationNote
            FROM {_capture}
            WHERE {Q("CompanyId")}=@companyId AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            ORDER BY {Q("UploadedAt")} DESC
            """;
        var rows = await connection.QueryAsync<PhysicalInventoryCaptureDto>(sql, new { companyId });
        return rows.ToList();
    }

    public async Task<IReadOnlyList<PhysicalInventoryCaptureLineDto>> GetLinesAsync(long captureId)
    {
        using var connection = context.CreateConnection();
        var sql = $"""
            SELECT l.{Q("PhysicalInventoryCaptureLineGuid")} AS PhysicalInventoryCaptureLineGuid,
                   l.{Q("Barcode")} AS Barcode,
                   l.{Q("CountedQuantity")} AS CountedQuantity,
                   l.{Q("ScanCount")} AS ScanCount,
                   l.{Q("Source")} AS Source,
                   i.{Q("ItemCode")} AS ItemCode,
                   i.{Q("ItemName")} AS ItemName,
                   l.{Q("ExpectedQuantity")} AS ExpectedQuantity,
                   l.{Q("Variance")} AS Variance,
                   l.{Q("ValidationResult")} AS ValidationResult
            FROM {_line} l
            LEFT JOIN {_item} i ON i.{Q("InventoryItemId")} = l.{Q("InventoryItemId")}
            WHERE l.{Q("PhysicalInventoryCaptureId")}=@captureId
              AND l.{Q("StatusId")}>0 AND l.{Q("DeletedTime")} IS NULL
            ORDER BY l.{Q("Barcode")}
            """;
        var rows = await connection.QueryAsync<PhysicalInventoryCaptureLineDto>(sql, new { captureId });
        return rows.ToList();
    }

    public async Task<bool> TryVerifyAsync(
        Guid captureGuid, long companyId, long verifierUserId, string status, string? note, long expectedToken, IDbTransaction transaction)
    {
        var sql = $"""
            UPDATE {_capture}
            SET {Q("CaptureStatus")}=@status,
                {Q("VerifiedById")}=@verifierUserId,
                {Q("VerifiedAt")}=@Now,
                {Q("VerificationNote")}=@note,
                {Q("ConcurrencyToken")}={Q("ConcurrencyToken")} + 1,
                {Q("UpdatedById")}=@verifierUserId,
                {Q("UpdatedTime")}=@Now
            WHERE {Q("PhysicalInventoryCaptureGuid")}=@captureGuid
              AND {Q("CompanyId")}=@companyId
              AND {Q("ConcurrencyToken")}=@expectedToken
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        var affected = await transaction.Connection!.ExecuteAsync(sql, new
        {
            status,
            verifierUserId,
            note,
            captureGuid,
            companyId,
            expectedToken,
            Now = DateTime.UtcNow
        }, transaction);
        return affected == 1;
    }
}
