using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects.Approval
{
    // ─── ApprovalTemplateLevelApprover DTOs ───────────────────────────
    public record ApprovalTemplateLevelApproverDto
    {
        public long ApprovalTemplateLevelApproverId { get; set; }
        public Guid ApprovalTemplateLevelApproverGuid { get; init; }
        public long ApprovalTemplateLevelId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
    }

    public record ApprovalTemplateLevelApproverForCreationDto
    {
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
    }

    // ─── ApprovalTemplateLevel DTOs ───────────────────────────────────
    public record ApprovalTemplateLevelDto
    {
        public long ApprovalTemplateLevelId { get; set; }
        public Guid ApprovalTemplateLevelGuid { get; init; }
        public long ApprovalTemplateId { get; set; }
        public int LevelOrder { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public string ApproverType { get; set; } = string.Empty;
        public bool RequireAllApprovers { get; set; }
        public bool CanSkipIfPreviousNotApproved { get; set; }
        public int SlaHours { get; set; }
        public int? EscalateToLevelOrder { get; set; }

        public IEnumerable<ApprovalTemplateLevelApproverDto> Approvers { get; set; } = new List<ApprovalTemplateLevelApproverDto>();
    }

    public record ApprovalTemplateLevelForCreationDto
    {
        public int LevelOrder { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public string ApproverType { get; set; } = string.Empty;
        public bool RequireAllApprovers { get; set; }
        public bool CanSkipIfPreviousNotApproved { get; set; }
        public int SlaHours { get; set; }
        public int? EscalateToLevelOrder { get; set; }

        public IEnumerable<ApprovalTemplateLevelApproverForCreationDto> Approvers { get; set; } = new List<ApprovalTemplateLevelApproverForCreationDto>();
    }

    // ─── ApprovalTemplate DTOs ────────────────────────────────────────
    public record ApprovalTemplateDto
    {
        public long ApprovalTemplateId { get; set; }
        public Guid ApprovalTemplateGuid { get; init; }
        public string TemplateName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<ApprovalTemplateLevelDto> Levels { get; set; } = new List<ApprovalTemplateLevelDto>();
    }

    public record ApprovalTemplateForCreationDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedById { get; set; }

        public IEnumerable<ApprovalTemplateLevelForCreationDto> Levels { get; set; } = new List<ApprovalTemplateLevelForCreationDto>();
    }

    public record ApprovalTemplateForUpdateDto
    {
        public string TemplateName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long? UpdatedById { get; set; }

        // Update levels diganti secara full batch untuk simplicity
        public IEnumerable<ApprovalTemplateLevelForCreationDto> Levels { get; set; } = new List<ApprovalTemplateLevelForCreationDto>();
    }
}
