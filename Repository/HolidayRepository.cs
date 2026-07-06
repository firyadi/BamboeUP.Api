using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    // Old-style: does NOT inherit RepositoryBase<T>
    public class HolidayRepository : IHolidayRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public HolidayRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<Holiday> GetHolidayAsync(Guid holidayGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Holiday] a
                WHERE a.HolidayGuid = @holidayGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Holiday>(sql, new { holidayGuid });
        }

        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Holiday] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.HolidayId DESC";
            return await connection.QueryAsync<Holiday>(sql);
        }

        public async Task CreateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(holiday);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Holiday]
                (HolidayGuid, CreatedById, StatusId, CreatedTime, YearPeriode, HolidayDates, Note)
                VALUES
                (@HolidayGuid, @CreatedById, @StatusId, @CreatedTime, @YearPeriode, @HolidayDates, @Note)";
            await conn.ExecuteAsync(sql, holiday, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "Holiday",
                primaryKey: holiday.HolidayGuid.ToString(),
                userId: holiday.CreatedById.ToString(),
                oldEntity: null,
                newEntity: holiday);
        }

        public async Task UpdateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(holiday);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetHolidayAsync(holiday.HolidayGuid, false);

            const string sql = @"
                UPDATE [app].[Holiday]
                SET YearPeriode = @YearPeriode,
                                      HolidayDates = @HolidayDates,
                                      Note = @Note,
                                      StatusId = @StatusId,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime
                WHERE HolidayGuid = @HolidayGuid";
            await conn.ExecuteAsync(sql, holiday, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "Holiday",
                primaryKey: holiday.HolidayGuid.ToString(),
                userId: holiday.UpdatedById.ToString(),
                oldEntity: oldData,
                newEntity: holiday);
        }

        public async Task SoftDeleteHolidayAsync(Holiday holiday, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(holiday, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetHolidayAsync(holiday.HolidayGuid, false);

            const string sql = @"
                UPDATE [app].[Holiday]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE HolidayGuid = @HolidayGuid";

            await conn.ExecuteAsync(sql, holiday, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "Holiday",
                primaryKey: holiday.HolidayGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteHolidayAsync(Guid holidayGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetHolidayAsync(holidayGuid, false);

            const string sql = @"DELETE FROM [app].[Holiday] WHERE HolidayGuid = @holidayGuid";
            await conn.ExecuteAsync(sql, new { holidayGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "Holiday",
                primaryKey: holidayGuid.ToString(),
                userId: oldData?.DeletedById.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }


        public async Task<IEnumerable<Holiday>> SearchHolidayAsync(
            string? yearPeriode, string? yearPeriodeSearchType,
string? note, string? noteSearchType,
DateTime? holidayDatesFrom, DateTime? holidayDatesTo,
IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 YearPeriode
if (!string.IsNullOrWhiteSpace(yearPeriode))
{
    var param = SqlFilterHelper.BuildFilter("a.YearPeriode", "@yearPeriode", yearPeriodeSearchType, parameters, "yearPeriode", yearPeriode);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 Note
if (!string.IsNullOrWhiteSpace(note))
{
    var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 HolidayDates (range)
if (holidayDatesFrom.HasValue)
{
    whereClauses.Add("a.HolidayDates >= @holidayDatesFrom");
    parameters.Add("holidayDatesFrom", holidayDatesFrom.Value.Date);
}
if (holidayDatesTo.HasValue)
{
    whereClauses.Add("a.HolidayDates < @holidayDatesToPlusOne");
    parameters.Add("holidayDatesToPlusOne", holidayDatesTo.Value.Date.AddDays(1));
}
            

            var sql = $@"
                SELECT a.*
                , FORMAT(a.HolidayDates, 'yyyy-MM-dd') AS HolidayDatesString
                FROM [app].[Holiday] a
                
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.HolidayId DESC";

            return await connection.QueryAsync<Holiday>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        
    }
}
