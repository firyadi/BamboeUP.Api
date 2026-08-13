using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record AwardCriteriaDto
    {
        public long AwardCriteriaId { get; set; }
        public Guid AwardCriteriaGuid { get; init; }
        public long AwardId { get; set; }
        public int LineNo { get; set; }
        public string CriteriaCode { get; set; } = string.Empty;
        public string CriteriaName { get; set; } = string.Empty;
        public decimal? MinimumScore { get; set; }
        public decimal? MaximumScore { get; set; }
        public decimal? Weight { get; set; }
        public string? Note { get; set; }
public string? AwardName { get; set; }

        public IEnumerable<AwardCriteriaRuleDto>? AwardCriteriaRules { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record AwardCriteriaForCreationDto
    {
        public long AwardId { get; set; }
        public int LineNo { get; set; }
        public string CriteriaCode { get; set; } = string.Empty;
        public string CriteriaName { get; set; } = string.Empty;
        public decimal? MinimumScore { get; set; }
        public decimal? MaximumScore { get; set; }
        public decimal? Weight { get; set; }
        public string? Note { get; set; }
        public IEnumerable<AwardCriteriaRuleForCreationDto>? AwardCriteriaRules { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record AwardCriteriaForUpdateDto
    {
        public Guid AwardCriteriaGuid { get; set; }
        public long AwardId { get; set; }
        public int LineNo { get; set; }
        public string CriteriaCode { get; set; } = string.Empty;
        public string CriteriaName { get; set; } = string.Empty;
        public decimal? MinimumScore { get; set; }
        public decimal? MaximumScore { get; set; }
        public decimal? Weight { get; set; }
        public string? Note { get; set; }
        public IEnumerable<AwardCriteriaRuleForUpdateDto>? AwardCriteriaRules { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record AwardCriteriaForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class AwardCriteriaSearchDto
    {
        public string? LineNo { get; set; }
        public SearchType LineNoSearchType { get; set; } = SearchType.Contains;

        public string? CriteriaCode { get; set; }
        public SearchType CriteriaCodeSearchType { get; set; } = SearchType.Contains;

        public string? CriteriaName { get; set; }
        public SearchType CriteriaNameSearchType { get; set; } = SearchType.Contains;

        public string? MinimumScore { get; set; }
        public SearchType MinimumScoreSearchType { get; set; } = SearchType.Contains;

        public string? MaximumScore { get; set; }
        public SearchType MaximumScoreSearchType { get; set; } = SearchType.Contains;

        public string? Weight { get; set; }
        public SearchType WeightSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
    }
}
