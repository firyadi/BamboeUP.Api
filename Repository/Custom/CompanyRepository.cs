using Dapper;
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public partial class CompanyRepository
    {
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[Company] a
                WHERE a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CompanyId DESC";
            return await connection.QueryAsync<Company>(sql);
        }

        public async Task<Company?> GetCompanyByIdAsync(long companyId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP 1 a.*
                FROM [app].[Company] a
                WHERE a.CompanyId = @companyId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { companyId });
        }
    }
}
