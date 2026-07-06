using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    // Old-style: does NOT inherit RepositoryBase<T>
    public partial class CompanyRepository : ICompanyRepository
    {
        private readonly RepositoryContext _context;

        public CompanyRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Company> GetCompanyAsync(Guid companyGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Company] a
                WHERE a.CompanyGuid = @companyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { companyGuid });
        }
        
        public async Task<Company?> GetCompanyByIdAsync(long companyId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Company] a
                WHERE a.CompanyId = @companyId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { companyId });
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Company] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CompanyId DESC";
            return await connection.QueryAsync<Company>(sql);
        }

        public async Task CreateCompanyAsync(Company company, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(company);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Company]
                (CompanyGuid, CreatedById, StatusId, CreatedTime, CompanyName, InitialName, TaxCompulsionNo, RegistrationNo, ParentCompanyId, DefaultCurrency, CompanyLogo)
                VALUES
                (@CompanyGuid, @CreatedById, @StatusId, @CreatedTime, @CompanyName, @InitialName, @TaxCompulsionNo, @RegistrationNo, @ParentCompanyId, @DefaultCurrency, @CompanyLogo);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            company.CompanyId = await conn.QuerySingleAsync<long>(sql, company, transaction);
        }

        public async Task UpdateCompanyAsync(Company company, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(company);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                UPDATE [app].[Company]
                SET CompanyName = @CompanyName,
                                      InitialName = @InitialName,
                                      TaxCompulsionNo = @TaxCompulsionNo,
                                      RegistrationNo = @RegistrationNo,
                                      ParentCompanyId = @ParentCompanyId,
                                      DefaultCurrency = @DefaultCurrency,
                                      CompanyLogo = @CompanyLogo,
                                      StatusId = @StatusId,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime
                WHERE CompanyGuid = @CompanyGuid";
            await conn.ExecuteAsync(sql, company, transaction);
        }

        public async Task SoftDeleteCompanyAsync(Company company, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(company, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                UPDATE [app].[Company]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CompanyGuid = @CompanyGuid";

            await conn.ExecuteAsync(sql, company, transaction);
        }

        public async Task DeleteCompanyAsync(Guid companyGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"DELETE FROM [app].[Company] WHERE CompanyGuid = @companyGuid";
            await conn.ExecuteAsync(sql, new { companyGuid }, transaction);
        }


        public async Task<IEnumerable<Company>> SearchCompanyAsync(
            string? companyName, string? companyNameSearchType,
string? initialName, string? initialNameSearchType,
string? taxCompulsionNo, string? taxCompulsionNoSearchType,
string? registrationNo, string? registrationNoSearchType,
string? defaultCurrency, string? defaultCurrencySearchType,
IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 CompanyName
if (!string.IsNullOrWhiteSpace(companyName))
{
    var param = SqlFilterHelper.BuildFilter("a.CompanyName", "@companyName", companyNameSearchType, parameters, "companyName", companyName);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 InitialName
if (!string.IsNullOrWhiteSpace(initialName))
{
    var param = SqlFilterHelper.BuildFilter("a.InitialName", "@initialName", initialNameSearchType, parameters, "initialName", initialName);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 TaxCompulsionNo
if (!string.IsNullOrWhiteSpace(taxCompulsionNo))
{
    var param = SqlFilterHelper.BuildFilter("a.TaxCompulsionNo", "@taxCompulsionNo", taxCompulsionNoSearchType, parameters, "taxCompulsionNo", taxCompulsionNo);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 RegistrationNo
if (!string.IsNullOrWhiteSpace(registrationNo))
{
    var param = SqlFilterHelper.BuildFilter("a.RegistrationNo", "@registrationNo", registrationNoSearchType, parameters, "registrationNo", registrationNo);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 DefaultCurrency
if (!string.IsNullOrWhiteSpace(defaultCurrency))
{
    var param = SqlFilterHelper.BuildFilter("a.DefaultCurrency", "@defaultCurrency", defaultCurrencySearchType, parameters, "defaultCurrency", defaultCurrency);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}
            

            var sql = $@"
                SELECT a.*
                
                FROM [app].[Company] a
                
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CompanyId DESC";

            return await connection.QueryAsync<Company>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        
    }
}
