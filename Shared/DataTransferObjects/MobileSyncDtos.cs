using System.Text.Json;

namespace Shared.DataTransferObjects;

public static class MobileSyncV1
{
    public const int ApiVersion = 1;
    public const string AttendanceEntity = "Attendance";
    public const string CheckInOperation = "CheckIn";
    public const string CheckOutOperation = "CheckOut";
}

public sealed record MobileSyncBatchV1
{
    public int Version { get; init; } = MobileSyncV1.ApiVersion;
    public required Guid BatchGuid { get; init; }
    public required Guid DeviceGuid { get; init; }
    public required long CompanyId { get; init; }
    public DateTimeOffset ClientCreatedAt { get; init; }
    public IReadOnlyList<MobileSyncOperationV1> Operations { get; init; } = [];
}

public sealed record MobileSyncOperationV1
{
    public required Guid OperationGuid { get; init; }
    public required string EntityType { get; init; }
    public required string OperationType { get; init; }
    public required Guid EntityGuid { get; init; }
    public long? BaseVersion { get; init; }
    public required JsonElement Payload { get; init; }
}

public sealed record AttendanceSyncEventV1
{
    public required long PersonId { get; init; }
    public required long CompanyOfficeId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public string Channel { get; init; } = "Mobile";
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public int? AccuracyMeters { get; init; }
    public string? Note { get; init; }
    public Guid? AttachmentGuid { get; init; }
}

public enum MobileSyncOutcome
{
    Applied,
    Duplicate,
    Conflict,
    Rejected,
    Retry
}

public sealed record MobileSyncReceiptV1
{
    public required Guid OperationGuid { get; init; }
    public required Guid EntityGuid { get; init; }
    public required MobileSyncOutcome Outcome { get; init; }
    public long? ServerVersion { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
    public JsonElement? ServerState { get; init; }
    public DateTimeOffset ProcessedAt { get; init; }
}

public sealed record MobileSyncBatchResultV1
{
    public int Version { get; init; } = MobileSyncV1.ApiVersion;
    public required Guid BatchGuid { get; init; }
    public required IReadOnlyList<MobileSyncReceiptV1> Receipts { get; init; }
    public DateTimeOffset ServerTime { get; init; } = DateTimeOffset.UtcNow;
}
