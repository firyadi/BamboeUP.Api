using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PositionDto
    {
        public long PositionId { get; set; }
        public Guid PositionGuid { get; init; }
        
        public string PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public long PositionGradeId { get; set; }
        public long PositionLevelId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? PositionDescription { get; set; }
public string? PositionGradeName { get; set; }
public string? PositionLevelName { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PositionForCreationDto
    {
        public string PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public long PositionGradeId { get; set; }
        public long PositionLevelId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? PositionDescription { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PositionForUpdateDto
    {
        public string PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public long PositionGradeId { get; set; }
        public long PositionLevelId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? PositionDescription { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PositionForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PositionSearchDto
    {
        public string? PositionCode { get; set; }
        public SearchType PositionCodeSearchType { get; set; } = SearchType.Contains;

        public string? PositionName { get; set; }
        public SearchType PositionNameSearchType { get; set; } = SearchType.Contains;

        public string? Summary { get; set; }
        public SearchType SummarySearchType { get; set; } = SearchType.Contains;

        public string? ValidFrom { get; set; }
        public SearchType ValidFromSearchType { get; set; } = SearchType.Contains;

        public string? Validto { get; set; }
        public SearchType ValidtoSearchType { get; set; } = SearchType.Contains;

        public string? PositionDescription { get; set; }
        public SearchType PositionDescriptionSearchType { get; set; } = SearchType.Contains;
public string? PositionGradeName { get; set; }
public string? PositionLevelName { get; set; }
public SearchType PositionGradeNameSearchType { get; set; } = SearchType.Contains;
public SearchType PositionLevelNameSearchType { get; set; } = SearchType.Contains;

    }
}
