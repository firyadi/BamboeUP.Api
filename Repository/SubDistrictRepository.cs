using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using System.Text;
using Repository.Extensions;

namespace Repository
{
    public class SubDistrictRepository : ISubDistrictRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public SubDistrictRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<SubDistrict> GetSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) s.* 
                FROM [core].[SubDistrict] s
                INNER JOIN [core].[District] d ON s.DistrictId = d.DistrictId
                WHERE s.SubDistrictGuid = @subDistrictGuid AND d.DistrictGuid = @districtGuid AND s.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<SubDistrict>(sql, new { subDistrictGuid, districtGuid });
        }

        public async Task<IEnumerable<SubDistrict>> GetAllSubDistrictsAsync(Guid districtGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) s.*
                FROM [core].[SubDistrict] s
                INNER JOIN [core].[District] d ON s.DistrictId = d.DistrictId
                WHERE d.DistrictGuid = @districtGuid AND s.StatusId > 0 AND s.DeletedTime IS NULL
                ORDER BY s.SubDistrictId DESC";

            return await connection.QueryAsync<SubDistrict>(sql, new { districtGuid });
        }

        public async Task CreateSubDistrictAsync(SubDistrict subDistrict, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(subDistrict);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[SubDistrict]
                                 (SubDistrictGuid, SubDistrictName, DistrictId, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@SubDistrictGuid, @SubDistrictName, @DistrictId, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, subDistrict, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "SubDistrict",
                primaryKey: subDistrict.SubDistrictGuid.ToString(),
                userId: subDistrict.CreatedById.ToString(),
                oldEntity: null,
                newEntity: subDistrict);
        }

        public async Task UpdateSubDistrictAsync(SubDistrict subDistrict, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(subDistrict);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[SubDistrict] WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrict.SubDistrictGuid }, transaction);

            const string sql = @"UPDATE [core].[SubDistrict]
                                 SET SubDistrictName = @SubDistrictName,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE SubDistrictGuid = @SubDistrictGuid";
            await conn.ExecuteAsync(sql, subDistrict, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "SubDistrict",
                primaryKey: subDistrict.SubDistrictGuid.ToString(),
                userId: subDistrict.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: subDistrict);
        }

        public async Task SoftDeleteSubDistrictAsync(SubDistrict subDistrict, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(subDistrict, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[SubDistrict] WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrict.SubDistrictGuid }, transaction);

            const string sql = @"UPDATE [core].[SubDistrict]
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE SubDistrictGuid = @SubDistrictGuid";

            await conn.ExecuteAsync(sql, subDistrict, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "SubDistrict",
                primaryKey: subDistrict.SubDistrictGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: subDistrict);
        }

        public async Task<long?> GetDistrictIdByGuidAsync(Guid districtGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT DistrictId FROM [core].[District] WHERE DistrictGuid = @districtGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { districtGuid });
        }

        public async Task<long?> GetSubDistrictIdByGuidAsync(Guid subDistrictGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT SubDistrictId FROM [core].[SubDistrict] WHERE SubDistrictGuid = @subDistrictGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { subDistrictGuid });
        }

        public async Task DeleteSubDistrictAsync(Guid subDistrictGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[SubDistrict] WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrictGuid }, transaction);

            const string sql = @"DELETE FROM [core].[SubDistrict]
                                 WHERE SubDistrictGuid = @subDistrictGuid";
            await conn.ExecuteAsync(sql, new { subDistrictGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "SubDistrict",
                primaryKey: subDistrictGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<SubDistrict>> SearchSubDistrictAsync(
            Guid districtGuid,
            string? subDistrictName, string? subDistrictNameSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "d.DistrictGuid = @districtGuid",
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@districtGuid", districtGuid);

            if (!string.IsNullOrWhiteSpace(subDistrictName))
            {
                var param = SqlFilterHelper.BuildFilter("a.SubDistrictName", "@subDistrictName", subDistrictNameSearchType, parameters, "subDistrictName", subDistrictName);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM [core].[SubDistrict] a
            INNER JOIN [core].[District] d ON a.DistrictId = d.DistrictId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.SubDistrictId DESC";

            return await connection.QueryAsync<SubDistrict>(sql, parameters, transaction);
        }
    }
}
