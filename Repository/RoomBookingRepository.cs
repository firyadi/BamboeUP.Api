using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class RoomBookingRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IRoomBookingRepository
    {
        private readonly string _table = dialect.Qualify("app", "RoomBooking");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<RoomBooking?> GetRoomBookingAsync(Guid roomBookingGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_MeetingRoomId.MeetingRoomName AS MeetingRoomName
, j_BookedByUserId.BookedbyUserName AS BookedbyUserName

, j_BookedByUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "MeetingRoom")} j_MeetingRoomId ON a.MeetingRoomId = j_MeetingRoomId.MeetingRoomId
LEFT JOIN {dialect.Qualify("app", "Person")} j_BookedByUserId ON a.BookedByUserId = j_BookedByUserId.BookedbyUserId

                WHERE a.RoomBookingGuid = @roomBookingGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<RoomBooking>(sql, new { roomBookingGuid });
        }

        public async Task<RoomBooking?> GetByPersonGuidAndRoomBookingGuidAsync(Guid personGuid, Guid roomBookingGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_MeetingRoomId.MeetingRoomName AS MeetingRoomName
, j_BookedByUserId.BookedbyUserName AS BookedbyUserName

, j_BookedByUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "MeetingRoom")} j_MeetingRoomId ON a.MeetingRoomId = j_MeetingRoomId.MeetingRoomId
LEFT JOIN {dialect.Qualify("app", "Person")} j_BookedByUserId ON a.BookedByUserId = j_BookedByUserId.BookedbyUserId

                WHERE j_BookedByUserId.PersonGuid = @personGuid
                  AND a.RoomBookingGuid = @roomBookingGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<RoomBooking>(sql, new { personGuid, roomBookingGuid });
        }

        public async Task<IEnumerable<RoomBooking>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_MeetingRoomId.MeetingRoomName AS MeetingRoomName
, j_BookedByUserId.BookedbyUserName AS BookedbyUserName

, j_BookedByUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "MeetingRoom")} j_MeetingRoomId ON a.MeetingRoomId = j_MeetingRoomId.MeetingRoomId
LEFT JOIN {dialect.Qualify("app", "Person")} j_BookedByUserId ON a.BookedByUserId = j_BookedByUserId.BookedbyUserId

                WHERE j_BookedByUserId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.RoomBookingId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<RoomBooking>(sql, new { personGuid });
        }

        public async Task CreateRoomBookingAsync(RoomBooking roomBooking, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(roomBooking);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (RoomBookingGuid, BookedByUserId, CreatedById, StatusId, CreatedTime, [RoomBookingCode], [MeetingRoomId], [Subject], [StartDateTime], [EndDateTime], [Note])
                VALUES
                (@RoomBookingGuid, @BookedByUserId, @CreatedById, @StatusId, @CreatedTime, @RoomBookingCode, @MeetingRoomId, @Subject, @StartDateTime, @EndDateTime, @Note){dialect.ReturnIdentitySuffix("RoomBookingId")}";
            roomBooking.RoomBookingId = await conn.QuerySingleAsync<long>(sql, roomBooking, transaction);
        }

        public async Task UpdateRoomBookingAsync(RoomBooking roomBooking, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(roomBooking);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [BookedByUserId] = @BookedByUserId,
                    [RoomBookingCode] = @RoomBookingCode,
                    [MeetingRoomId] = @MeetingRoomId,
                    [Subject] = @Subject,
                    [StartDateTime] = @StartDateTime,
                    [EndDateTime] = @EndDateTime,
                    [Note] = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE RoomBookingGuid = @RoomBookingGuid";
            await conn.ExecuteAsync(sql, roomBooking, transaction);
        }

        public async Task SoftDeleteRoomBookingAsync(RoomBooking roomBooking, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(roomBooking, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE RoomBookingGuid = @RoomBookingGuid";

            await conn.ExecuteAsync(sql, roomBooking, transaction);
        }

        public async Task DeleteRoomBookingAsync(Guid roomBookingGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE RoomBookingGuid = @roomBookingGuid";
            await conn.ExecuteAsync(sql, new { roomBookingGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE BookedByUserId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<RoomBooking>> SearchRoomBookingAsync(
            string? roomBookingCode,
            string? roomBookingCodeSearchType,
            string? subject,
            string? subjectSearchType,
            string? startDateTime,
            string? startDateTimeSearchType,
            string? endDateTime,
            string? endDateTimeSearchType,
            string? note,
            string? noteSearchType,
            Guid personGuid, Guid roomBookingGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_BookedByUserId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (roomBookingGuid != Guid.Empty)
            {
                whereClauses.Add("a.RoomBookingGuid = @roomBookingGuid");
                parameters.Add("@roomBookingGuid", roomBookingGuid);
            }

            if (!string.IsNullOrWhiteSpace(roomBookingCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.RoomBookingCode", "@roomBookingCode", roomBookingCodeSearchType, parameters, "roomBookingCode", roomBookingCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                var param = SqlFilterHelper.BuildFilter("a.Subject", "@subject", subjectSearchType, parameters, "subject", subject, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(startDateTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.StartDateTime", "@startDateTime", startDateTimeSearchType, parameters, "startDateTime", startDateTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(endDateTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.EndDateTime", "@endDateTime", endDateTimeSearchType, parameters, "endDateTime", endDateTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_MeetingRoomId.MeetingRoomName AS MeetingRoomName
, j_BookedByUserId.BookedbyUserName AS BookedbyUserName

, j_BookedByUserId.PersonName AS PersonName

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "MeetingRoom")} j_MeetingRoomId ON a.MeetingRoomId = j_MeetingRoomId.MeetingRoomId
LEFT JOIN {dialect.Qualify("app", "Person")} j_BookedByUserId ON a.BookedByUserId = j_BookedByUserId.BookedbyUserId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.RoomBookingId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<RoomBooking>(sql, parameters, transaction);
        }
    }
}
