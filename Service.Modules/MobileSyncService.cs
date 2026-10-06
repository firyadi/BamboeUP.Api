using Contracts;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System.Text.Json;

namespace Service.Modules;

public sealed class MobileSyncService(
    IMobileSyncRepository repository,
    ITransactionManager transactionManager,
    IUserContext userContext) : IMobileSyncService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const int MaxOperations = 200;

    public async Task<MobileSyncBatchResultV1> PushAsync(MobileSyncBatchV1 batch)
    {
        ValidateEnvelope(batch);

        var replay = await repository.GetCompletedBatchAsync(batch.BatchGuid, batch.CompanyId, userContext.UserId);
        if (replay is not null)
            return replay;

        await transactionManager.BeginTransactionAsync();
        var transaction = transactionManager.GetTransaction();
        try
        {
            await repository.RegisterDeviceAndBatchAsync(batch, userContext.UserId, transaction);
            var receipts = new List<MobileSyncReceiptV1>(batch.Operations.Count);

            foreach (var operation in batch.Operations)
            {
                var existing = await repository.GetReceiptAsync(
                    operation.OperationGuid, batch.CompanyId, userContext.UserId, transaction);
                if (existing is not null)
                {
                    receipts.Add(existing);
                    continue;
                }

                var reserved = await repository.TryReserveOperationAsync(
                    batch.BatchGuid, operation, batch.CompanyId, userContext.UserId, transaction);
                if (!reserved)
                {
                    receipts.Add(Retry(operation, "OPERATION_IN_PROGRESS",
                        "The operation is currently being processed by another request."));
                    continue;
                }

                var receipt = await HandleOperationAsync(operation, batch.CompanyId, transaction);
                await repository.SaveReceiptAsync(
                    operation.OperationGuid, receipt, batch.CompanyId, userContext.UserId, transaction);
                receipts.Add(receipt);
            }

            await repository.CompleteBatchAsync(batch.BatchGuid, batch.CompanyId, userContext.UserId, transaction);
            await transactionManager.CommitAsync();
            return new MobileSyncBatchResultV1 { BatchGuid = batch.BatchGuid, Receipts = receipts };
        }
        catch
        {
            await transactionManager.RollbackAsync();
            throw;
        }
    }

    private void ValidateEnvelope(MobileSyncBatchV1 batch)
    {
        if (!userContext.IsAuthenticated || userContext.UserId <= 0)
            throw new UnauthorizedAccessException("An authenticated user is required.");
        if (batch.Version != MobileSyncV1.ApiVersion)
            throw new ArgumentException($"Unsupported sync version '{batch.Version}'.");
        if (batch.BatchGuid == Guid.Empty || batch.DeviceGuid == Guid.Empty)
            throw new ArgumentException("BatchGuid and DeviceGuid are required.");
        if (userContext.CompanyId is null || userContext.CompanyId.Value != batch.CompanyId)
            throw new UnauthorizedAccessException("The sync tenant does not match the active user tenant.");
        if (batch.Operations.Count is < 1 or > MaxOperations)
            throw new ArgumentException($"A batch must contain between 1 and {MaxOperations} operations.");
        if (batch.Operations.Any(x => x.OperationGuid == Guid.Empty || x.EntityGuid == Guid.Empty))
            throw new ArgumentException("Every operation requires OperationGuid and EntityGuid.");
        if (batch.Operations.Select(x => x.OperationGuid).Distinct().Count() != batch.Operations.Count)
            throw new ArgumentException("OperationGuid values must be unique within a batch.");
    }

    private async Task<MobileSyncReceiptV1> HandleOperationAsync(
        MobileSyncOperationV1 operation, long companyId, System.Data.IDbTransaction transaction)
    {
        if (!string.Equals(operation.EntityType, MobileSyncV1.AttendanceEntity, StringComparison.OrdinalIgnoreCase))
            return Rejected(operation, "ENTITY_NOT_SUPPORTED", $"Entity '{operation.EntityType}' is not sync-enabled.");

        var checkIn = string.Equals(operation.OperationType, MobileSyncV1.CheckInOperation, StringComparison.OrdinalIgnoreCase);
        var checkOut = string.Equals(operation.OperationType, MobileSyncV1.CheckOutOperation, StringComparison.OrdinalIgnoreCase);
        if (!checkIn && !checkOut)
            return Rejected(operation, "OPERATION_NOT_SUPPORTED", $"Operation '{operation.OperationType}' is not supported.");

        AttendanceSyncEventV1? input;
        try
        {
            input = operation.Payload.Deserialize<AttendanceSyncEventV1>(JsonOptions);
        }
        catch (JsonException ex)
        {
            return Rejected(operation, "INVALID_PAYLOAD", ex.Message);
        }

        if (input is null || input.PersonId <= 0 || input.CompanyOfficeId <= 0 || input.OccurredAt == default)
            return Rejected(operation, "INVALID_PAYLOAD", "PersonId, CompanyOfficeId and OccurredAt are required.");
        if (input.Latitude is < -90 or > 90 || input.Longitude is < -180 or > 180)
            return Rejected(operation, "INVALID_LOCATION", "Latitude or longitude is outside its valid range.");
        if (!await repository.PersonBelongsToTenantAsync(input.PersonId, companyId, transaction))
            return Rejected(operation, "PERSON_OUTSIDE_TENANT", "The person is not active in the requested tenant.");
        if (!await repository.OfficeBelongsToTenantAsync(input.CompanyOfficeId, companyId, transaction))
            return Rejected(operation, "OFFICE_OUTSIDE_TENANT", "The office is not active in the requested tenant.");

        long? attachmentId = null;
        if (input.AttachmentGuid.HasValue)
        {
            attachmentId = await repository.ResolveAttachmentAsync(input.AttachmentGuid.Value, transaction);
            if (!attachmentId.HasValue)
                return Retry(operation, "ATTACHMENT_NOT_READY",
                    "Upload the attachment with the same AttachmentGuid before replaying this operation.");
        }

        var state = await repository.GetAttendanceAsync(operation.EntityGuid, companyId, transaction, true);
        if (checkIn && state is null)
        {
            var byDate = await repository.GetAttendanceByPersonDateAsync(
                input.PersonId, companyId, DateOnly.FromDateTime(input.OccurredAt.UtcDateTime), transaction, true);
            if (byDate is not null)
                return Conflict(operation, byDate, "ATTENDANCE_ALREADY_EXISTS",
                    "An attendance row already exists for this person and date.");

            var version = await repository.InsertCheckInAsync(
                operation.EntityGuid, companyId, input, attachmentId, userContext.UserId, transaction);
            return Applied(operation, version);
        }

        if (state is null)
            return Conflict(operation, null, "ATTENDANCE_NOT_FOUND",
                "Check-out requires an existing attendance created with the same EntityGuid.");
        if (state.PersonId != input.PersonId)
            return Conflict(operation, state, "PERSON_MISMATCH", "EntityGuid belongs to a different person.");
        if (!operation.BaseVersion.HasValue)
            return Conflict(operation, state, "BASE_VERSION_REQUIRED", "BaseVersion is required for an existing attendance.");
        if (operation.BaseVersion.Value != state.SyncVersion)
            return Conflict(operation, state, "VERSION_CONFLICT", "The attendance changed after the client last read it.");
        if (checkIn && state.CheckInTime.HasValue)
            return Conflict(operation, state, "ALREADY_CHECKED_IN", "Check-in has already been recorded.");
        if (checkOut && !state.CheckInTime.HasValue)
            return Conflict(operation, state, "CHECK_IN_REQUIRED", "Check-in must be recorded before check-out.");
        if (checkOut && state.CheckOutTime.HasValue)
            return Conflict(operation, state, "ALREADY_CHECKED_OUT", "Check-out has already been recorded.");
        if (checkOut && input.OccurredAt.UtcDateTime < state.CheckInTime!.Value)
            return Conflict(operation, state, "CHECK_OUT_BEFORE_CHECK_IN", "Check-out cannot precede check-in.");

        var newVersion = await repository.ApplyAttendanceEventAsync(
            operation.EntityGuid, companyId, input, attachmentId, state.SyncVersion,
            checkIn, userContext.UserId, transaction);
        return newVersion.HasValue
            ? Applied(operation, newVersion.Value)
            : Conflict(operation,
                await repository.GetAttendanceAsync(operation.EntityGuid, companyId, transaction, false),
                "VERSION_CONFLICT", "The attendance was concurrently modified.");
    }

    private static MobileSyncReceiptV1 Applied(MobileSyncOperationV1 operation, long version) =>
        Receipt(operation, MobileSyncOutcome.Applied, version, null, null, null);

    private static MobileSyncReceiptV1 Rejected(MobileSyncOperationV1 operation, string code, string message) =>
        Receipt(operation, MobileSyncOutcome.Rejected, null, code, message, null);

    private static MobileSyncReceiptV1 Retry(MobileSyncOperationV1 operation, string code, string message) =>
        Receipt(operation, MobileSyncOutcome.Retry, null, code, message, null);

    private static MobileSyncReceiptV1 Conflict(
        MobileSyncOperationV1 operation, AttendanceSyncState? state, string code, string message) =>
        Receipt(operation, MobileSyncOutcome.Conflict, state?.SyncVersion, code, message,
            state is null ? null : JsonSerializer.SerializeToElement(state, JsonOptions));

    private static MobileSyncReceiptV1 Receipt(
        MobileSyncOperationV1 operation,
        MobileSyncOutcome outcome,
        long? version,
        string? code,
        string? message,
        JsonElement? state) =>
        new()
        {
            OperationGuid = operation.OperationGuid,
            EntityGuid = operation.EntityGuid,
            Outcome = outcome,
            ServerVersion = version,
            Code = code,
            Message = message,
            ServerState = state,
            ProcessedAt = DateTimeOffset.UtcNow
        };
}
