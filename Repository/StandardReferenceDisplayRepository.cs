using Contracts;
using Dapper;
using Entities.Models;

using Repository.Database;
namespace Repository
{
    public class StandardReferenceDisplayRepository : IStandardReferenceDisplayRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public StandardReferenceDisplayRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<IEnumerable<StandardReferenceDisplay>> GetAllDisplaysAsync(Shared.RequestFeatures.StandardReferenceDisplayParameters parameters, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT StandardReferenceItemId, StandardReferenceItemName,
                       CompanyId, CompanyOfficeId, StandardReferenceGroupId, StandardReferenceInitial
                FROM {_dialect.Qualify("app", "vw_StandardReference_Display")}
                WHERE StandardReferenceInitial = @Initial
                  AND (
                        (CompanyId = @CompanyId AND CompanyOfficeId = @CompanyOfficeId)
                        OR (CompanyId = @CompanyId AND CompanyOfficeId = 0)
                        OR (CompanyId = 0 AND CompanyOfficeId = 0)
                      )
                  AND (@GroupId IS NULL OR StandardReferenceGroupId = @GroupId)
                ORDER BY StandardReferenceItemName ASC";

            var dapperParams = new
            {
                Initial = parameters.StandardReferenceInitial,
                CompanyId = parameters.CompanyId,
                CompanyOfficeId = parameters.CompanyOfficeId,
                GroupId = parameters.StandardReferenceGroupId
            };

            return await connection.QueryAsync<StandardReferenceDisplay>(sql, dapperParams);
        }

        public async Task<StandardReferenceDisplay?> GetOneAsync(long id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT StandardReferenceItemId, StandardReferenceItemName
                FROM {_dialect.Qualify("app", "vw_StandardReference_Display")}
                WHERE StandardReferenceItemId = @Id";

            return await connection.QuerySingleOrDefaultAsync<StandardReferenceDisplay>(sql, new { Id = id });
        }
    }
}
