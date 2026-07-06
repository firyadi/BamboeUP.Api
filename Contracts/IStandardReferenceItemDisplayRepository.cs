using Entities.Models;

namespace Contracts
{
    public partial interface IStandardReferenceItemDisplayRepository
    {
        /// <summary>
        /// Calls app.fn_GetStandardReferenceItems and returns items ordered by DisplayOrder.
        /// Resolves the most specific scope (Company+Office > Company-only) before falling back to the global template.
        /// </summary>
        Task<IEnumerable<StandardReferenceItemDisplay>> GetItemsAsync(
            long companyId,
            long companyOfficeId,
            long? departmentId,
            string standardReferenceInitial);
    }
}
