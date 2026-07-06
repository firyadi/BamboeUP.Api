using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects.Approval
{
    public record ApprovalRequestDto
    {
        public long ApprovalRequestId { get; set; }
        public Guid ApprovalRequestGuid { get; init; }
        public long ApprovalTemplateId { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public Guid ReferenceGuid { get; set; }
        public string? ReferenceNumber { get; set; }
        public long RequestedByUserId { get; set; }
        public int CurrentLevelOrder { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? Notes { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public byte[]? RowVersion { get; set; }

        public IEnumerable<ApprovalStepDto> Steps { get; set; } = new List<ApprovalStepDto>();
    }

    public record ApprovalRequestForCreationDto
    {
        public string ModuleCode { get; set; } = string.Empty;
        public Guid ReferenceGuid { get; set; }
        public string? ReferenceNumber { get; set; }
        public long RequestedByUserId { get; set; }
        public string? Notes { get; set; }
    }

    // DTO untuk aksi Approve / Reject dari atasan
    public record ApprovalRequestForActionDto
    {
        public long ActionByUserId { get; set; }
        public string? Comment { get; set; }
    }
}
