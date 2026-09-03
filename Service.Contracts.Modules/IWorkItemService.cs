using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IWorkItemService
    {
        Task<IEnumerable<WorkItemDto>> GetAllWorkItemsAsync(bool trackChanges);
        Task<WorkItemDto?> GetWorkItemByGuidAsync(Guid workItemGuid, bool trackChanges);
        Task<WorkItemDto> CreateWorkItemAsync(Guid personGuid, WorkItemForCreationDto input);
        Task UpdateWorkItemAsync(Guid personGuid, Guid workItemGuid, WorkItemForUpdateDto input, bool trackChanges);
        Task DeleteWorkItemAsync(Guid personGuid, Guid workItemGuid, WorkItemForDeleteDto input, bool trackChanges);
        Task DeleteWorkItemByAdminAsync(Guid workItemGuid, bool trackChanges);

        Task<IEnumerable<WorkItemDto>> SearchWorkItemAsync(
            string? workItemCode, string? workItemCodeSearchType, string? title, string? titleSearchType, string? description, string? descriptionSearchType, string? module, string? moduleSearchType, string? workItemType, string? workItemTypeSearchType, string? ownerName, string? ownerNameSearchType, string? workflowStatus, string? workflowStatusSearchType, string? priority, string? prioritySearchType, string? completionPercent, string? completionPercentSearchType, string? tags, string? tagsSearchType, string? framework, string? frameworkSearchType, string? dueDate, string? dueDateSearchType, string? note, string? noteSearchType,
            Guid personGuid, Guid workItemGuid);

        // Detail (child) helpers
        Task<IEnumerable<WorkItemDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<WorkItemDto?> GetByPersonGuidAndWorkItemGuidAsync(Guid personGuid, Guid workItemGuid);
    }
}
