using Contracts;
using Dapper;
using Entities.Models;

using Repository.Database;
namespace Repository
{
    public class VwStandardReferenceItemRepository : IVwStandardReferenceItemRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public VwStandardReferenceItemRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<IEnumerable<VwStandardReferenceItem>> GetAllAsync(string? standardReferenceInitial, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT 
                    StandardReferenceItemId,
                    StandardReferenceItemGuid,
                    CompanyId,
                    CompanyGuid,
                    CompanyOfficeId,
                    CompanyOfficeGuid,
                    StandardReferenceId,
                    StandardReferenceGuid,
                    StandardReferenceInitial,
                    StandardReferenceItemInitial,
                    StandardReferenceItemName,
                    Note
                FROM {_dialect.Qualify("app", "vw_StandardReferenceItem")} ";

            if (!string.IsNullOrEmpty(standardReferenceInitial))
            {
                sql += " WHERE StandardReferenceInitial = @Initial ";
            }

            sql += " ORDER BY StandardReferenceItemName ASC";

            return await connection.QueryAsync<VwStandardReferenceItem>(sql, new { Initial = standardReferenceInitial });
        }

        public async Task<VwStandardReferenceItem?> GetOneAsync(long id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT 
                    StandardReferenceItemId,
                    StandardReferenceItemGuid,
                    CompanyId,
                    CompanyGuid,
                    CompanyOfficeId,
                    CompanyOfficeGuid,
                    StandardReferenceId,
                    StandardReferenceGuid,
                    StandardReferenceInitial,
                    StandardReferenceItemInitial,
                    StandardReferenceItemName,
                    Note
                FROM {_dialect.Qualify("app", "vw_StandardReferenceItem")}
                WHERE StandardReferenceItemId = @Id";

            return await connection.QuerySingleOrDefaultAsync<VwStandardReferenceItem>(sql, new { Id = id });
        }
    }
}
