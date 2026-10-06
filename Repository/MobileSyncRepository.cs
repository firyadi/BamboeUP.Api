using Contracts;
using Dapper;
using Repository.Database;
using Shared.DataTransferObjects;
using Shared.Settings;
using System.Data;
using System.Text.Json;

namespace Repository;

public sealed class MobileSyncRepository(RepositoryContext context, ISqlDialect dialect) : IMobileSyncRepository
{
    private readonly string _device = dialect.Qualify("sync", "MobileDevice");
    private readonly string _batch = dialect.Qualify("sync", "MobileSyncBatch");
    private readonly string _operation = dialect.Qualify("sync", "MobileSyncOperation");
    private readonly string _attendance = dialect.Qualify("app", "Attendance");
    private readonly string _employee = dialect.Qualify("app", "Employee");
    private readonly string _assignment = dialect.Qualify("app", "EmployeeAssignment");
    private readonly string _office = dialect.Qualify("app", "CompanyOffice");
    private readonly string _fileStorage = dialect.Qualify("app", "FileStorage");

    private string Q(string identifier) =>
        dialect.Provider == DatabaseProvider.PostgreSql ? $"\"{identifier}\"" : $"[{identifier}]";

    public async Task<MobileSyncBatchResultV1?> GetCompletedBatchAsync(Guid batchGuid, long companyId, long userId)
    {
        using var connection = context.CreateConnection();
        var existsSql = $"""
            SELECT COUNT(1) FROM {_batch}
            WHERE {Q("BatchGuid")} = @batchGuid AND {Q("CompanyId")} = @companyId
              AND {Q("UserId")} = @userId AND {Q("Status")} = 'Completed'
            """;
        if (await connection.ExecuteScalarAsync<int>(existsSql, new { batchGuid, companyId, userId }) == 0)
            return null;

        var receipts = await ReadReceiptsAsync(connection, batchGuid, companyId, userId, null);
        return new MobileSyncBatchResultV1 { BatchGuid = batchGuid, Receipts = receipts };
    }

    public async Task RegisterDeviceAndBatchAsync(MobileSyncBatchV1 batch, long userId, IDbTransaction transaction)
    {
        var connection = transaction.Connection!;
        string deviceSql;
        string batchSql;
        if (dialect.Provider == DatabaseProvider.PostgreSql)
        {
            deviceSql = $"""
                INSERT INTO {_device} ("DeviceGuid", "CompanyId", "UserId", "LastSeenAt", "CreatedTime")
                VALUES (@DeviceGuid, @CompanyId, @UserId, @Now, @Now)
                ON CONFLICT ("DeviceGuid", "CompanyId", "UserId")
                DO UPDATE SET "LastSeenAt" = EXCLUDED."LastSeenAt";
                """;
            batchSql = $"""
                INSERT INTO {_batch} ("BatchGuid", "DeviceGuid", "CompanyId", "UserId", "ClientCreatedAt", "Status", "CreatedTime")
                VALUES (@BatchGuid, @DeviceGuid, @CompanyId, @UserId, @ClientCreatedAt, 'Processing', @Now)
                ON CONFLICT ("BatchGuid", "CompanyId", "UserId") DO NOTHING;
                """;
        }
        else
        {
            deviceSql = $"""
                IF EXISTS (SELECT 1 FROM {_device} WHERE [DeviceGuid]=@DeviceGuid AND [CompanyId]=@CompanyId AND [UserId]=@UserId)
                    UPDATE {_device} SET [LastSeenAt]=@Now WHERE [DeviceGuid]=@DeviceGuid AND [CompanyId]=@CompanyId AND [UserId]=@UserId;
                ELSE
                    INSERT INTO {_device} ([DeviceGuid],[CompanyId],[UserId],[LastSeenAt],[CreatedTime])
                    VALUES (@DeviceGuid,@CompanyId,@UserId,@Now,@Now);
                """;
            batchSql = $"""
                IF NOT EXISTS (SELECT 1 FROM {_batch} WHERE [BatchGuid]=@BatchGuid AND [CompanyId]=@CompanyId AND [UserId]=@UserId)
                    INSERT INTO {_batch} ([BatchGuid],[DeviceGuid],[CompanyId],[UserId],[ClientCreatedAt],[Status],[CreatedTime])
                    VALUES (@BatchGuid,@DeviceGuid,@CompanyId,@UserId,@ClientCreatedAt,'Processing',@Now);
                """;
        }

        var args = new
        {
            batch.BatchGuid,
            batch.DeviceGuid,
            batch.CompanyId,
            UserId = userId,
            batch.ClientCreatedAt,
            Now = DateTime.UtcNow
        };
        await connection.ExecuteAsync(deviceSql, args, transaction);
        await connection.ExecuteAsync(batchSql, args, transaction);
    }

    public async Task<MobileSyncReceiptV1?> GetReceiptAsync(
        Guid operationGuid, long companyId, long userId, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT {Q("ReceiptJson")} FROM {_operation}
            WHERE {Q("OperationGuid")}=@operationGuid AND {Q("CompanyId")}=@companyId
              AND {Q("UserId")}=@userId AND {Q("ReceiptJson")} IS NOT NULL
            """;
        var json = await transaction.Connection!.QuerySingleOrDefaultAsync<string>(
            sql, new { operationGuid, companyId, userId }, transaction);
        return json is null ? null : JsonSerializer.Deserialize<MobileSyncReceiptV1>(json);
    }

    public async Task<bool> TryReserveOperationAsync(
        Guid batchGuid, MobileSyncOperationV1 operation, long companyId, long userId, IDbTransaction transaction)
    {
        string sql;
        if (dialect.Provider == DatabaseProvider.PostgreSql)
        {
            sql = $"""
                INSERT INTO {_operation}
                    ("OperationGuid","BatchGuid","CompanyId","UserId","EntityType","OperationType","EntityGuid","BaseVersion","Status","CreatedTime")
                VALUES
                    (@OperationGuid,@BatchGuid,@CompanyId,@UserId,@EntityType,@OperationType,@EntityGuid,@BaseVersion,'Processing',@Now)
                ON CONFLICT ("OperationGuid","CompanyId","UserId") DO NOTHING;
                """;
        }
        else
        {
            sql = $"""
                IF NOT EXISTS (SELECT 1 FROM {_operation} WITH (UPDLOCK, HOLDLOCK)
                    WHERE [OperationGuid]=@OperationGuid AND [CompanyId]=@CompanyId AND [UserId]=@UserId)
                BEGIN
                    INSERT INTO {_operation}
                        ([OperationGuid],[BatchGuid],[CompanyId],[UserId],[EntityType],[OperationType],[EntityGuid],[BaseVersion],[Status],[CreatedTime])
                    VALUES
                        (@OperationGuid,@BatchGuid,@CompanyId,@UserId,@EntityType,@OperationType,@EntityGuid,@BaseVersion,'Processing',@Now);
                END
                """;
        }

        await transaction.Connection!.ExecuteAsync(sql, new
        {
            operation.OperationGuid,
            BatchGuid = batchGuid,
            CompanyId = companyId,
            UserId = userId,
            operation.EntityType,
            operation.OperationType,
            operation.EntityGuid,
            operation.BaseVersion,
            Now = DateTime.UtcNow
        }, transaction);

        var countSql = $"""
            SELECT COUNT(1) FROM {_operation}
            WHERE {Q("OperationGuid")}=@OperationGuid AND {Q("CompanyId")}=@CompanyId
              AND {Q("UserId")}=@UserId AND {Q("BatchGuid")}=@BatchGuid AND {Q("ReceiptJson")} IS NULL
            """;
        return await transaction.Connection!.ExecuteScalarAsync<int>(
            countSql,
            new { operation.OperationGuid, BatchGuid = batchGuid, CompanyId = companyId, UserId = userId },
            transaction) == 1;
    }

    public async Task<bool> PersonBelongsToTenantAsync(long personId, long companyId, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT COUNT(1)
            FROM {_employee} e
            INNER JOIN {_assignment} a ON e.{Q("EmployeeId")} = a.{Q("EmployeeId")}
            WHERE e.{Q("PersonId")}=@personId AND a.{Q("CompanyId")}=@companyId
              AND e.{Q("StatusId")}>0 AND e.{Q("DeletedTime")} IS NULL
              AND a.{Q("StatusId")}>0 AND a.{Q("DeletedTime")} IS NULL
            """;
        return await transaction.Connection!.ExecuteScalarAsync<int>(
            sql, new { personId, companyId }, transaction) > 0;
    }

    public async Task<bool> OfficeBelongsToTenantAsync(long officeId, long companyId, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT COUNT(1) FROM {_office}
            WHERE {Q("CompanyOfficeId")}=@officeId AND {Q("CompanyId")}=@companyId
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return await transaction.Connection!.ExecuteScalarAsync<int>(
            sql, new { officeId, companyId }, transaction) > 0;
    }

    public Task<long?> ResolveAttachmentAsync(Guid attachmentGuid, IDbTransaction transaction)
    {
        var sql = $"""
            SELECT {Q("FileStorageId")} FROM {_fileStorage}
            WHERE {Q("FileStorageGuid")}=@attachmentGuid AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        return transaction.Connection!.QuerySingleOrDefaultAsync<long?>(sql, new { attachmentGuid }, transaction);
    }

    public Task<AttendanceSyncState?> GetAttendanceAsync(
        Guid entityGuid, long companyId, IDbTransaction transaction, bool forUpdate)
    {
        var sql = $"""
            SELECT {Q("AttendanceGuid")} AS AttendanceGuid, {Q("CompanyId")} AS CompanyId,
                   {Q("PersonId")} AS PersonId, {Q("AttendanceDate")} AS AttendanceDate,
                   {Q("CheckInTime")} AS CheckInTime, {Q("CheckOutTime")} AS CheckOutTime,
                   {Q("SyncVersion")} AS SyncVersion
            FROM {_attendance}{(forUpdate ? dialect.TableLockHint : string.Empty)}
            WHERE {Q("AttendanceGuid")}=@entityGuid AND {Q("CompanyId")}=@companyId
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            {(forUpdate ? dialect.ForUpdateSuffix : string.Empty)}
            """;
        return transaction.Connection!.QuerySingleOrDefaultAsync<AttendanceSyncState>(
            sql, new { entityGuid, companyId }, transaction);
    }

    public Task<AttendanceSyncState?> GetAttendanceByPersonDateAsync(
        long personId, long companyId, DateOnly attendanceDate, IDbTransaction transaction, bool forUpdate)
    {
        var sql = $"""
            SELECT {Q("AttendanceGuid")} AS AttendanceGuid, {Q("CompanyId")} AS CompanyId,
                   {Q("PersonId")} AS PersonId, {Q("AttendanceDate")} AS AttendanceDate,
                   {Q("CheckInTime")} AS CheckInTime, {Q("CheckOutTime")} AS CheckOutTime,
                   {Q("SyncVersion")} AS SyncVersion
            FROM {_attendance}{(forUpdate ? dialect.TableLockHint : string.Empty)}
            WHERE {Q("PersonId")}=@personId AND {Q("CompanyId")}=@companyId
              AND {Q("AttendanceDate")}=@attendanceDate
              AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            {(forUpdate ? dialect.ForUpdateSuffix : string.Empty)}
            """;
        return transaction.Connection!.QuerySingleOrDefaultAsync<AttendanceSyncState>(
            sql, new { personId, companyId, attendanceDate }, transaction);
    }

    public async Task<long> InsertCheckInAsync(
        Guid entityGuid, long companyId, AttendanceSyncEventV1 input, long? attachmentId,
        long userId, IDbTransaction transaction)
    {
        var sql = $"""
            INSERT INTO {_attendance}
                ({Q("AttendanceGuid")},{Q("CompanyId")},{Q("CompanyOfficeId")},{Q("PersonId")},
                 {Q("AttendanceDate")},{Q("CheckInTime")},{Q("CheckInChannel")},{Q("CheckInFileStorageId")},
                 {Q("CheckInLatitude")},{Q("CheckInLongitude")},{Q("CheckInAccuracyMeters")},{Q("Note")},
                 {Q("SyncVersion")},{Q("StatusId")},{Q("CreatedById")},{Q("CreatedTime")})
            VALUES
                (@entityGuid,@companyId,@CompanyOfficeId,@PersonId,@AttendanceDate,@OccurredAt,@Channel,@attachmentId,
                 @Latitude,@Longitude,@AccuracyMeters,@Note,1,1,@userId,@Now)
            """;
        await transaction.Connection!.ExecuteAsync(sql, new
        {
            entityGuid,
            companyId,
            input.CompanyOfficeId,
            input.PersonId,
            AttendanceDate = input.OccurredAt.UtcDateTime.Date,
            OccurredAt = input.OccurredAt.UtcDateTime,
            input.Channel,
            attachmentId,
            input.Latitude,
            input.Longitude,
            input.AccuracyMeters,
            input.Note,
            userId,
            Now = DateTime.UtcNow
        }, transaction);
        return 1;
    }

    public async Task<long?> ApplyAttendanceEventAsync(
        Guid entityGuid, long companyId, AttendanceSyncEventV1 input, long? attachmentId,
        long expectedVersion, bool checkIn, long userId, IDbTransaction transaction)
    {
        var prefix = checkIn ? "CheckIn" : "CheckOut";
        var sql = $"""
            UPDATE {_attendance}
            SET {Q(prefix + "Time")}=@OccurredAt,
                {Q(prefix + "Channel")}=@Channel,
                {Q(prefix + "FileStorageId")}=@attachmentId,
                {Q(prefix + "Latitude")}=@Latitude,
                {Q(prefix + "Longitude")}=@Longitude,
                {Q(prefix + "AccuracyMeters")}=@AccuracyMeters,
                {Q("Note")}=COALESCE(@Note,{Q("Note")}),
                {Q("SyncVersion")}={Q("SyncVersion")}+1,
                {Q("UpdatedById")}=@userId,
                {Q("UpdatedTime")}=@Now
            WHERE {Q("AttendanceGuid")}=@entityGuid AND {Q("CompanyId")}=@companyId
              AND {Q("SyncVersion")}=@expectedVersion AND {Q("StatusId")}>0 AND {Q("DeletedTime")} IS NULL
            """;
        var affected = await transaction.Connection!.ExecuteAsync(sql, new
        {
            entityGuid,
            companyId,
            OccurredAt = input.OccurredAt.UtcDateTime,
            input.Channel,
            attachmentId,
            input.Latitude,
            input.Longitude,
            input.AccuracyMeters,
            input.Note,
            userId,
            expectedVersion,
            Now = DateTime.UtcNow
        }, transaction);
        return affected == 1 ? expectedVersion + 1 : null;
    }

    public Task SaveReceiptAsync(
        Guid operationGuid, MobileSyncReceiptV1 receipt, long companyId, long userId, IDbTransaction transaction)
    {
        var receiptValue = dialect.Provider == DatabaseProvider.PostgreSql
            ? $"CAST(@ReceiptJson AS jsonb)"
            : "@ReceiptJson";
        var sql = $"""
            UPDATE {_operation}
            SET {Q("Status")}=@Status, {Q("ReceiptJson")}={receiptValue}, {Q("ProcessedTime")}=@ProcessedTime
            WHERE {Q("OperationGuid")}=@operationGuid AND {Q("CompanyId")}=@companyId AND {Q("UserId")}=@userId
            """;
        return transaction.Connection!.ExecuteAsync(sql, new
        {
            operationGuid,
            companyId,
            userId,
            Status = receipt.Outcome.ToString(),
            ReceiptJson = JsonSerializer.Serialize(receipt),
            ProcessedTime = receipt.ProcessedAt.UtcDateTime
        }, transaction);
    }

    public Task CompleteBatchAsync(Guid batchGuid, long companyId, long userId, IDbTransaction transaction)
    {
        var sql = $"""
            UPDATE {_batch} SET {Q("Status")}='Completed', {Q("CompletedTime")}=@now
            WHERE {Q("BatchGuid")}=@batchGuid AND {Q("CompanyId")}=@companyId AND {Q("UserId")}=@userId
            """;
        return transaction.Connection!.ExecuteAsync(
            sql, new { batchGuid, companyId, userId, now = DateTime.UtcNow }, transaction);
    }

    private async Task<IReadOnlyList<MobileSyncReceiptV1>> ReadReceiptsAsync(
        IDbConnection connection, Guid batchGuid, long companyId, long userId, IDbTransaction? transaction)
    {
        var sql = $"""
            SELECT {Q("ReceiptJson")} FROM {_operation}
            WHERE {Q("BatchGuid")}=@batchGuid AND {Q("CompanyId")}=@companyId
              AND {Q("UserId")}=@userId AND {Q("ReceiptJson")} IS NOT NULL
            ORDER BY {Q("MobileSyncOperationId")}
            """;
        var json = await connection.QueryAsync<string>(sql, new { batchGuid, companyId, userId }, transaction);
        return json.Select(value => JsonSerializer.Deserialize<MobileSyncReceiptV1>(value)!).ToArray();
    }
}
