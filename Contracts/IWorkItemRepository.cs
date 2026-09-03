using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IWorkItemRepository
    {
        Task<WorkItem?> GetWorkItemAsync(Guid workItemGuid, bool trackChanges);
        Task<WorkItem?> GetByPersonGuidAndWorkItemGuidAsync(Guid personGuid, Guid workItemGuid);
        Task<IEnumerable<WorkItem>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreateWorkItemAsync(WorkItem workItem, IDbTransaction? transaction = null);
        Task UpdateWorkItemAsync(WorkItem workItem, IDbTransaction? transaction = null);
        Task SoftDeleteWorkItemAsync(WorkItem workItem, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteWorkItemAsync(Guid workItemGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<WorkItem>> SearchWorkItemAsync(
            string? workItemCode,
            string? workItemCodeSearchType,
            string? title,
            string? titleSearchType,
            string? description,
            string? descriptionSearchType,
            string? module,
            string? moduleSearchType,
            string? workItemType,
            string? workItemTypeSearchType,
            string? ownerName,
            string? ownerNameSearchType,
            string? workflowStatus,
            string? workflowStatusSearchType,
            string? priority,
            string? prioritySearchType,
            string? completionPercent,
            string? completionPercentSearchType,
            string? tags,
            string? tagsSearchType,
            string? framework,
            string? frameworkSearchType,
            string? dueDate,
            string? dueDateSearchType,
            string? note,
            string? noteSearchType,
            Guid personGuid, Guid workItemGuid,
            IDbTransaction? transaction = null);
    }
}
