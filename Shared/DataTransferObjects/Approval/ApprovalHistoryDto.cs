using System;

namespace Shared.DataTransferObjects.Approval
{
    public record ApprovalHistoryDto
    {
        public long ApprovalHistoryId { get; set; }
        public Guid ApprovalHistoryGuid { get; init; }
        public long ApprovalRequestId { get; set; }
        public long? ApprovalStepId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public long ActionByUserId { get; set; }
        public string? ActionByUserName { get; set; }
        public DateTime ActionTime { get; set; }
        public string? Comment { get; set; }
        public string? FromStatus { get; set; }
        public string? ToStatus { get; set; }
        public int? LevelOrder { get; set; }
    }
}
