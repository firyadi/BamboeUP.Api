using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CertificationDto
    {
        public long CertificationId { get; set; }
        public Guid CertificationGuid { get; init; }
        
        public string CertificationCode { get; set; } = string.Empty;
        public string CertificationName { get; set; } = string.Empty;
        public string? CertificationShortName { get; set; }
        public long SrCertificationCategory { get; set; }
        public long SrCertificationProvider { get; set; }
        public int? DefaultValidityMonth { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsRenewable { get; set; }
        public long? LogoFileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid? LogoFileStorageGuid { get; set; }
public string? CertificationCategoryName { get; set; }
public string? CertificationProviderName { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CertificationForCreationDto
    {
        public string CertificationCode { get; set; } = string.Empty;
        public string CertificationName { get; set; } = string.Empty;
        public string? CertificationShortName { get; set; }
        public long SrCertificationCategory { get; set; }
        public long SrCertificationProvider { get; set; }
        public int? DefaultValidityMonth { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsRenewable { get; set; }
        public long? LogoFileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record CertificationForUpdateDto
    {
        public string CertificationCode { get; set; } = string.Empty;
        public string CertificationName { get; set; } = string.Empty;
        public string? CertificationShortName { get; set; }
        public long SrCertificationCategory { get; set; }
        public long SrCertificationProvider { get; set; }
        public int? DefaultValidityMonth { get; set; }
        public bool IsLifetime { get; set; }
        public bool IsRenewable { get; set; }
        public long? LogoFileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record CertificationForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class CertificationSearchDto
    {
        public string? CertificationCode { get; set; }
        public SearchType CertificationCodeSearchType { get; set; } = SearchType.Contains;

        public string? CertificationName { get; set; }
        public SearchType CertificationNameSearchType { get; set; } = SearchType.Contains;

        public string? CertificationShortName { get; set; }
        public SearchType CertificationShortNameSearchType { get; set; } = SearchType.Contains;

        public string? DefaultValidityMonth { get; set; }
        public SearchType DefaultValidityMonthSearchType { get; set; } = SearchType.Contains;

        public string? IsLifetime { get; set; }
        public SearchType IsLifetimeSearchType { get; set; } = SearchType.Contains;

        public string? IsRenewable { get; set; }
        public SearchType IsRenewableSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public string? IsActive { get; set; }
        public SearchType IsActiveSearchType { get; set; } = SearchType.Contains;
public string? CertificationCategoryName { get; set; }
public string? CertificationProviderName { get; set; }
public SearchType CertificationCategoryNameSearchType { get; set; } = SearchType.Contains;
public SearchType CertificationProviderNameSearchType { get; set; } = SearchType.Contains;

    }
}
