using Shared.DataTransferObjects;

namespace Service.Contracts.Shell;

public interface IAutoNumberTemplateService
{
    Task<IEnumerable<AutoNumberTemplateDto>> GetAllAutoNumberTemplatesAsync(bool trackChanges);
    Task<AutoNumberTemplateDto> GetAutoNumberTemplateByGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges);
    Task<AutoNumberTemplateDto> CreateAutoNumberTemplateAsync(AutoNumberTemplateForCreationDto input);
    Task UpdateAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, AutoNumberTemplateForUpdateDto input, bool trackChanges);
    Task DeleteAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, AutoNumberTemplateForDeleteDto input, bool trackChanges);
    Task DeleteAutoNumberTemplateByAdminAsync(Guid autoNumberTemplateGuid, bool trackChanges);

    Task<IEnumerable<AutoNumberTemplateDto>> SearchAutoNumberTemplateAsync(
        string? templateName, string? templateNameSearchType,
        string? description, string? descriptionSearchType,
        DateTime? effectiveDateFrom, DateTime? effectiveDateTo,
        string? templateScopeType,
        long? companyId,
        long? companyOfficeId
    );
}
