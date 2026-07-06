using Shared.DataTransferObjects.Approval;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell.Approval
{
    public interface IApprovalService
    {
        Task<ApprovalRequestDto> SubmitRequestAsync(ApprovalRequestForCreationDto input);
        Task<ApprovalRequestDto> GetRequestAsync(Guid guid, bool trackChanges);
        Task<IEnumerable<ApprovalRequestDto>> GetMyPendingRequestsAsync(long userId, bool trackChanges);
        Task ApproveStepAsync(Guid requestGuid, Guid stepGuid, ApprovalRequestForActionDto input);
        Task RejectStepAsync(Guid requestGuid, Guid stepGuid, ApprovalRequestForActionDto input);
        Task CancelRequestAsync(Guid requestGuid, long cancelledByUserId);
        
        // Dipanggil oleh Hangfire Background Job
        Task CheckAndProcessSlaAsync();
    }
}
