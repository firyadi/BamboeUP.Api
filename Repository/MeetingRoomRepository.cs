using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class MeetingRoomRepository(RepositoryContext context, ISqlDialect dialect) : IMeetingRoomRepository
    {
        private readonly string _table = dialect.Qualify("app", "MeetingRoom");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<MeetingRoom?> GetMeetingRoomAsync(Guid meetingRoomGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.MeetingRoomGuid = @meetingRoomGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<MeetingRoom>(sql, new { meetingRoomGuid });
        }

        public async Task<IEnumerable<MeetingRoom>> GetAllMeetingRoomsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.MeetingRoomId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<MeetingRoom>(sql);
        }

        public async Task CreateMeetingRoomAsync(MeetingRoom meetingRoom, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (MeetingRoomGuid, CreatedById, StatusId, CreatedTime, RoomCode, RoomName, Capacity, Location, Note
                )
                VALUES
                (@MeetingRoomGuid, @CreatedById, @StatusId, @CreatedTime, @RoomCode, @RoomName, @Capacity, @Location, @Note
                );
                {dialect.ReturnIdentitySuffix("MeetingRoomId")}";
            meetingRoom.MeetingRoomId = await conn.QuerySingleAsync<long>(sql, meetingRoom, transaction);
        }

        public async Task UpdateMeetingRoomAsync(MeetingRoom meetingRoom, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     RoomCode = @RoomCode,
                    RoomName = @RoomName,
                    Capacity = @Capacity,
                    Location = @Location,
                    Note = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE MeetingRoomGuid = @MeetingRoomGuid";
            await conn.ExecuteAsync(sql, meetingRoom, transaction);
        }

        public async Task SoftDeleteMeetingRoomAsync(MeetingRoom meetingRoom, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE MeetingRoomGuid = @MeetingRoomGuid";

            await conn.ExecuteAsync(sql, meetingRoom, transaction);
        }

        public async Task DeleteMeetingRoomAsync(Guid meetingRoomGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE MeetingRoomGuid = @meetingRoomGuid";
            await conn.ExecuteAsync(sql, new { meetingRoomGuid }, transaction);
        }

        public async Task<IEnumerable<MeetingRoom>> SearchMeetingRoomAsync(
            string? roomCode,
            string? roomCodeSearchType,
            string? roomName,
            string? roomNameSearchType,
            string? capacity,
            string? capacitySearchType,
            string? location,
            string? locationSearchType,
            string? note,
            string? noteSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(roomCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.RoomCode", "@roomCode", roomCodeSearchType, parameters, "roomCode", roomCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(roomName))
            {
                var param = SqlFilterHelper.BuildFilter("a.RoomName", "@roomName", roomNameSearchType, parameters, "roomName", roomName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(capacity))
            {
                var param = SqlFilterHelper.BuildFilter("a.Capacity", "@capacity", capacitySearchType, parameters, "capacity", capacity, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var param = SqlFilterHelper.BuildFilter("a.Location", "@location", locationSearchType, parameters, "location", location, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.MeetingRoomId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<MeetingRoom>(sql, parameters, transaction);
        }
    }
}
