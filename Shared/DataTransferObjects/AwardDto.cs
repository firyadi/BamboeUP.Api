using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record AwardDto
    {
        public long AwardId { get; set; }
        public Guid AwardGuid { get; init; }
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }
public string? AwardTypeName { get; set; }


        public IEnumerable<AwardCriteriaDto>? AwardCriteria { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record AwardForCreationDto
    {
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }
        public IEnumerable<AwardCriteriaForCreationDto>? AwardCriteria { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record AwardForUpdateDto
    {
        public string AwardCode { get; set; } = string.Empty;
        public string AwardName { get; set; } = string.Empty;
        public long SrAwardType { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime Validto { get; set; }
        public string? AwardPrize { get; set; }
        public string? Note { get; set; }
        public IEnumerable<AwardCriteriaForUpdateDto>? AwardCriteria { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record AwardForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class AwardSearchDto
    {
        public string? AwardCode { get; set; }
        public SearchType AwardCodeSearchType { get; set; } = SearchType.Contains;

        public string? AwardName { get; set; }
        public SearchType AwardNameSearchType { get; set; } = SearchType.Contains;

        public string? ValidFrom { get; set; }
        public SearchType ValidFromSearchType { get; set; } = SearchType.Contains;

        public string? Validto { get; set; }
        public SearchType ValidtoSearchType { get; set; } = SearchType.Contains;

        public string? AwardPrize { get; set; }
        public SearchType AwardPrizeSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
public string? AwardTypeName { get; set; }
public SearchType AwardTypeNameSearchType { get; set; } = SearchType.Contains;

    }
}
