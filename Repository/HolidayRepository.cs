using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class HolidayRepository(RepositoryContext context, ISqlDialect dialect) : IHolidayRepository
    {
        private readonly string _table = dialect.Qualify("app", "Holiday");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Holiday?> GetHolidayAsync(Guid holidayGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.HolidayGuid = @holidayGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Holiday>(sql, new { holidayGuid });
        }

        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.HolidayId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Holiday>(sql);
        }

        public async Task CreateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (HolidayGuid, CreatedById, StatusId, CreatedTime, HolidayDate, HolidayName, HolidayDescription, SrHolidayType, IsNational, IsCompanyHoliday
                )
                VALUES
                (@HolidayGuid, @CreatedById, @StatusId, @CreatedTime, @HolidayDate, @HolidayName, @HolidayDescription, @SrHolidayType, @IsNational, @IsCompanyHoliday
                );
                {dialect.ReturnIdentitySuffix("HolidayId")}";
            holiday.HolidayId = await conn.QuerySingleAsync<long>(sql, holiday, transaction);
        }

        public async Task UpdateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     HolidayDate = @HolidayDate,
                    HolidayName = @HolidayName,
                    HolidayDescription = @HolidayDescription,
                    SrHolidayType = @SrHolidayType,
                    IsNational = @IsNational,
                    IsCompanyHoliday = @IsCompanyHoliday,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE HolidayGuid = @HolidayGuid";
            await conn.ExecuteAsync(sql, holiday, transaction);
        }

        public async Task SoftDeleteHolidayAsync(Holiday holiday, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE HolidayGuid = @HolidayGuid";

            await conn.ExecuteAsync(sql, holiday, transaction);
        }

        public async Task DeleteHolidayAsync(Guid holidayGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE HolidayGuid = @holidayGuid";
            await conn.ExecuteAsync(sql, new { holidayGuid }, transaction);
        }

        public async Task<IEnumerable<Holiday>> SearchHolidayAsync(
            string? holidayDate,
            string? holidayDateSearchType,
            string? holidayName,
            string? holidayNameSearchType,
            string? holidayDescription,
            string? holidayDescriptionSearchType,
            string? srHolidayType,
            string? srHolidayTypeSearchType,
            string? isNational,
            string? isNationalSearchType,
            string? isCompanyHoliday,
            string? isCompanyHolidaySearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(holidayDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.HolidayDate", "@holidayDate", holidayDateSearchType, parameters, "holidayDate", holidayDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(holidayName))
            {
                var param = SqlFilterHelper.BuildFilter("a.HolidayName", "@holidayName", holidayNameSearchType, parameters, "holidayName", holidayName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(holidayDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.HolidayDescription", "@holidayDescription", holidayDescriptionSearchType, parameters, "holidayDescription", holidayDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srHolidayType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrHolidayType", "@srHolidayType", srHolidayTypeSearchType, parameters, "srHolidayType", srHolidayType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isNational))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsNational", "@isNational", isNationalSearchType, parameters, "isNational", isNational, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCompanyHoliday))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCompanyHoliday", "@isCompanyHoliday", isCompanyHolidaySearchType, parameters, "isCompanyHoliday", isCompanyHoliday, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.HolidayId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Holiday>(sql, parameters, transaction);
        }

        public async Task<IEnumerable<Holiday>> GetHolidaysByDateRangeAsync(
            DateTime start,
            DateTime end,
            string? category = null,
            string? search = null,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                SELECT a.*
                FROM {_table} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                  AND a.HolidayDate < @End
                  AND a.HolidayDate >= @Start
                  AND (@Category IS NULL OR a.SrHolidayType = @Category)
                  AND (@Search IS NULL OR a.HolidayName LIKE N'%' + @Search + N'%'
                       OR a.HolidayDescription LIKE N'%' + @Search + N'%')
                ORDER BY a.HolidayDate, a.HolidayName";

            return await connection.QueryAsync<Holiday>(sql, new
            {
                Start = start.Date,
                End = end.Date,
                Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
                Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim()
            }, transaction);
        }
    }
}
