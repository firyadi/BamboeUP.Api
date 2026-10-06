using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PositionGradeDto
    {
        public long PositionGradeId { get; set; }
        public Guid PositionGradeGuid { get; init; }
        
        public string PositionGradeCode { get; set; } = string.Empty;
        public string PositionGradeName { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionGradeDescription { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PositionGradeForCreationDto
    {
        public string PositionGradeCode { get; set; } = string.Empty;
        public string PositionGradeName { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionGradeDescription { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PositionGradeForUpdateDto
    {
        public string PositionGradeCode { get; set; } = string.Empty;
        public string PositionGradeName { get; set; } = string.Empty;
        public string Interval { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionGradeDescription { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PositionGradeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PositionGradeSearchDto
    {
        public string? PositionGradeCode { get; set; }
        public SearchType PositionGradeCodeSearchType { get; set; } = SearchType.Contains;

        public string? PositionGradeName { get; set; }
        public SearchType PositionGradeNameSearchType { get; set; } = SearchType.Contains;

        public string? Interval { get; set; }
        public SearchType IntervalSearchType { get; set; } = SearchType.Contains;

        public string? Ranking { get; set; }
        public SearchType RankingSearchType { get; set; } = SearchType.Contains;

        public string? PositionGradeDescription { get; set; }
        public SearchType PositionGradeDescriptionSearchType { get; set; } = SearchType.Contains;

    }
}
