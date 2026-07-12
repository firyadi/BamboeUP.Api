using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record AwardDto
    {
        public long AwardId { get; set; }
        public Guid AwardGuid { get; init; }
        
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardCriteria { get; set; }
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record AwardForCreationDto
    {
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardCriteria { get; set; }
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record AwardForUpdateDto
    {
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardCriteria { get; set; }
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public record AwardForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class AwardSearchDto
    {
        public string? AwardCode { get; set; }
        public SearchType AwardCodeSearchType { get; set; } = SearchType.Contains;

        public string? AwardName { get; set; }
        public SearchType AwardNameSearchType { get; set; } = SearchType.Contains;

        public string? SrAwardCriteria { get; set; }
        public SearchType SrAwardCriteriaSearchType { get; set; } = SearchType.Contains;

        public string? SrAwardType { get; set; }
        public SearchType SrAwardTypeSearchType { get; set; } = SearchType.Contains;

        public string? ValidFrom { get; set; }
        public SearchType ValidFromSearchType { get; set; } = SearchType.Contains;

        public string? Validto { get; set; }
        public SearchType ValidtoSearchType { get; set; } = SearchType.Contains;

        public string? AwardPrize { get; set; }
        public SearchType AwardPrizeSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;

    }
}
