using System;

namespace Shared.DataTransferObjects.Approval
{
    public record ApprovalStepDto
    {
        public long ApprovalStepId { get; set; }
        public Guid ApprovalStepGuid { get; init; }
        public long ApprovalRequestId { get; set; }
        public int LevelOrder { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public long ApproverUserId { get; set; }
        public string? ApproverName { get; set; } // Tambahan untuk View
        public long? DelegatedFromUserId { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public DateTime? ActionTime { get; set; }
        public string? Comment { get; set; }
        public DateTime? SlaDeadline { get; set; }
        public bool IsEscalated { get; set; }
    }
}
