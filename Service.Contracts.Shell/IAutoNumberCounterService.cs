using Shared.DataTransferObjects;

namespace Service.Contracts.Shell;

public interface IAutoNumberCounterService
{
    Task<IEnumerable<AutoNumberCounterDto>> GetAllAutoNumberCountersAsync(bool trackChanges);
    Task<AutoNumberCounterDto> GetAutoNumberCounterByGuidAsync(Guid autoNumberCounterGuid, bool trackChanges);
    Task<AutoNumberCounterDto> CreateAutoNumberCounterAsync(AutoNumberCounterForCreationDto input);
    Task UpdateAutoNumberCounterAsync(Guid autoNumberCounterGuid, AutoNumberCounterForUpdateDto input, bool trackChanges);
    Task DeleteAutoNumberCounterAsync(Guid autoNumberCounterGuid, AutoNumberCounterForDeleteDto input, bool trackChanges);
    Task DeleteAutoNumberCounterByAdminAsync(Guid autoNumberCounterGuid, bool trackChanges);

    Task<IEnumerable<AutoNumberCounterDto>> SearchAutoNumberCounterAsync(
        long? autoNumberTemplateId,
        long? companyId,
        long? companyOfficeId,
        int? organizationUnitId,
        int? yearNo,
        int? monthNo,
        int? dayNo
    );

    // Detail helpers — scoped by parent AutoNumberTemplateGuid
    Task<IEnumerable<AutoNumberCounterDto>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid);
}
