using Shared.DataTransferObjects;
using System.Data;

namespace Contracts;

public interface IPhysicalInventoryRepository
{
    Task UpsertPermitAsync(OfflineAccessPermitDto permit, string permitJson, IDbTransaction transaction);
    Task<PhysicalInventoryCaptureRecord?> GetCaptureByOperationAsync(Guid operationGuid, long companyId, IDbTransaction? transaction = null);
    Task<PhysicalInventoryCaptureRecord?> GetCaptureByGuidAsync(Guid captureGuid, long companyId, IDbTransaction? transaction = null);
    Task<bool> OtherCaptureExistsAsync(string documentNumber, int pageNumber, long companyId, Guid captureGuid, IDbTransaction transaction);
    Task<(long? DocumentId, long? PageId)> ResolveDocumentPageAsync(string documentNumber, int pageNumber, long companyId, IDbTransaction transaction);
    Task<InventoryItemMatch?> FindItemByBarcodeAsync(string barcode, long companyId, IDbTransaction transaction);
    Task<decimal?> GetExpectedQuantityAsync(long pageId, long inventoryItemId, IDbTransaction transaction);
    Task<long> InsertCaptureAsync(PhysicalInventoryCaptureInsert insert, IDbTransaction transaction);
    Task InsertCaptureLineAsync(PhysicalInventoryCaptureLineInsert insert, IDbTransaction transaction);
    Task<IReadOnlyList<PhysicalInventoryCaptureDto>> GetQueueAsync(long companyId);
    Task<IReadOnlyList<PhysicalInventoryCaptureLineDto>> GetLinesAsync(long captureId);
    Task<bool> TryVerifyAsync(Guid captureGuid, long companyId, long verifierUserId, string status, string? note, long expectedToken, IDbTransaction transaction);
}

public sealed record InventoryItemMatch(long InventoryItemId, string ItemCode, string ItemName);

public sealed record PhysicalInventoryCaptureRecord(
    long PhysicalInventoryCaptureId,
    Guid PhysicalInventoryCaptureGuid,
    Guid OperationGuid,
    string PayloadHash,
    string CaptureStatus,
    long ConcurrencyToken,
    long CounterUserId);

public sealed record PhysicalInventoryCaptureInsert(
    Guid CaptureGuid,
    Guid OperationGuid,
    long CompanyId,
    long? OfficeId,
    Guid DeviceGuid,
    string DocumentNumber,
    int PageNumber,
    long? DocumentId,
    long? PageId,
    long CounterUserId,
    Guid CounterUserGuid,
    string CaptureStatus,
    string PayloadHash,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    long UserId);

public sealed record PhysicalInventoryCaptureLineInsert(
    Guid LineGuid,
    long CaptureId,
    string Barcode,
    decimal Quantity,
    int ScanCount,
    string Source,
    long? InventoryItemId,
    decimal? ExpectedQuantity,
    decimal? Variance,
    string ValidationResult,
    DateTimeOffset FirstCapturedAt,
    DateTimeOffset LastCapturedAt,
    long UserId);
