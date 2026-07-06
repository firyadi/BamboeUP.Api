using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public partial interface IStandardReferenceItemDisplayService
    {
        /// <summary>
        /// Returns Standard Reference items resolved via the most specific scope
        /// (Company+Office > Company-only) or falls back to the global template.
        /// </summary>
        Task<IEnumerable<StandardReferenceItemDisplayDto>> GetItemsAsync(
            long companyId,
            long companyOfficeId,
            long? departmentId,
            string standardReferenceInitial);
    }
}
