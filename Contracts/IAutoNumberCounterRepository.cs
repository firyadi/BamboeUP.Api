using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IAutoNumberCounterRepository
    {
        Task<AutoNumberCounter?> GetAutoNumberCounterAsync(Guid autoNumberCounterGuid, bool trackChanges);
        Task<IEnumerable<AutoNumberCounter>> GetAllAutoNumberCountersAsync(bool trackChanges);

        Task CreateAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, IDbTransaction? transaction = null);
        Task UpdateAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, IDbTransaction? transaction = null);
        Task DeleteAutoNumberCounterAsync(Guid autoNumberCounterGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<AutoNumberCounter>> SearchAutoNumberCounterAsync(
            long? autoNumberTemplateId,
            long? companyId,
            long? companyOfficeId,
            int? organizationUnitId,
            int? yearNo,
            int? monthNo,
            int? dayNo,
            IDbTransaction? transaction = null);

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        Task<IEnumerable<AutoNumberCounter>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges);
    }
}
