using Dapper;
using Entities.Models;
using Repository.Extensions;
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
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_LeaderId.PersonName AS LeaderName
, j_ParentCompanyId.CompanyName AS ParentCompanyName
                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN {_table} j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId
                WHERE a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CompanyId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Company>(sql);
        }

        public async Task<Company?> GetCompanyByIdAsync(long companyId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, 1, "a.*")}
, j_LeaderId.PersonName AS LeaderName
, j_ParentCompanyId.CompanyName AS ParentCompanyName
                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_LeaderId ON a.LeaderId = j_LeaderId.PersonId
LEFT JOIN {_table} j_ParentCompanyId ON a.ParentCompanyId = j_ParentCompanyId.CompanyId
                WHERE a.CompanyId = @companyId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL{dialect.AppendLimit(1)}";
            return await connection.QuerySingleOrDefaultAsync<Company>(sql, new { companyId });
        }
    }
}
