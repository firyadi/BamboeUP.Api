using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonWorkExperienceDto
    {
        public long PersonWorkExperienceId { get; set; }
        public Guid PersonWorkExperienceGuid { get; init; }
        public long PersonId { get; set; }
        public long? SrIndustry { get; set; }
        public long? SrEmploymentType { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Supervisor { get; set; }
        public string? JobDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEmployment { get; set; }
        public decimal? LastSalary { get; set; }
        public string? ReasonforLeaving { get; set; }
        public string? Remarks { get; set; }
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

    public partial record PersonWorkExperienceForCreationDto
    {
        public long PersonId { get; set; }
        public long? SrIndustry { get; set; }
        public long? SrEmploymentType { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Supervisor { get; set; }
        public string? JobDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEmployment { get; set; }
        public decimal? LastSalary { get; set; }
        public string? ReasonforLeaving { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonWorkExperienceForUpdateDto
    {
        public Guid PersonWorkExperienceGuid { get; set; }
        public long PersonId { get; set; }
        public long? SrIndustry { get; set; }
        public long? SrEmploymentType { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Supervisor { get; set; }
        public string? JobDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentEmployment { get; set; }
        public decimal? LastSalary { get; set; }
        public string? ReasonforLeaving { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PersonWorkExperienceForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonWorkExperienceSearchDto
    {
        public string? SrIndustry { get; set; }
        public SearchType SrIndustrySearchType { get; set; } = SearchType.Contains;

        public string? SrEmploymentType { get; set; }
        public SearchType SrEmploymentTypeSearchType { get; set; } = SearchType.Contains;

        public string? CompanyName { get; set; }
        public SearchType CompanyNameSearchType { get; set; } = SearchType.Contains;

        public string? JobTitle { get; set; }
        public SearchType JobTitleSearchType { get; set; } = SearchType.Contains;

        public string? Department { get; set; }
        public SearchType DepartmentSearchType { get; set; } = SearchType.Contains;

        public string? Location { get; set; }
        public SearchType LocationSearchType { get; set; } = SearchType.Contains;

        public string? Supervisor { get; set; }
        public SearchType SupervisorSearchType { get; set; } = SearchType.Contains;

        public string? JobDescription { get; set; }
        public SearchType JobDescriptionSearchType { get; set; } = SearchType.Contains;

        public string? StartDate { get; set; }
        public SearchType StartDateSearchType { get; set; } = SearchType.Contains;

        public string? EndDate { get; set; }
        public SearchType EndDateSearchType { get; set; } = SearchType.Contains;

        public string? IsCurrentEmployment { get; set; }
        public SearchType IsCurrentEmploymentSearchType { get; set; } = SearchType.Contains;

        public string? LastSalary { get; set; }
        public SearchType LastSalarySearchType { get; set; } = SearchType.Contains;

        public string? ReasonforLeaving { get; set; }
        public SearchType ReasonforLeavingSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
