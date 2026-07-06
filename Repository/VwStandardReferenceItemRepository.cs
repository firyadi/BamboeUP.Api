using Contracts;
using Dapper;
using Entities.Models;

namespace Repository
{
    public class VwStandardReferenceItemRepository : IVwStandardReferenceItemRepository
    {
        private readonly RepositoryContext _context;

        public VwStandardReferenceItemRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VwStandardReferenceItem>> GetAllAsync(string? standardReferenceInitial, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
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
                FROM [app].[vw_StandardReferenceItem] ";

            if (!string.IsNullOrEmpty(standardReferenceInitial))
            {
                sql += " WHERE StandardReferenceInitial = @Initial ";
            }

            sql += " ORDER BY StandardReferenceItemName ASC";

            return await connection.QueryAsync<VwStandardReferenceItem>(sql, new { Initial = standardReferenceInitial });
        }

        public async Task<VwStandardReferenceItem> GetOneAsync(long id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
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
                FROM [app].[vw_StandardReferenceItem]
                WHERE StandardReferenceItemId = @Id";

            return await connection.QuerySingleOrDefaultAsync<VwStandardReferenceItem>(sql, new { Id = id });
        }
    }
}
