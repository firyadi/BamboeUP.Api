using Shared.DataTransferObjects.Approval;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell.Approval
{
    public interface IApprovalDelegationService
    {
        Task<IEnumerable<ApprovalDelegationDto>> GetMyDelegationsAsync(long userId, bool trackChanges);
        Task<ApprovalDelegationDto> GetAsync(Guid guid, bool trackChanges);
        Task<ApprovalDelegationDto> CreateAsync(ApprovalDelegationForCreationDto input);
        Task UpdateAsync(Guid guid, ApprovalDelegationForUpdateDto input, bool trackChanges);
        Task DeleteAsync(Guid guid, bool trackChanges);
    }
}
