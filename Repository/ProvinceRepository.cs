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
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;


        public ProvinceRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _auditService = auditService;
            _dialect = dialect;
            _table = dialect.Qualify("core", "Province");
        }

        public async Task<Province?> GetProvinceAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} 
                FROM {_table} p
                INNER JOIN [core].[Country] c ON p.CountryId = c.CountryId
                WHERE p.ProvinceGuid = @provinceGuid AND c.CountryGuid = @countryGuid AND p.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<Province>(sql, new { provinceGuid, countryGuid });
        }

        public async Task<IEnumerable<Province>> GetAllProvincesAsync(Guid countryGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                INNER JOIN [core].[Country] c ON a.CountryId = c.CountryId
                WHERE c.CountryGuid = @countryGuid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.ProvinceId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Province>(sql, new { countryGuid });
        }

        public async Task CreateProvinceAsync(Province province, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(province);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO {_table}
                                 (ProvinceGuid, ProvinceName, CountryId, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@ProvinceGuid, @ProvinceName, @CountryId, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, province, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "Province",
                primaryKey: province.ProvinceGuid.ToString(),
                userId: province.CreatedById.ToString(),
                oldEntity: null,
                newEntity: province);
        }

        public async Task UpdateProvinceAsync(Province province, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(province);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE ProvinceGuid = @ProvinceGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<Province>(getSql, new { province.ProvinceGuid }, transaction);

            var sql = $@"UPDATE {_table}
                                 SET ProvinceName = @ProvinceName,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE ProvinceGuid = @ProvinceGuid";
            await conn.ExecuteAsync(sql, province, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "Province",
                primaryKey: province.ProvinceGuid.ToString(),
                userId: province.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: province);
        }

        public async Task SoftDeleteProvinceAsync(Province province, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(province, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE ProvinceGuid = @ProvinceGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<Province>(getSql, new { province.ProvinceGuid }, transaction);

            var sql = $@"UPDATE {_table}
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE ProvinceGuid = @ProvinceGuid";

            await conn.ExecuteAsync(sql, province, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Province",
                primaryKey: province.ProvinceGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: province);
        }

        public async Task DeleteProvinceAsync(Guid provinceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} WHERE ProvinceGuid = @ProvinceGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<Province>(getSql, new { provinceGuid }, transaction);

            var sql = $@"DELETE FROM {_table}
                                 WHERE ProvinceGuid = @provinceGuid";
            await conn.ExecuteAsync(sql, new { provinceGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Province",
                primaryKey: provinceGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<Province>> SearchProvinceAsync(
            Guid countryGuid,
            string? provinceName, string? provinceNameSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "c.CountryGuid = @countryGuid",
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@countryGuid", countryGuid);

            if (!string.IsNullOrWhiteSpace(provinceName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProvinceName", "@provinceName", provinceNameSearchType, parameters, "provinceName", provinceName, _dialect);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM {_table} a
            INNER JOIN [core].[Country] c ON a.CountryId = c.CountryId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.ProvinceId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Province>(sql, parameters, transaction);
        }
    }
}
