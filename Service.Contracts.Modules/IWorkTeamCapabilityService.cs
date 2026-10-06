using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IWorkTeamCapabilityService
    {
        Task<IEnumerable<WorkTeamCapabilityDto>> GetAllWorkTeamCapabilitiesAsync(bool trackChanges);
        Task<WorkTeamCapabilityDto?> GetWorkTeamCapabilityByGuidAsync(Guid workTeamCapabilityGuid, bool trackChanges);
        Task<WorkTeamCapabilityDto> CreateWorkTeamCapabilityAsync(Guid workTeamGuid, WorkTeamCapabilityForCreationDto input);
        Task UpdateWorkTeamCapabilityAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid, WorkTeamCapabilityForUpdateDto input, bool trackChanges);
        Task DeleteWorkTeamCapabilityAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid, WorkTeamCapabilityForDeleteDto input, bool trackChanges);
        Task DeleteWorkTeamCapabilityByAdminAsync(Guid workTeamCapabilityGuid, bool trackChanges);

        Task<IEnumerable<WorkTeamCapabilityDto>> SearchWorkTeamCapabilityAsync(
            string? isViewAble, string? isViewAbleSearchType, string? isAddAble, string? isAddAbleSearchType, string? isEditAble, string? isEditAbleSearchType, string? isApproveAble, string? isApproveAbleSearchType, string? isExportAble, string? isExportAbleSearchType,
            Guid workTeamGuid, Guid workTeamCapabilityGuid);

        // Detail (child) helpers
        Task<IEnumerable<WorkTeamCapabilityDto>> GetAllByWorkTeamGuidAsync(Guid workTeamGuid);
        Task<WorkTeamCapabilityDto?> GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid);
        Task<IEnumerable<WorkCapabilityDto>> GetWorkCapabilitiesAsync();
    }
}
