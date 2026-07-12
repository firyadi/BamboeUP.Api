using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using System.Text;
using Repository.Extensions;

namespace Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public CountryRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Country> GetCountryAsync(Guid countryGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[Country] 
                                 WHERE CountryGuid = @countryGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<Country>(sql, new { countryGuid });
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [core].[Country] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CountryId DESC";

            return await connection.QueryAsync<Country>(sql);
        }

        public async Task CreateCountryAsync(Country country, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(country);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[Country]
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

            const string sql = @"UPDATE [core].[Country]
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
                userId: country.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: country);
        }

        public async Task SoftDeleteCountryAsync(Country country, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(country, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetCountryAsync(country.CountryGuid, false);

            const string sql = @"UPDATE [core].[Country]
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

            const string sql = @"DELETE FROM [core].[Country]
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
                var param = SqlFilterHelper.BuildFilter("a.CountryName", "@countryName", countryNameSearchType, parameters, "countryName", countryName);
                whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(countryIso))
            {
                var param = SqlFilterHelper.BuildFilter("a.CountryIso", "@countryIso", countryIsoSearchType, parameters, "countryIso", countryIso);
                whereClauses.Add(param);
            }

            var sql = $@"
            SELECT a.*
            FROM [core].[Country] a
            WHERE {string.Join(" AND ", whereClauses)}
            ORDER BY a.CountryId DESC";

            return await connection.QueryAsync<Country>(sql, parameters, transaction);
        }
    }
}
