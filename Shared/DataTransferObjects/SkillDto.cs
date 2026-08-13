using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record SkillDto
    {
        public long SkillId { get; set; }
        public Guid SkillGuid { get; init; }
        
        public string SkillCode { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string? SkillShortName { get; set; }
        public long SrSkillCategory { get; set; }
        public long? SrSkillType { get; set; }
        public long? SrDefaultSkillLevel { get; set; }
        public bool IsCertificationRequired { get; set; }
        public bool IsMandatory { get; set; }
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

    public partial record SkillForCreationDto
    {
        public string SkillCode { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string? SkillShortName { get; set; }
        public long SrSkillCategory { get; set; }
        public long? SrSkillType { get; set; }
        public long? SrDefaultSkillLevel { get; set; }
        public bool IsCertificationRequired { get; set; }
        public bool IsMandatory { get; set; }
        public long? FileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record SkillForUpdateDto
    {
        public string SkillCode { get; set; } = string.Empty;
        public string SkillName { get; set; } = string.Empty;
        public string? SkillShortName { get; set; }
        public long SrSkillCategory { get; set; }
        public long? SrSkillType { get; set; }
        public long? SrDefaultSkillLevel { get; set; }
        public bool IsCertificationRequired { get; set; }
        public bool IsMandatory { get; set; }
        public long? FileStorageId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record SkillForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class SkillSearchDto
    {
        public string? SkillCode { get; set; }
        public SearchType SkillCodeSearchType { get; set; } = SearchType.Contains;

        public string? SkillName { get; set; }
        public SearchType SkillNameSearchType { get; set; } = SearchType.Contains;

        public string? SkillShortName { get; set; }
        public SearchType SkillShortNameSearchType { get; set; } = SearchType.Contains;

        public string? SrSkillCategory { get; set; }
        public SearchType SrSkillCategorySearchType { get; set; } = SearchType.Contains;

        public string? SrSkillType { get; set; }
        public SearchType SrSkillTypeSearchType { get; set; } = SearchType.Contains;

        public string? SrDefaultSkillLevel { get; set; }
        public SearchType SrDefaultSkillLevelSearchType { get; set; } = SearchType.Contains;

        public string? IsCertificationRequired { get; set; }
        public SearchType IsCertificationRequiredSearchType { get; set; } = SearchType.Contains;

        public string? IsMandatory { get; set; }
        public SearchType IsMandatorySearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public string? IsActive { get; set; }
        public SearchType IsActiveSearchType { get; set; } = SearchType.Contains;

    }
}
