using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects;

public static class PhysicalInventoryApps
{
    public const string AppCode = "MOB.PHYSICAL_INVENTORY_COUNT";
    public const string RoleCounter = "InventoryCounter";
    public const string RoleVerifier = "InventoryVerifier";
    public const int PayloadVersion = 1;
    public const int PermitDays = 7;
    public const int ReceiptRetentionDays = 30;
}

public static class PhysicalInventoryStatuses
{
    public const string InProgress = "InProgress";
    public const string PendingUpload = "PendingUpload";
    public const string Uploading = "Uploading";
    public const string UploadedPendingVerification = "UploadedPendingVerification";
    public const string DocumentMatchRequired = "DocumentMatchRequired";
    public const string DuplicatePageReview = "DuplicatePageReview";
    public const string Verified = "Verified";
    public const string Rejected = "Rejected";
    public const string Conflict = "Conflict";
}

public sealed record OfflineAccessPermitDto
{
    public int Version { get; init; } = 1;
    public required Guid DeviceGuid { get; init; }
    public required Guid UserGuid { get; init; }
    public required long UserId { get; init; }
    public required long CompanyId { get; init; }
    public long? OfficeId { get; init; }
    public Guid? CompanyGuid { get; init; }
    public Guid? OfficeGuid { get; init; }
    public required string AppCode { get; init; }
    public bool CanCapture { get; init; }
    public bool CanVerify { get; init; }
    public int PermissionVersion { get; init; }
    public DateTimeOffset IssuedAt { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset ServerTime { get; init; }
    public string? Signature { get; init; }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;
}

public sealed record OfflinePermitIssueRequest
{
    public required Guid DeviceGuid { get; init; }
}

public sealed record PhysicalInventoryCountLineUploadDto
{
    public required Guid LineGuid { get; init; }
    public required string Barcode { get; init; }
    public required decimal Quantity { get; init; }
    public int ScanCount { get; init; } = 1;
    public DateTimeOffset FirstCapturedAt { get; init; }
    public DateTimeOffset LastCapturedAt { get; init; }
    public string Source { get; init; } = "Manual";
}

public sealed record PhysicalInventoryCountUploadDto
{
    public int Version { get; init; } = PhysicalInventoryApps.PayloadVersion;
    public required Guid OperationId { get; init; }
    public required Guid CaptureGuid { get; init; }
    public required Guid DeviceGuid { get; init; }
    public required string DocumentNumber { get; init; }
    public required int PageNumber { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset FinishedAt { get; init; }
    public required string PayloadHash { get; init; }
    public IReadOnlyList<PhysicalInventoryCountLineUploadDto> Lines { get; init; } = [];
}

public sealed record PhysicalInventoryCountReceiptDto
{
    public required Guid OperationId { get; init; }
    public required Guid CaptureGuid { get; init; }
    public required string Status { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
    public long? ConcurrencyToken { get; init; }
    public string? PayloadHash { get; init; }
    public DateTimeOffset CommittedAt { get; init; }
}

public sealed record PhysicalInventoryCaptureLineDto
{
    public Guid PhysicalInventoryCaptureLineGuid { get; init; }
    public string Barcode { get; init; } = string.Empty;
    public decimal CountedQuantity { get; init; }
    public int ScanCount { get; init; }
    public string Source { get; init; } = "Manual";
    public string? ItemCode { get; init; }
    public string? ItemName { get; init; }
    public decimal? ExpectedQuantity { get; init; }
    public decimal? Variance { get; init; }
    public string ValidationResult { get; init; } = "Unresolved";
}

public sealed record PhysicalInventoryCaptureDto
{
    public Guid PhysicalInventoryCaptureGuid { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public int PageNumber { get; init; }
    public long CompanyId { get; init; }
    public long? CompanyOfficeId { get; init; }
    public Guid DeviceGuid { get; init; }
    public long CounterUserId { get; init; }
    public Guid CounterUserGuid { get; init; }
    public string? CounterUserName { get; init; }
    public string CaptureStatus { get; init; } = string.Empty;
    public string PayloadHash { get; init; } = string.Empty;
    public long ConcurrencyToken { get; init; }
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset FinishedAt { get; init; }
    public DateTimeOffset UploadedAt { get; init; }
    public long? VerifiedById { get; init; }
    public DateTimeOffset? VerifiedAt { get; init; }
    public string? VerificationNote { get; init; }
    public IReadOnlyList<PhysicalInventoryCaptureLineDto> Lines { get; init; } = [];
}

public sealed record PhysicalInventoryVerifyRequestDto
{
    public required long ExpectedConcurrencyToken { get; init; }
    public string? Note { get; init; }
}

public static class PhysicalInventoryPayloadHasher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Compute(PhysicalInventoryCountUploadDto upload)
    {
        var canonical = new
        {
            upload.CaptureGuid,
            DocumentNumber = upload.DocumentNumber.Trim(),
            upload.PageNumber,
            Lines = upload.Lines
                .OrderBy(x => x.Barcode, StringComparer.OrdinalIgnoreCase)
                .Select(x => new { Barcode = x.Barcode.Trim(), Quantity = x.Quantity, x.ScanCount, x.Source })
                .ToArray()
        };
        var json = JsonSerializer.Serialize(canonical, JsonOptions);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    }
}

public static class PhysicalInventoryLocalRules
{
    public static bool TryValidateHeader(string? documentNumber, int pageNumber, out string error)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            error = "Stock Count Document Number is required.";
            return false;
        }

        if (pageNumber <= 0)
        {
            error = "Page Number must be a positive integer.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool TryValidateLine(string? barcode, decimal quantity, out string error)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            error = "Barcode is required.";
            return false;
        }

        if (quantity <= 0)
        {
            error = "Quantity must be greater than zero.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static string NormalizeBarcode(string barcode) => barcode.Trim().ToUpperInvariant();

    public static decimal Accumulate(decimal existing, decimal added) => existing + added;
}
