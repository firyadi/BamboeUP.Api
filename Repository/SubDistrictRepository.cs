using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using System.Text;
using Repository.Database;
using Repository.Extensions;

namespace Repository
{
    public class SubDistrictRepository : ISubDistrictRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;


        public SubDistrictRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _auditService = auditService;
            _dialect = dialect;
            _table = dialect.Qualify("core", "SubDistrict");
        }

        public async Task<SubDistrict?> GetSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} 
                FROM {_table} a
                INNER JOIN [core].[District] d ON a.DistrictId = d.DistrictId
                WHERE a.SubDistrictGuid = @subDistrictGuid AND d.DistrictGuid = @districtGuid AND a.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<SubDistrict>(sql, new { subDistrictGuid, districtGuid });
        }

        public async Task<IEnumerable<SubDistrict>> GetAllSubDistrictsAsync(Guid districtGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                INNER JOIN [core].[District] d ON a.DistrictId = d.DistrictId
                WHERE d.DistrictGuid = @districtGuid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.SubDistrictId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<SubDistrict>(sql, new { districtGuid });
        }

        public async Task CreateSubDistrictAsync(SubDistrict subDistrict, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(subDistrict);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO {_table}
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
            
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrict.SubDistrictGuid }, transaction);

            var sql = $@"UPDATE {_table}
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
                userId: subDistrict.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: subDistrict);
        }

        public async Task SoftDeleteSubDistrictAsync(SubDistrict subDistrict, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(subDistrict, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrict.SubDistrictGuid }, transaction);

            var sql = $@"UPDATE {_table}
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
            const string sql = "SELECT SubDistrictId FROM {_table} WHERE SubDistrictGuid = @subDistrictGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { subDistrictGuid });
        }

        public async Task DeleteSubDistrictAsync(Guid subDistrictGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE SubDistrictGuid = @SubDistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<SubDistrict>(getSql, new { subDistrictGuid }, transaction);

            var sql = $@"DELETE FROM {_table}
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
                var param = SqlFilterHelper.BuildFilter("a.SubDistrictName", "@subDistrictName", subDistrictNameSearchType, parameters, "subDistrictName", subDistrictName, _dialect);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM {_table} a
            INNER JOIN [core].[District] d ON a.DistrictId = d.DistrictId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.SubDistrictId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<SubDistrict>(sql, parameters, transaction);
        }
    }
}
