using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeSkillDto
    {
        public long EmployeeSkillId { get; set; }
        public Guid EmployeeSkillGuid { get; init; }
        public long EmployeeId { get; set; }
        public long SkillId { get; set; }
        public long? SrSkillLevel { get; set; }
        public decimal? YearsofExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsCertified { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? CertificateIssueDate { get; set; }
        public DateTime? CertificateExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public Guid? FileStorageGuid { get; set; }
public string? SkillName { get; set; }
public string? SkillLevelName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeSkillForCreationDto
    {
        public long EmployeeId { get; set; }
        public long SkillId { get; set; }
        public long? SrSkillLevel { get; set; }
        public decimal? YearsofExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsCertified { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? CertificateIssueDate { get; set; }
        public DateTime? CertificateExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeSkillForUpdateDto
    {
        public Guid EmployeeSkillGuid { get; set; }
        public long EmployeeId { get; set; }
        public long SkillId { get; set; }
        public long? SrSkillLevel { get; set; }
        public decimal? YearsofExperience { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsCertified { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? CertificateIssueDate { get; set; }
        public DateTime? CertificateExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeSkillForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeSkillSearchDto
    {
        public string? YearsofExperience { get; set; }
        public SearchType YearsofExperienceSearchType { get; set; } = SearchType.Contains;

        public string? LastUsedDate { get; set; }
        public SearchType LastUsedDateSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;

        public string? IsCertified { get; set; }
        public SearchType IsCertifiedSearchType { get; set; } = SearchType.Contains;

        public string? CertificateNumber { get; set; }
        public SearchType CertificateNumberSearchType { get; set; } = SearchType.Contains;

        public string? CertificateIssueDate { get; set; }
        public SearchType CertificateIssueDateSearchType { get; set; } = SearchType.Contains;

        public string? CertificateExpiredDate { get; set; }
        public SearchType CertificateExpiredDateSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
