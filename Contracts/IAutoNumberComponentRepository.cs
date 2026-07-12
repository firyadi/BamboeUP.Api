using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IAutoNumberComponentRepository
    {
        Task<AutoNumberComponent?> GetAutoNumberComponentAsync(Guid autoNumberComponentGuid, bool trackChanges);
        Task<IEnumerable<AutoNumberComponent>> GetAllAutoNumberComponentsAsync(bool trackChanges);

        Task CreateAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, IDbTransaction? transaction = null);
        Task UpdateAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, IDbTransaction? transaction = null);
        Task DeleteAutoNumberComponentAsync(Guid autoNumberComponentGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAutoNumberComponentAsync(AutoNumberComponent autoNumberComponent, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<AutoNumberComponent>> SearchAutoNumberComponentAsync(
            string? componentType, string? componentTypeSearchType,
            string? staticValue, string? staticValueSearchType,
            string? format, string? formatSearchType,
            long? autoNumberTemplateId,
            IDbTransaction? transaction = null);

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        Task<IEnumerable<AutoNumberComponent>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges);
    }
}
