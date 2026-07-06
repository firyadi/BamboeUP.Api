using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IAutoNumberTemplateRepository
    {
        Task<AutoNumberTemplate> GetAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, bool trackChanges);
        Task<IEnumerable<AutoNumberTemplate>> GetAllAutoNumberTemplatesAsync(bool trackChanges);

        Task CreateAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, IDbTransaction? transaction = null);
        Task UpdateAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, IDbTransaction? transaction = null);
        Task DeleteAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAutoNumberTemplateAsync(AutoNumberTemplate autoNumberTemplate, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<AutoNumberTemplate>> SearchAutoNumberTemplateAsync(
            string? templateName, string? templateNameSearchType,
            string? description, string? descriptionSearchType,
            DateTime? effectiveDateFrom, DateTime? effectiveDateTo,
            string? templateScopeType,
            long? companyId,
            long? companyOfficeId,
            IDbTransaction? transaction = null);
    }
}
