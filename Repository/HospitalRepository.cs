using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class HospitalRepository : IHospitalRepository
    {
        private readonly RepositoryContext _context;

        public HospitalRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Hospital> GetHospitalAsync(Guid hospitalGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Hospital] a

                WHERE a.HospitalGuid = @hospitalGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Hospital>(sql, new { hospitalGuid });
        }

        public async Task<IEnumerable<Hospital>> GetAllHospitalsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Hospital] a

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.HospitalId DESC";
            return await connection.QueryAsync<Hospital>(sql);
        }

        public async Task CreateHospitalAsync(Hospital hospital, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(hospital);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Hospital]
                (HospitalGuid, CreatedById, StatusId, CreatedTime, HospitalName, HospitalCode, ShortName, LicenseNo, HospitalType, HospitalClass, PhoneNo, Email, Website
                )
                VALUES
                (@HospitalGuid, @CreatedById, @StatusId, @CreatedTime, @HospitalName, @HospitalCode, @ShortName, @LicenseNo, @HospitalType, @HospitalClass, @PhoneNo, @Email, @Website
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            hospital.HospitalId = await conn.QuerySingleAsync<long>(sql, hospital, transaction);
        }

        public async Task UpdateHospitalAsync(Hospital hospital, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(hospital);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Hospital]
                SET                     HospitalName = @HospitalName,
                    HospitalCode = @HospitalCode,
                    ShortName = @ShortName,
                    LicenseNo = @LicenseNo,
                    HospitalType = @HospitalType,
                    HospitalClass = @HospitalClass,
                    PhoneNo = @PhoneNo,
                    Email = @Email,
                    Website = @Website,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE HospitalGuid = @HospitalGuid";
            await conn.ExecuteAsync(sql, hospital, transaction);
        }

        public async Task SoftDeleteHospitalAsync(Hospital hospital, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(hospital, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Hospital]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE HospitalGuid = @HospitalGuid";

            await conn.ExecuteAsync(sql, hospital, transaction);
        }

        public async Task DeleteHospitalAsync(Guid hospitalGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Hospital] WHERE HospitalGuid = @hospitalGuid";
            await conn.ExecuteAsync(sql, new { hospitalGuid }, transaction);
        }

        public async Task<IEnumerable<Hospital>> SearchHospitalAsync(
            string? hospitalName,
            string? hospitalNameSearchType,
            string? hospitalCode,
            string? hospitalCodeSearchType,
            string? shortName,
            string? shortNameSearchType,
            string? licenseNo,
            string? licenseNoSearchType,
            string? hospitalType,
            string? hospitalTypeSearchType,
            string? hospitalClass,
            string? hospitalClassSearchType,
            string? phoneNo,
            string? phoneNoSearchType,
            string? email,
            string? emailSearchType,
            string? website,
            string? websiteSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(hospitalName))
            {
                var param = SqlFilterHelper.BuildFilter("a.HospitalName", "@hospitalName", hospitalNameSearchType, parameters, "hospitalName", hospitalName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hospitalCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.HospitalCode", "@hospitalCode", hospitalCodeSearchType, parameters, "hospitalCode", hospitalCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(shortName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ShortName", "@shortName", shortNameSearchType, parameters, "shortName", shortName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(licenseNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.LicenseNo", "@licenseNo", licenseNoSearchType, parameters, "licenseNo", licenseNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hospitalType))
            {
                var param = SqlFilterHelper.BuildFilter("a.HospitalType", "@hospitalType", hospitalTypeSearchType, parameters, "hospitalType", hospitalType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hospitalClass))
            {
                var param = SqlFilterHelper.BuildFilter("a.HospitalClass", "@hospitalClass", hospitalClassSearchType, parameters, "hospitalClass", hospitalClass);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(phoneNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.PhoneNo", "@phoneNo", phoneNoSearchType, parameters, "phoneNo", phoneNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var param = SqlFilterHelper.BuildFilter("a.Email", "@email", emailSearchType, parameters, "email", email);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(website))
            {
                var param = SqlFilterHelper.BuildFilter("a.Website", "@website", websiteSearchType, parameters, "website", website);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Hospital] a

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.HospitalId DESC";

            return await connection.QueryAsync<Hospital>(sql, parameters, transaction);
        }
    }
}
