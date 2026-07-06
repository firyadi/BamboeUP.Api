using Contracts;
using Entities.Models.Approval;
using Service.Contracts.Shell.Approval;
using System.Threading.Tasks;

namespace Service.Shell.Approval
{
    public class ApprovalNotificationService : IApprovalNotificationService
    {
        private readonly ILoggerManager _logger;

        public ApprovalNotificationService(ILoggerManager logger)
        {
            _logger = logger;
        }

        public async Task SendApprovalRequiredEmailAsync(ApprovalRequest request, ApprovalStep step)
        {
            // Placeholder: Implement email sending logic using MailKit/SmtpClient
            // Example: "Dear {step.ApproverUserId}, you have a new request ({request.RequestNumber}) waiting for your approval."
            _logger.LogInfo($"MOCK EMAIL: Sent 'Approval Required' to UserID {step.ApproverUserId} for Request {request.RequestNumber}");
            await Task.CompletedTask;
        }

        public async Task SendApprovalApprovedEmailAsync(ApprovalRequest request)
        {
            _logger.LogInfo($"MOCK EMAIL: Sent 'Request Approved' to Requestor {request.RequestedByUserId} for Request {request.RequestNumber}");
            await Task.CompletedTask;
        }

        public async Task SendApprovalRejectedEmailAsync(ApprovalRequest request, ApprovalStep step)
        {
            _logger.LogInfo($"MOCK EMAIL: Sent 'Request Rejected' to Requestor {request.RequestedByUserId} for Request {request.RequestNumber}");
            await Task.CompletedTask;
        }

        public async Task SendEscalationEmailAsync(ApprovalRequest request, ApprovalStep step)
        {
            _logger.LogInfo($"MOCK EMAIL: Sent 'Step Escalated' for Request {request.RequestNumber} from Level {step.LevelOrder}");
            await Task.CompletedTask;
        }
    }
}
