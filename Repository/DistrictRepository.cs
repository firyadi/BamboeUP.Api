using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using System.Text;
using Repository.Extensions;

namespace Repository
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public DistrictRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<District> GetDistrictAsync(Guid cityGuid, Guid districtGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) d.* 
                FROM [core].[District] d
                INNER JOIN [core].[City] c ON d.CityId = c.CityId
                WHERE d.DistrictGuid = @districtGuid AND c.CityGuid = @cityGuid AND d.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<District>(sql, new { districtGuid, cityGuid });
        }

        public async Task<IEnumerable<District>> GetAllDistrictsAsync(Guid cityGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) d.*
                FROM [core].[District] d
                INNER JOIN [core].[City] c ON d.CityId = c.CityId
                WHERE c.CityGuid = @cityGuid AND d.StatusId > 0 AND d.DeletedTime IS NULL
                ORDER BY d.DistrictId DESC";

            return await connection.QueryAsync<District>(sql, new { cityGuid });
        }

        public async Task CreateDistrictAsync(District district, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampCreate(district);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[District]
                                 (DistrictGuid, DistrictName, CityId, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@DistrictGuid, @DistrictName, @CityId, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, district, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "District",
                primaryKey: district.DistrictGuid.ToString(),
                userId: district.CreatedById.ToString(),
                oldEntity: null,
                newEntity: district);
        }

        public async Task UpdateDistrictAsync(District district, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampUpdate(district);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[District] WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { district.DistrictGuid }, transaction);

            const string sql = @"UPDATE [core].[District]
                                 SET DistrictName = @DistrictName,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE DistrictGuid = @DistrictGuid";
            await conn.ExecuteAsync(sql, district, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "District",
                primaryKey: district.DistrictGuid.ToString(),
                userId: district.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: district);
        }

        public async Task SoftDeleteDistrictAsync(District district, long deletedBy, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(district, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[District] WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { district.DistrictGuid }, transaction);

            const string sql = @"UPDATE [core].[District]
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE DistrictGuid = @DistrictGuid";

            await conn.ExecuteAsync(sql, district, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "District",
                primaryKey: district.DistrictGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: district);
        }

        public async Task<long?> GetCityIdByGuidAsync(Guid cityGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT CityId FROM [core].[City] WHERE CityGuid = @cityGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { cityGuid });
        }

        public async Task<long?> GetDistrictIdByGuidAsync(Guid districtGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT DistrictId FROM [core].[District] WHERE DistrictGuid = @districtGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { districtGuid });
        }

        public async Task DeleteDistrictAsync(Guid districtGuid, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[District] WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { districtGuid }, transaction);

            const string sql = @"DELETE FROM [core].[District]
                                 WHERE DistrictGuid = @districtGuid";
            await conn.ExecuteAsync(sql, new { districtGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "District",
                primaryKey: districtGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<District>> SearchDistrictAsync(
            Guid cityGuid,
            string? districtName, string? districtNameSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "c.CityGuid = @cityGuid",
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@cityGuid", cityGuid);

            if (!string.IsNullOrWhiteSpace(districtName))
            {
                var param = SqlFilterHelper.BuildFilter("a.DistrictName", "@districtName", districtNameSearchType, parameters, "districtName", districtName);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM [core].[District] a
            INNER JOIN [core].[City] c ON a.CityId = c.CityId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.DistrictId DESC";

            return await connection.QueryAsync<District>(sql, parameters, transaction);
        }
    }
}
