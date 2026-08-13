using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record AwardCriteriaRuleDto
    {
        public long AwardCriteriaRuleId { get; set; }
        public Guid AwardCriteriaRuleGuid { get; init; }
        public long AwardCriteriaId { get; set; }
        public string RuleCode { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public decimal? ValueFrom { get; set; }
        public decimal? Valueto { get; set; }
        public decimal? Score { get; set; }
        public string? Note { get; set; }
public string? AwardCriteriaName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record AwardCriteriaRuleForCreationDto
    {
        public long AwardCriteriaId { get; set; }
        public string RuleCode { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public decimal? ValueFrom { get; set; }
        public decimal? Valueto { get; set; }
        public decimal? Score { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record AwardCriteriaRuleForUpdateDto
    {
        public Guid AwardCriteriaRuleGuid { get; set; }
        public long AwardCriteriaId { get; set; }
        public string RuleCode { get; set; } = string.Empty;
        public string RuleName { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public decimal? ValueFrom { get; set; }
        public decimal? Valueto { get; set; }
        public decimal? Score { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record AwardCriteriaRuleForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class AwardCriteriaRuleSearchDto
    {
        public string? RuleCode { get; set; }
        public SearchType RuleCodeSearchType { get; set; } = SearchType.Contains;

        public string? RuleName { get; set; }
        public SearchType RuleNameSearchType { get; set; } = SearchType.Contains;

        public string? Operator { get; set; }
        public SearchType OperatorSearchType { get; set; } = SearchType.Contains;

        public string? ValueFrom { get; set; }
        public SearchType ValueFromSearchType { get; set; } = SearchType.Contains;

        public string? Valueto { get; set; }
        public SearchType ValuetoSearchType { get; set; } = SearchType.Contains;

        public string? Score { get; set; }
        public SearchType ScoreSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
    }
}
