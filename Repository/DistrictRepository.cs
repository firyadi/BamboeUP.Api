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
    public class DistrictRepository : IDistrictRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;


        public DistrictRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _auditService = auditService;
            _dialect = dialect;
            _table = dialect.Qualify("core", "District");
        }

        public async Task<District?> GetDistrictAsync(Guid cityGuid, Guid districtGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} 
                FROM {_table} a
                INNER JOIN [core].[City] c ON a.CityId = c.CityId
                WHERE a.DistrictGuid = @districtGuid AND c.CityGuid = @cityGuid AND a.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<District>(sql, new { districtGuid, cityGuid });
        }

        public async Task<IEnumerable<District>> GetAllDistrictsAsync(Guid cityGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                INNER JOIN [core].[City] c ON a.CityId = c.CityId
                WHERE c.CityGuid = @cityGuid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.DistrictId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<District>(sql, new { cityGuid });
        }

        public async Task CreateDistrictAsync(District district, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(district);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO {_table}
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

        public async Task UpdateDistrictAsync(District district, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(district);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { district.DistrictGuid }, transaction);

            var sql = $@"UPDATE {_table}
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
                userId: district.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: district);
        }

        public async Task SoftDeleteDistrictAsync(District district, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(district, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { district.DistrictGuid }, transaction);

            var sql = $@"UPDATE {_table}
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
            const string sql = "SELECT DistrictId FROM {_table} WHERE DistrictGuid = @districtGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { districtGuid });
        }

        public async Task DeleteDistrictAsync(Guid districtGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE DistrictGuid = @DistrictGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<District>(getSql, new { districtGuid }, transaction);

            var sql = $@"DELETE FROM {_table}
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
                var param = SqlFilterHelper.BuildFilter("a.DistrictName", "@districtName", districtNameSearchType, parameters, "districtName", districtName, _dialect);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM {_table} a
            INNER JOIN [core].[City] c ON a.CityId = c.CityId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.DistrictId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<District>(sql, parameters, transaction);
        }
    }
}
