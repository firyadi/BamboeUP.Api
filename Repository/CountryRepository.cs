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
    public class CountryRepository : ICountryRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;


        public CountryRepository(RepositoryContext context, IAuditService auditService, ISqlDialect dialect)
        {
            _context = context;
            _auditService = auditService;
            _dialect = dialect;
            _table = dialect.Qualify("core", "Country");
        }

        public async Task<Country?> GetCountryAsync(Guid countryGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"{SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")} FROM {_table} 
                                 WHERE CountryGuid = @countryGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<Country>(sql, new { countryGuid });
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CountryId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Country>(sql);
        }

        public async Task CreateCountryAsync(Country country, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(country);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"INSERT INTO {_table}
                                 (CountryGuid, CountryIso, CountryName, CountryIso3, PhoneCode, CurrencyCode, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@CountryGuid, @CountryIso, @CountryName, @CountryIso3, @PhoneCode, @CurrencyCode, @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, country, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "Country",
                primaryKey: country.CountryGuid.ToString(),
                userId: country.CreatedById.ToString(),
                oldEntity: null,
                newEntity: country);
        }

        public async Task UpdateCountryAsync(Country country, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(country);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetCountryAsync(country.CountryGuid, false);

            var sql = $@"UPDATE {_table}
                                 SET CountryIso = @CountryIso,
                                     CountryName = @CountryName,
                                     CountryIso3 = @CountryIso3,
                                     PhoneCode = @PhoneCode,
                                     CurrencyCode = @CurrencyCode,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE CountryGuid = @CountryGuid";
            await conn.ExecuteAsync(sql, country, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "Country",
                primaryKey: country.CountryGuid.ToString(),
                userId: country.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: country);
        }

        public async Task SoftDeleteCountryAsync(Country country, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(country, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetCountryAsync(country.CountryGuid, false);

            var sql = $@"UPDATE {_table}
                         SET StatusId = 0,
                             DeletedById = @DeletedById,
                             DeletedTime = @DeletedTime
                         WHERE CountryGuid = @CountryGuid";

            await conn.ExecuteAsync(sql, country, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Country",
                primaryKey: country.CountryGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: country);
        }

        public async Task DeleteCountryAsync(Guid countryGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetCountryAsync(countryGuid, false);

            var sql = $@"DELETE FROM {_table}
                                 WHERE CountryGuid = @countryGuid";
            await conn.ExecuteAsync(sql, new { countryGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Country",
                primaryKey: countryGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<Country>> SearchCountryAsync(
            string? countryName, string? countryNameSearchType,
            string? countryIso, string? countryIsoSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(countryName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CountryName", "@countryName", countryNameSearchType, parameters, "countryName", countryName, _dialect);
                whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(countryIso))
            {
                var param = SqlFilterHelper.BuildFilter("a.CountryIso", "@countryIso", countryIsoSearchType, parameters, "countryIso", countryIso, _dialect);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM {_table} a
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.CountryId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Country>(sql, parameters, transaction);
        }
    }
}
