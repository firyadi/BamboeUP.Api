using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;

namespace Repository
{
    public partial class StandardReferenceItemDisplayRepository : IStandardReferenceItemDisplayRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;

        public StandardReferenceItemDisplayRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StandardReferenceItemDisplay>> GetItemsAsync(
            long companyId,
            long companyOfficeId,
            long? departmentId,
            string standardReferenceInitial)
        {
            using var connection = _context.CreateConnection();
            var sql = SqlDialectQueryHelper.BuildStandardReferenceItemsQuery(_dialect);

            return await connection.QueryAsync<StandardReferenceItemDisplay>(sql, new
            {
                CompanyId = companyId,
                CompanyOfficeId = companyOfficeId,
                DepartmentId = departmentId,
                StandardReferenceInitial = standardReferenceInitial
            });
        }
    }
}
