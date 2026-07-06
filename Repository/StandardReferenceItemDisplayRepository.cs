using Contracts;
using Dapper;
using Entities.Models;

namespace Repository
{
    public partial class StandardReferenceItemDisplayRepository : IStandardReferenceItemDisplayRepository
    {
        private readonly RepositoryContext _context;

        public StandardReferenceItemDisplayRepository(RepositoryContext context)
            => _context = context;

        /// <inheritdoc />
        public async Task<IEnumerable<StandardReferenceItemDisplay>> GetItemsAsync(
            long companyId,
            long companyOfficeId,
            long? departmentId,
            string standardReferenceInitial)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
                SELECT
                    StandardReferenceId,
                    StandardReferenceGuid,
                    StandardReferenceInitial,
                    StandardReferenceName,
                    StandardReferenceItemId,
                    StandardReferenceItemGuid,
                    StandardReferenceItemInitial,
                    StandardReferenceItemName,
                    StandardReferenceItemValue,
                    DisplayOrder,
                    DataSource
                FROM [app].[fn_GetStandardReferenceItems]
                (
                    @CompanyId,
                    @CompanyOfficeId,
                    @DepartmentId,
                    @StandardReferenceInitial
                )
                ORDER BY DisplayOrder";

            return await connection.QueryAsync<StandardReferenceItemDisplay>(sql, new
            {
                CompanyId                 = companyId,
                CompanyOfficeId           = companyOfficeId,
                DepartmentId              = departmentId,
                StandardReferenceInitial  = standardReferenceInitial
            });
        }
    }
}
