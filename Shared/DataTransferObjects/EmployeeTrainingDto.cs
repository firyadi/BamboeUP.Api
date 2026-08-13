using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeTrainingDto
    {
        public long EmployeeTrainingId { get; set; }
        public Guid EmployeeTrainingGuid { get; init; }
        public long EmployeeId { get; set; }
        public long TrainingId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DurationHour { get; set; }
        public decimal? Score { get; set; }
        public string? Grade { get; set; }
        public bool IsPassed { get; set; }
        public string? CertificateNumber { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public Guid? FileStorageGuid { get; set; }
public string? TrainingName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeTrainingForCreationDto
    {
        public long EmployeeId { get; set; }
        public long TrainingId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DurationHour { get; set; }
        public decimal? Score { get; set; }
        public string? Grade { get; set; }
        public bool IsPassed { get; set; }
        public string? CertificateNumber { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeTrainingForUpdateDto
    {
        public Guid EmployeeTrainingGuid { get; set; }
        public long EmployeeId { get; set; }
        public long TrainingId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? DurationHour { get; set; }
        public decimal? Score { get; set; }
        public string? Grade { get; set; }
        public bool IsPassed { get; set; }
        public string? CertificateNumber { get; set; }
        public long? FileStorageId { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeTrainingForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeTrainingSearchDto
    {
        public string? StartDate { get; set; }
        public SearchType StartDateSearchType { get; set; } = SearchType.Contains;

        public string? EndDate { get; set; }
        public SearchType EndDateSearchType { get; set; } = SearchType.Contains;

        public string? DurationHour { get; set; }
        public SearchType DurationHourSearchType { get; set; } = SearchType.Contains;

        public string? Score { get; set; }
        public SearchType ScoreSearchType { get; set; } = SearchType.Contains;

        public string? Grade { get; set; }
        public SearchType GradeSearchType { get; set; } = SearchType.Contains;

        public string? IsPassed { get; set; }
        public SearchType IsPassedSearchType { get; set; } = SearchType.Contains;

        public string? CertificateNumber { get; set; }
        public SearchType CertificateNumberSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
