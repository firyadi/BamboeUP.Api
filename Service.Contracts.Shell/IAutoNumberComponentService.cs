using Shared.DataTransferObjects;

namespace Service.Contracts.Shell;

public interface IAutoNumberComponentService
{
    Task<IEnumerable<AutoNumberComponentDto>> GetAllAutoNumberComponentsAsync(bool trackChanges);
    Task<AutoNumberComponentDto> GetAutoNumberComponentByGuidAsync(Guid autoNumberComponentGuid, bool trackChanges);
    Task<AutoNumberComponentDto> CreateAutoNumberComponentAsync(AutoNumberComponentForCreationDto input);
    Task UpdateAutoNumberComponentAsync(Guid autoNumberComponentGuid, AutoNumberComponentForUpdateDto input, bool trackChanges);
    Task DeleteAutoNumberComponentAsync(Guid autoNumberComponentGuid, AutoNumberComponentForDeleteDto input, bool trackChanges);
    Task DeleteAutoNumberComponentByAdminAsync(Guid autoNumberComponentGuid, bool trackChanges);

    Task<IEnumerable<AutoNumberComponentDto>> SearchAutoNumberComponentAsync(
        string? componentType, string? componentTypeSearchType,
        string? staticValue, string? staticValueSearchType,
        string? format, string? formatSearchType,
        long? autoNumberTemplateId
    );

    // Detail helpers — scoped by parent AutoNumberTemplateGuid
    Task<IEnumerable<AutoNumberComponentDto>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid);
}
