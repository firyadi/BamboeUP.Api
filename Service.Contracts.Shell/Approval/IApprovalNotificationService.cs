using Entities.Models.Approval;
using System.Threading.Tasks;

namespace Service.Contracts.Shell.Approval
{
    public interface IApprovalNotificationService
    {
        Task SendApprovalRequiredEmailAsync(ApprovalRequest request, ApprovalStep step);
        Task SendApprovalApprovedEmailAsync(ApprovalRequest request);
        Task SendApprovalRejectedEmailAsync(ApprovalRequest request, ApprovalStep step);
        Task SendEscalationEmailAsync(ApprovalRequest request, ApprovalStep step);
    }
}
