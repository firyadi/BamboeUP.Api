using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record OvertimeRequestDto
    {
        public long OvertimeRequestId { get; set; }
        public Guid OvertimeRequestGuid { get; init; }
        
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime OvertimeDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? WorkDescription { get; set; }
        public string PlannedStartTime { get; set; } = string.Empty;
        public string PlannedEndTime { get; set; } = string.Empty;
        public int? TotalPlannedMinutes { get; set; }
        public string? ActualStartTime { get; set; }
        public string? ActualEndTime { get; set; }
        public int? TotalActualMinutes { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string? RejectionReason { get; set; }
        public string? Note { get; set; }
public string? ApprovedbyName { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record OvertimeRequestForCreationDto
    {
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime OvertimeDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? WorkDescription { get; set; }
        public string PlannedStartTime { get; set; } = string.Empty;
        public string PlannedEndTime { get; set; } = string.Empty;
        public int? TotalPlannedMinutes { get; set; }
        public string? ActualStartTime { get; set; }
        public string? ActualEndTime { get; set; }
        public int? TotalActualMinutes { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string? RejectionReason { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record OvertimeRequestForUpdateDto
    {
        public long CompanyId { get; set; }
        public long CompanyOfficeId { get; set; }
        public long PersonId { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime OvertimeDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? WorkDescription { get; set; }
        public string PlannedStartTime { get; set; } = string.Empty;
        public string PlannedEndTime { get; set; } = string.Empty;
        public int? TotalPlannedMinutes { get; set; }
        public string? ActualStartTime { get; set; }
        public string? ActualEndTime { get; set; }
        public int? TotalActualMinutes { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string? RejectionReason { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record OvertimeRequestForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class OvertimeRequestSearchDto
    {
        public string? RequestNo { get; set; }
        public SearchType RequestNoSearchType { get; set; } = SearchType.Contains;

        public string? RequestDate { get; set; }
        public SearchType RequestDateSearchType { get; set; } = SearchType.Contains;

        public string? OvertimeDate { get; set; }
        public SearchType OvertimeDateSearchType { get; set; } = SearchType.Contains;

        public string? Reason { get; set; }
        public SearchType ReasonSearchType { get; set; } = SearchType.Contains;

        public string? WorkDescription { get; set; }
        public SearchType WorkDescriptionSearchType { get; set; } = SearchType.Contains;

        public string? PlannedStartTime { get; set; }
        public SearchType PlannedStartTimeSearchType { get; set; } = SearchType.Contains;

        public string? PlannedEndTime { get; set; }
        public SearchType PlannedEndTimeSearchType { get; set; } = SearchType.Contains;

        public string? TotalPlannedMinutes { get; set; }
        public SearchType TotalPlannedMinutesSearchType { get; set; } = SearchType.Contains;

        public string? ActualStartTime { get; set; }
        public SearchType ActualStartTimeSearchType { get; set; } = SearchType.Contains;

        public string? ActualEndTime { get; set; }
        public SearchType ActualEndTimeSearchType { get; set; } = SearchType.Contains;

        public string? TotalActualMinutes { get; set; }
        public SearchType TotalActualMinutesSearchType { get; set; } = SearchType.Contains;

        public string? ApprovedTime { get; set; }
        public SearchType ApprovedTimeSearchType { get; set; } = SearchType.Contains;

        public string? RejectionReason { get; set; }
        public SearchType RejectionReasonSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
public string? ApprovedbyName { get; set; }
public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }
public string? PersonName { get; set; }
public SearchType ApprovedbyNameSearchType { get; set; } = SearchType.Contains;
public SearchType CompanyNameSearchType { get; set; } = SearchType.Contains;
public SearchType CompanyOfficeNameSearchType { get; set; } = SearchType.Contains;
public SearchType PersonNameSearchType { get; set; } = SearchType.Contains;

    }
}
