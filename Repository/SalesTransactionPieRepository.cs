using Contracts;
using Dapper;
using Repository.Database;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class SalesTransactionPieRepository : ISalesTransactionPieRepository
    {
        private readonly RepositoryContext _context;
        private readonly string _procedure;

        public SalesTransactionPieRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _procedure = dialect.Qualify("app", "usp_SalesTransactionPie");
        }

        public async Task<SalesTransactionPieResultDto> QueryAsync(SalesTransactionPieQueryDto query)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("CompanyId", query.CompanyId);
            parameters.Add("CompanyOfficeId", query.CompanyOfficeId);
            parameters.Add("DateFrom", query.DateFrom);
            parameters.Add("DateTo", query.DateTo);
                using var grid = await connection.QueryMultipleAsync(
                    _procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);
                var result = await grid.ReadFirstOrDefaultAsync<SalesTransactionPieResultDto>()
                    ?? new SalesTransactionPieResultDto();
                result.Rows = (await grid.ReadAsync<SalesTransactionPieRowDto>()).ToList();
                return result;
        }
    }
}