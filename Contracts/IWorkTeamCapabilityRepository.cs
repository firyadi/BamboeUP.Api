using Entities.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IWorkTeamCapabilityRepository
    {
        Task<WorkTeamCapability?> GetWorkTeamCapabilityAsync(Guid workTeamCapabilityGuid, bool trackChanges);
        Task<WorkTeamCapability?> GetByWorkTeamGuidAndWorkTeamCapabilityGuidAsync(Guid workTeamGuid, Guid workTeamCapabilityGuid);
        Task<IEnumerable<WorkTeamCapability>> GetAllByWorkTeamGuidAsync(Guid workTeamGuid);
        Task<IEnumerable<WorkCapabilityDto>> GetWorkCapabilitiesAsync();

        Task CreateWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, IDbTransaction? transaction = null);
        Task UpdateWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, IDbTransaction? transaction = null);
        Task SoftDeleteWorkTeamCapabilityAsync(WorkTeamCapability workTeamCapability, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteWorkTeamCapabilityAsync(Guid workTeamCapabilityGuid, IDbTransaction? transaction = null);
        Task DeleteByWorkTeamGuidAsync(Guid workTeamGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<WorkTeamCapability>> SearchWorkTeamCapabilityAsync(
            string? isViewAble,
            string? isViewAbleSearchType,
            string? isAddAble,
            string? isAddAbleSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? isApproveAble,
            string? isApproveAbleSearchType,
            string? isExportAble,
            string? isExportAbleSearchType,
            Guid workTeamGuid, Guid workTeamCapabilityGuid,
            IDbTransaction? transaction = null);
    }
}
