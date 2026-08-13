using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record TrainingDto
    {
        public long TrainingId { get; set; }
        public Guid TrainingGuid { get; init; }
        
        public string TrainingCode { get; set; } = string.Empty;
        public string TrainingName { get; set; } = string.Empty;
        public string? TrainingShortName { get; set; }
        public long SrTrainingCategory { get; set; }
        public long SrTrainingType { get; set; }
        public long? SrTrainingLevel { get; set; }
        public decimal? DurationHour { get; set; }
        public int? ValidityMonth { get; set; }
        public long? SrTrainingProvider { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsCertification { get; set; }
        public bool IsExternal { get; set; }
        public long? FileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid? FileStorageGuid { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record TrainingForCreationDto
    {
        public string TrainingCode { get; set; } = string.Empty;
        public string TrainingName { get; set; } = string.Empty;
        public string? TrainingShortName { get; set; }
        public long SrTrainingCategory { get; set; }
        public long SrTrainingType { get; set; }
        public long? SrTrainingLevel { get; set; }
        public decimal? DurationHour { get; set; }
        public int? ValidityMonth { get; set; }
        public long? SrTrainingProvider { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsCertification { get; set; }
        public bool IsExternal { get; set; }
        public long? FileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record TrainingForUpdateDto
    {
        public string TrainingCode { get; set; } = string.Empty;
        public string TrainingName { get; set; } = string.Empty;
        public string? TrainingShortName { get; set; }
        public long SrTrainingCategory { get; set; }
        public long SrTrainingType { get; set; }
        public long? SrTrainingLevel { get; set; }
        public decimal? DurationHour { get; set; }
        public int? ValidityMonth { get; set; }
        public long? SrTrainingProvider { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsCertification { get; set; }
        public bool IsExternal { get; set; }
        public long? FileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record TrainingForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class TrainingSearchDto
    {
        public string? TrainingCode { get; set; }
        public SearchType TrainingCodeSearchType { get; set; } = SearchType.Contains;

        public string? TrainingName { get; set; }
        public SearchType TrainingNameSearchType { get; set; } = SearchType.Contains;

        public string? TrainingShortName { get; set; }
        public SearchType TrainingShortNameSearchType { get; set; } = SearchType.Contains;

        public string? SrTrainingCategory { get; set; }
        public SearchType SrTrainingCategorySearchType { get; set; } = SearchType.Contains;

        public string? SrTrainingType { get; set; }
        public SearchType SrTrainingTypeSearchType { get; set; } = SearchType.Contains;

        public string? SrTrainingLevel { get; set; }
        public SearchType SrTrainingLevelSearchType { get; set; } = SearchType.Contains;

        public string? DurationHour { get; set; }
        public SearchType DurationHourSearchType { get; set; } = SearchType.Contains;

        public string? ValidityMonth { get; set; }
        public SearchType ValidityMonthSearchType { get; set; } = SearchType.Contains;

        public string? SrTrainingProvider { get; set; }
        public SearchType SrTrainingProviderSearchType { get; set; } = SearchType.Contains;

        public string? IsMandatory { get; set; }
        public SearchType IsMandatorySearchType { get; set; } = SearchType.Contains;

        public string? IsCertification { get; set; }
        public SearchType IsCertificationSearchType { get; set; } = SearchType.Contains;

        public string? IsExternal { get; set; }
        public SearchType IsExternalSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public string? IsActive { get; set; }
        public SearchType IsActiveSearchType { get; set; } = SearchType.Contains;

    }
}
