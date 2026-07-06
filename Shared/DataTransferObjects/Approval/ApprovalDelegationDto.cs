using System;

namespace Shared.DataTransferObjects.Approval
{
    public record ApprovalDelegationDto
    {
        public long ApprovalDelegationId { get; set; }
        public Guid ApprovalDelegationGuid { get; init; }
        public long DelegatorUserId { get; set; }
        public string? DelegatorName { get; set; }
        public long DelegateUserId { get; set; }
        public string? DelegateName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record ApprovalDelegationForCreationDto
    {
        public long DelegatorUserId { get; set; }
        public long DelegateUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Notes { get; set; }
        public long CreatedById { get; set; }
    }

    public record ApprovalDelegationForUpdateDto
    {
        public long DelegateUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public long? UpdatedById { get; set; }
    }
}
