using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record WorkItemDto
    {
        public long WorkItemId { get; set; }
        public Guid WorkItemGuid { get; init; }
        public long OwnerUserId { get; set; }
        public string WorkItemCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public string? WorkItemType { get; set; }
        public string? OwnerName { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public int? CompletionPercent { get; set; }
        public string? Tags { get; set; }
        public string? Framework { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
public string? OwnerUserName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record WorkItemForCreationDto
    {
        public long OwnerUserId { get; set; }
        public string WorkItemCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public string? WorkItemType { get; set; }
        public string? OwnerName { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public int? CompletionPercent { get; set; }
        public string? Tags { get; set; }
        public string? Framework { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record WorkItemForUpdateDto
    {
        public Guid WorkItemGuid { get; set; }
        public long OwnerUserId { get; set; }
        public string WorkItemCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Module { get; set; }
        public string? WorkItemType { get; set; }
        public string? OwnerName { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;
        public string? Priority { get; set; }
        public int? CompletionPercent { get; set; }
        public string? Tags { get; set; }
        public string? Framework { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record WorkItemForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class WorkItemSearchDto
    {
        public string? WorkItemCode { get; set; }
        public SearchType WorkItemCodeSearchType { get; set; } = SearchType.Contains;

        public string? Title { get; set; }
        public SearchType TitleSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public string? Module { get; set; }
        public SearchType ModuleSearchType { get; set; } = SearchType.Contains;

        public string? WorkItemType { get; set; }
        public SearchType WorkItemTypeSearchType { get; set; } = SearchType.Contains;

        public string? OwnerName { get; set; }
        public SearchType OwnerNameSearchType { get; set; } = SearchType.Contains;

        public string? WorkflowStatus { get; set; }
        public SearchType WorkflowStatusSearchType { get; set; } = SearchType.Contains;

        public string? Priority { get; set; }
        public SearchType PrioritySearchType { get; set; } = SearchType.Contains;

        public string? CompletionPercent { get; set; }
        public SearchType CompletionPercentSearchType { get; set; } = SearchType.Contains;

        public string? Tags { get; set; }
        public SearchType TagsSearchType { get; set; } = SearchType.Contains;

        public string? Framework { get; set; }
        public SearchType FrameworkSearchType { get; set; } = SearchType.Contains;

        public string? DueDate { get; set; }
        public SearchType DueDateSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
    }
}
