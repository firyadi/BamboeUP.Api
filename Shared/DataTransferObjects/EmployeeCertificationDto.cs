using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeCertificationDto
    {
        public long EmployeeCertificationId { get; set; }
        public Guid EmployeeCertificationGuid { get; init; }
        public long EmployeeId { get; set; }
        public long CertificationId { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string? Issuedby { get; set; }
        public decimal? Score { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsCurrent { get; set; }
        public string? Remarks { get; set; }
        public Guid? FileStorageGuid { get; set; }
public string? CertificationName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeCertificationForCreationDto
    {
        public long EmployeeId { get; set; }
        public long CertificationId { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string? Issuedby { get; set; }
        public decimal? Score { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsCurrent { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeCertificationForUpdateDto
    {
        public Guid EmployeeCertificationGuid { get; set; }
        public long EmployeeId { get; set; }
        public long CertificationId { get; set; }
        public string? CertificateNumber { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string? Issuedby { get; set; }
        public decimal? Score { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsCurrent { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeCertificationForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeCertificationSearchDto
    {
        public string? CertificateNumber { get; set; }
        public SearchType CertificateNumberSearchType { get; set; } = SearchType.Contains;

        public string? IssuedDate { get; set; }
        public SearchType IssuedDateSearchType { get; set; } = SearchType.Contains;

        public string? ExpiredDate { get; set; }
        public SearchType ExpiredDateSearchType { get; set; } = SearchType.Contains;

        public string? Issuedby { get; set; }
        public SearchType IssuedbySearchType { get; set; } = SearchType.Contains;

        public string? Score { get; set; }
        public SearchType ScoreSearchType { get; set; } = SearchType.Contains;

        public string? IsLifetime { get; set; }
        public SearchType IsLifetimeSearchType { get; set; } = SearchType.Contains;

        public string? IsCurrent { get; set; }
        public SearchType IsCurrentSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
