using Shared.DataTransferObjects;
using System.Data;

namespace Contracts;

public interface IMobileSyncRepository
{
    Task<MobileSyncBatchResultV1?> GetCompletedBatchAsync(Guid batchGuid, long companyId, long userId);
    Task RegisterDeviceAndBatchAsync(MobileSyncBatchV1 batch, long userId, IDbTransaction transaction);
    Task<MobileSyncReceiptV1?> GetReceiptAsync(Guid operationGuid, long companyId, long userId, IDbTransaction transaction);
    Task<bool> TryReserveOperationAsync(
        Guid batchGuid,
        MobileSyncOperationV1 operation,
        long companyId,
        long userId,
        IDbTransaction transaction);
    Task<bool> PersonBelongsToTenantAsync(long personId, long companyId, IDbTransaction transaction);
    Task<bool> OfficeBelongsToTenantAsync(long officeId, long companyId, IDbTransaction transaction);
    Task<long?> ResolveAttachmentAsync(Guid attachmentGuid, IDbTransaction transaction);
    Task<AttendanceSyncState?> GetAttendanceAsync(Guid entityGuid, long companyId, IDbTransaction transaction, bool forUpdate);
    Task<AttendanceSyncState?> GetAttendanceByPersonDateAsync(
        long personId,
        long companyId,
        DateOnly attendanceDate,
        IDbTransaction transaction,
        bool forUpdate);
    Task<long> InsertCheckInAsync(
        Guid entityGuid,
        long companyId,
        AttendanceSyncEventV1 input,
        long? attachmentId,
        long userId,
        IDbTransaction transaction);
    Task<long?> ApplyAttendanceEventAsync(
        Guid entityGuid,
        long companyId,
        AttendanceSyncEventV1 input,
        long? attachmentId,
        long expectedVersion,
        bool checkIn,
        long userId,
        IDbTransaction transaction);
    Task SaveReceiptAsync(
        Guid operationGuid,
        MobileSyncReceiptV1 receipt,
        long companyId,
        long userId,
        IDbTransaction transaction);
    Task CompleteBatchAsync(Guid batchGuid, long companyId, long userId, IDbTransaction transaction);
}

public sealed record AttendanceSyncState(
    Guid AttendanceGuid,
    long CompanyId,
    long PersonId,
    DateTime AttendanceDate,
    DateTime? CheckInTime,
    DateTime? CheckOutTime,
    long SyncVersion);
