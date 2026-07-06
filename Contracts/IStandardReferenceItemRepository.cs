using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IStandardReferenceItemRepository
    {
        Task<StandardReferenceItem> GetStandardReferenceItemAsync(Guid standardReferenceItemGuid, bool trackChanges);
        Task<IEnumerable<StandardReferenceItem>> GetAllStandardReferenceItemsAsync(bool trackChanges);

        Task CreateStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, IDbTransaction? transaction = null);
        Task UpdateStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, IDbTransaction? transaction = null);
        Task DeleteStandardReferenceItemAsync(Guid standardReferenceItemGuid, IDbTransaction? transaction = null);
        Task SoftDeleteStandardReferenceItemAsync(StandardReferenceItem standardReferenceItem, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<StandardReferenceItem>> SearchStandardReferenceItemAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType,
string? standardReferenceItemInitial, string? standardReferenceItemInitialSearchType,
string? standardReferenceItemName, string? standardReferenceItemNameSearchType,
string? note, string? noteSearchType,
Guid companyGuid, Guid companyOfficeGuid,
IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        Task<IEnumerable<StandardReferenceItem>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid);
        Task<StandardReferenceItem> GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(Guid standardReferenceGuid, Guid standardReferenceItemGuid);
    }
}
