using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PositionLevelDto
    {
        public long PositionLevelId { get; set; }
        public Guid PositionLevelGuid { get; init; }
        
        public string PositionLevelCode { get; set; } = string.Empty;
        public string PositionLevelName { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionLevelDescription { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PositionLevelForCreationDto
    {
        public string PositionLevelCode { get; set; } = string.Empty;
        public string PositionLevelName { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionLevelDescription { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PositionLevelForUpdateDto
    {
        public string PositionLevelCode { get; set; } = string.Empty;
        public string PositionLevelName { get; set; } = string.Empty;
        public short Ranking { get; set; }
        public string? PositionLevelDescription { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PositionLevelForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PositionLevelSearchDto
    {
        public string? PositionLevelCode { get; set; }
        public SearchType PositionLevelCodeSearchType { get; set; } = SearchType.Contains;

        public string? PositionLevelName { get; set; }
        public SearchType PositionLevelNameSearchType { get; set; } = SearchType.Contains;

        public string? Ranking { get; set; }
        public SearchType RankingSearchType { get; set; } = SearchType.Contains;

        public string? PositionLevelDescription { get; set; }
        public SearchType PositionLevelDescriptionSearchType { get; set; } = SearchType.Contains;

    }
}
