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
    public class CityRepository : ICityRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;


        public CityRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _auditService = auditService;
            _dialect = dialect;
            _table = dialect.Qualify("core", "Province");
        }

        public async Task<long?> GetProvinceIdByGuidAsync(Guid provinceGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT ProvinceId FROM {_table} WHERE ProvinceGuid = @provinceGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { provinceGuid });
        }

        public async Task<long?> GetCityIdByGuidAsync(Guid cityGuid)
        {
            using var connection = _context.CreateConnection();
            const string sql = "SELECT CityId FROM [core].[City] WHERE CityGuid = @cityGuid";
            return await connection.QuerySingleOrDefaultAsync<long?>(sql, new { cityGuid });
        }

        public async Task<City?> GetCityAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} 
                FROM [core].[City] ci
                INNER JOIN {_table} p ON ci.ProvinceId = p.ProvinceId
                WHERE ci.CityGuid = @cityGuid AND p.ProvinceGuid = @provinceGuid AND ci.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<City>(sql, new { cityGuid, provinceGuid });
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync(Guid provinceGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM [core].[City] a
                INNER JOIN {_table} p ON a.ProvinceId = p.ProvinceId
                WHERE p.ProvinceGuid = @provinceGuid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CityId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<City>(sql, new { provinceGuid });
        }

        public async Task CreateCityAsync(City city, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(city);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO [core].[City]
                                 (CityGuid, CityName, ProvinceId, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@CityGuid, @CityName, @ProvinceId, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, city, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "City",
                primaryKey: city.CityGuid.ToString(),
                userId: city.CreatedById.ToString(),
                oldEntity: null,
                newEntity: city);
        }

        public async Task UpdateCityAsync(City city, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(city);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM [core].[City] WHERE CityGuid = @CityGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<City>(getSql, new { city.CityGuid }, transaction);

            var sql = $@"UPDATE [core].[City]
                                 SET CityName = @CityName,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE CityGuid = @CityGuid";
            await conn.ExecuteAsync(sql, city, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "City",
                primaryKey: city.CityGuid.ToString(),
                userId: city.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: city);
        }

        public async Task SoftDeleteCityAsync(City city, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(city, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM [core].[City] WHERE CityGuid = @CityGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<City>(getSql, new { city.CityGuid }, transaction);

            var sql = $@"UPDATE [core].[City]
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE CityGuid = @CityGuid";

            await conn.ExecuteAsync(sql, city, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "City",
                primaryKey: city.CityGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: city);
        }

        public async Task DeleteCityAsync(Guid cityGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var getSql = $"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM [core].[City] WHERE CityGuid = @CityGuid";
            var oldData = await conn.QuerySingleOrDefaultAsync<City>(getSql, new { cityGuid }, transaction);

            var sql = $@"DELETE FROM [core].[City]
                                 WHERE CityGuid = @cityGuid";
            await conn.ExecuteAsync(sql, new { cityGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "City",
                primaryKey: cityGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<City>> SearchCityAsync(
            Guid provinceGuid,
            string? cityName, string? cityNameSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "p.ProvinceGuid = @provinceGuid",
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();
            parameters.Add("@provinceGuid", provinceGuid);

            if (!string.IsNullOrWhiteSpace(cityName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CityName", "@cityName", cityNameSearchType, parameters, "cityName", cityName, _dialect);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM [core].[City] a
            INNER JOIN {_table} p ON a.ProvinceId = p.ProvinceId
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.CityId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<City>(sql, parameters, transaction);
        }
    }
}
