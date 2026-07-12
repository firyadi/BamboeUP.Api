using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IStandardReferenceRepository
    {
        Task<StandardReference?> GetStandardReferenceAsync(Guid standardReferenceGuid, bool trackChanges);
        Task<IEnumerable<StandardReference>> GetAllStandardReferencesAsync(bool trackChanges);
        Task<IEnumerable<StandardReference>> GetStandardReferencesForParentSelectionAsync(Guid? currentRecordGuid, bool trackChanges);

        Task CreateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null);
        Task UpdateStandardReferenceAsync(StandardReference standardReference, IDbTransaction? transaction = null);
        Task DeleteStandardReferenceAsync(Guid standardReferenceGuid, IDbTransaction? transaction = null);
        Task SoftDeleteStandardReferenceAsync(StandardReference standardReference, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<StandardReference>> SearchStandardReferenceAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType,
string? standardReferenceName, string? standardReferenceNameSearchType,
string? note, string? noteSearchType,
Guid companyGuid, Guid companyOfficeGuid,
IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        
    }
}
