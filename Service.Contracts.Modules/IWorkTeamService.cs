using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IWorkTeamService
    {
        Task<IEnumerable<WorkTeamDto>> GetAllWorkTeamsAsync(bool trackChanges);
        Task<WorkTeamDto?> GetWorkTeamByGuidAsync(Guid workTeamGuid, bool trackChanges);
        Task<WorkTeamDto> CreateWorkTeamAsync(WorkTeamForCreationDto input);
        Task UpdateWorkTeamAsync(Guid workTeamGuid, WorkTeamForUpdateDto input, bool trackChanges);
        Task DeleteWorkTeamAsync(Guid workTeamGuid, WorkTeamForDeleteDto input, bool trackChanges);
        Task DeleteWorkTeamByAdminAsync(Guid workTeamGuid, bool trackChanges);

        Task<IEnumerable<WorkTeamDto>> SearchWorkTeamAsync(
            string? workTeamCode, string? workTeamCodeSearchType, string? workTeamName, string? workTeamNameSearchType, string? isEditAble, string? isEditAbleSearchType, string? note, string? noteSearchType

        );
    }
}
