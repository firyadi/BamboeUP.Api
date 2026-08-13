using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class HolidayRepository(RepositoryContext context) : IHolidayRepository
    {
        public async Task<Holiday?> GetHolidayAsync(Guid holidayGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
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
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*



                FROM [app].[Holiday] a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.HolidayId DESC";
            return await connection.QueryAsync<Holiday>(sql);
        }

        public async Task CreateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Holiday]
                (HolidayGuid, CreatedById, StatusId, CreatedTime, HolidayDate, HolidayName, HolidayDescription, SrHolidayType, IsNational, IsCompanyHoliday
                )
                VALUES
                (@HolidayGuid, @CreatedById, @StatusId, @CreatedTime, @HolidayDate, @HolidayName, @HolidayDescription, @SrHolidayType, @IsNational, @IsCompanyHoliday
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            holiday.HolidayId = await conn.QuerySingleAsync<long>(sql, holiday, transaction);
        }

        public async Task UpdateHolidayAsync(Holiday holiday, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Holiday]
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
            const string sql = @"
                UPDATE [app].[Holiday]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE HolidayGuid = @HolidayGuid";

            await conn.ExecuteAsync(sql, holiday, transaction);
        }

        public async Task DeleteHolidayAsync(Guid holidayGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Holiday] WHERE HolidayGuid = @holidayGuid";
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
                var param = SqlFilterHelper.BuildFilter("a.HolidayDate", "@holidayDate", holidayDateSearchType, parameters, "holidayDate", holidayDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(holidayName))
            {
                var param = SqlFilterHelper.BuildFilter("a.HolidayName", "@holidayName", holidayNameSearchType, parameters, "holidayName", holidayName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(holidayDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.HolidayDescription", "@holidayDescription", holidayDescriptionSearchType, parameters, "holidayDescription", holidayDescription);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srHolidayType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrHolidayType", "@srHolidayType", srHolidayTypeSearchType, parameters, "srHolidayType", srHolidayType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isNational))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsNational", "@isNational", isNationalSearchType, parameters, "isNational", isNational);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCompanyHoliday))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCompanyHoliday", "@isCompanyHoliday", isCompanyHolidaySearchType, parameters, "isCompanyHoliday", isCompanyHoliday);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Holiday] a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.HolidayId DESC";

            return await connection.QueryAsync<Holiday>(sql, parameters, transaction);
        }
    }
}
