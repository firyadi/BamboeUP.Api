using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeLanguageDto
    {
        public long EmployeeLanguageId { get; set; }
        public Guid EmployeeLanguageGuid { get; init; }
        public long EmployeeId { get; set; }
        public long SrLanguage { get; set; }
        public long? SrReadingLevel { get; set; }
        public long? SrWritingLevel { get; set; }
        public long? SrListeningLevel { get; set; }
        public long? SrSpeakingLevel { get; set; }
        public string? CertificationName { get; set; }
        public decimal? Score { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsPrimary { get; set; }
        public string? Remarks { get; set; }
        public Guid? FileStorageGuid { get; set; }
public string? LanguageName { get; set; }
public string? ListeningLevelName { get; set; }
public string? ReadingLevelName { get; set; }
public string? SpeakingLevelName { get; set; }
public string? WritingLevelName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeLanguageForCreationDto
    {
        public long EmployeeId { get; set; }
        public long SrLanguage { get; set; }
        public long? SrReadingLevel { get; set; }
        public long? SrWritingLevel { get; set; }
        public long? SrListeningLevel { get; set; }
        public long? SrSpeakingLevel { get; set; }
        public string? CertificationName { get; set; }
        public decimal? Score { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsPrimary { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeLanguageForUpdateDto
    {
        public Guid EmployeeLanguageGuid { get; set; }
        public long EmployeeId { get; set; }
        public long SrLanguage { get; set; }
        public long? SrReadingLevel { get; set; }
        public long? SrWritingLevel { get; set; }
        public long? SrListeningLevel { get; set; }
        public long? SrSpeakingLevel { get; set; }
        public string? CertificationName { get; set; }
        public decimal? Score { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long? FileStorageId { get; set; }
        public bool IsPrimary { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeLanguageForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeLanguageSearchDto
    {
        public string? CertificationName { get; set; }
        public SearchType CertificationNameSearchType { get; set; } = SearchType.Contains;

        public string? Score { get; set; }
        public SearchType ScoreSearchType { get; set; } = SearchType.Contains;

        public string? IssuedDate { get; set; }
        public SearchType IssuedDateSearchType { get; set; } = SearchType.Contains;

        public string? ExpiredDate { get; set; }
        public SearchType ExpiredDateSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
