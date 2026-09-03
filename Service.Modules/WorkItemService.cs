using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class WorkItemService : IWorkItemService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public WorkItemService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public WorkItemService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
        }

        public async Task<IEnumerable<WorkItemDto>> GetAllWorkItemsAsync(bool trackChanges)
        {
            var entities = await _repository.WorkItem.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<WorkItemDto>>();
        }

        public async Task<WorkItemDto?> GetWorkItemByGuidAsync(Guid workItemGuid, bool trackChanges)
        {
            var entity = await _repository.WorkItem.GetWorkItemAsync(workItemGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<WorkItemDto>();
            return dto;
        }

        public async Task<WorkItemDto> CreateWorkItemAsync(Guid personGuid, WorkItemForCreationDto input)
        {
            var model = input.Adapt<WorkItem>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.WorkItem.CreateWorkItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "WorkItem",
                RootEntityKey = model.WorkItemGuid.ToString(),
                RootDisplayName = model.WorkItemCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkItem",
                        EntityKey = model.WorkItemGuid.ToString(),
                        EntityDisplayName = model.WorkItemCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<WorkItemDto>();
        }

        public async Task UpdateWorkItemAsync(Guid personGuid, Guid workItemGuid, WorkItemForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.WorkItem.GetWorkItemAsync(workItemGuid, false);

            var model = input.Adapt<WorkItem>();
            model.WorkItemGuid = workItemGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.WorkItem.UpdateWorkItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "WorkItem",
                RootEntityKey = model.WorkItemGuid.ToString(),
                RootDisplayName = model.WorkItemCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkItem",
                        EntityKey = model.WorkItemGuid.ToString(),
                        EntityDisplayName = model.WorkItemCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteWorkItemAsync(Guid personGuid, Guid workItemGuid, WorkItemForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.WorkItem.GetWorkItemAsync(workItemGuid, false);
            var model = new WorkItem
            {
                WorkItemGuid = workItemGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.WorkItem.SoftDeleteWorkItemAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "WorkItem",
                RootEntityKey = workItemGuid.ToString(),
                RootDisplayName = oldDetail?.WorkItemCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkItem",
                        EntityKey = workItemGuid.ToString(),
                        EntityDisplayName = oldDetail?.WorkItemCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteWorkItemByAdminAsync(Guid workItemGuid, bool trackChanges)
        {
            await _repository.WorkItem.DeleteWorkItemAsync(workItemGuid);
        }

        public async Task<IEnumerable<WorkItemDto>> SearchWorkItemAsync(
            string? workItemCode, string? workItemCodeSearchType, string? title, string? titleSearchType, string? description, string? descriptionSearchType, string? module, string? moduleSearchType, string? workItemType, string? workItemTypeSearchType, string? ownerName, string? ownerNameSearchType, string? workflowStatus, string? workflowStatusSearchType, string? priority, string? prioritySearchType, string? completionPercent, string? completionPercentSearchType, string? tags, string? tagsSearchType, string? framework, string? frameworkSearchType, string? dueDate, string? dueDateSearchType, string? note, string? noteSearchType,
            Guid personGuid, Guid workItemGuid)
        {
            var data = await _repository.WorkItem.SearchWorkItemAsync(
                workItemCode, workItemCodeSearchType, title, titleSearchType, description, descriptionSearchType, module, moduleSearchType, workItemType, workItemTypeSearchType, ownerName, ownerNameSearchType, workflowStatus, workflowStatusSearchType, priority, prioritySearchType, completionPercent, completionPercentSearchType, tags, tagsSearchType, framework, frameworkSearchType, dueDate, dueDateSearchType, note, noteSearchType, 
                personGuid, workItemGuid);
            return data.Adapt<IEnumerable<WorkItemDto>>();
        }

        public async Task<IEnumerable<WorkItemDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.WorkItem.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<WorkItemDto>>();
        }

        public async Task<WorkItemDto?> GetByPersonGuidAndWorkItemGuidAsync(Guid personGuid, Guid workItemGuid)
        {
            var result = await _repository.WorkItem.GetByPersonGuidAndWorkItemGuidAsync(personGuid, workItemGuid);
            if (result == null) return null;
            return result.Adapt<WorkItemDto>();
        }
    }
}
