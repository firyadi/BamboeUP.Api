using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Shell
{
    public class StandardReferenceScopeItemService : IStandardReferenceScopeItemService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public StandardReferenceScopeItemService(
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

        public async Task<IEnumerable<StandardReferenceScopeItemDto>> GetAllStandardReferenceScopeItemsAsync(bool trackChanges)
        {
            var entities = await _repository.StandardReferenceScopeItem.GetAllStandardReferenceScopeItemsAsync(trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceScopeItemDto>>();
        }

        public async Task<StandardReferenceScopeItemDto> GetStandardReferenceScopeItemByGuidAsync(Guid scopeItemGuid, bool trackChanges)
        {
            var entity = await _repository.StandardReferenceScopeItem.GetStandardReferenceScopeItemAsync(scopeItemGuid, trackChanges)
                ?? throw new KeyNotFoundException($"StandardReferenceScopeItem with Guid '{scopeItemGuid}' not found.");
            return entity.Adapt<StandardReferenceScopeItemDto>();
        }

        public async Task<StandardReferenceScopeItemDto> CreateStandardReferenceScopeItemAsync(StandardReferenceScopeItemForCreationDto input)
        {
            var model = input.Adapt<StandardReferenceScopeItem>();
            model.StatusId = 1;

            var parent = await _repository.StandardReferenceScope.GetStandardReferenceScopeAsync(input.StandardReferenceScopeGuid, false);
            if (parent != null) model.StandardReferenceScopeId = parent.StandardReferenceScopeId;

            await _repository.StandardReferenceScopeItem.CreateStandardReferenceScopeItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "StandardReferenceScopeItem",
                RootEntityKey = model.StandardReferenceScopeItemGuid.ToString(),
                RootDisplayName = model.StandardReferenceScopeItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = model.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = model.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = model.StandardReferenceScopeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<StandardReferenceScopeItemDto>();
        }

        public async Task UpdateStandardReferenceScopeItemAsync(Guid scopeItemGuid, StandardReferenceScopeItemForUpdateDto input, bool trackChanges)
        {
            var oldItem = await _repository.StandardReferenceScopeItem.GetStandardReferenceScopeItemAsync(scopeItemGuid, false);
            if (oldItem is null)
                throw new KeyNotFoundException($"StandardReferenceScopeItem with GUID {scopeItemGuid} not found.");

            var model = input.Adapt<StandardReferenceScopeItem>();
            model.StandardReferenceScopeItemGuid = scopeItemGuid;
            model.StandardReferenceScopeItemId = oldItem.StandardReferenceScopeItemId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;

            var parent = await _repository.StandardReferenceScope.GetStandardReferenceScopeAsync(input.StandardReferenceScopeGuid, false);
            if (parent != null) model.StandardReferenceScopeId = parent.StandardReferenceScopeId;

            await _repository.StandardReferenceScopeItem.UpdateStandardReferenceScopeItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "StandardReferenceScopeItem",
                RootEntityKey = model.StandardReferenceScopeItemGuid.ToString(),
                RootDisplayName = model.StandardReferenceScopeItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (model.UpdatedById ?? 0).ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = model.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = model.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = model.StandardReferenceScopeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldItem,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceScopeItemAsync(Guid scopeItemGuid, StandardReferenceScopeItemForDeleteDto input, bool trackChanges)
        {
            var oldItem = await _repository.StandardReferenceScopeItem.GetStandardReferenceScopeItemAsync(scopeItemGuid, false);
            var model = new StandardReferenceScopeItem { StandardReferenceScopeItemGuid = scopeItemGuid };
            await _repository.StandardReferenceScopeItem.SoftDeleteStandardReferenceScopeItemAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "StandardReferenceScopeItem",
                RootEntityKey = scopeItemGuid.ToString(),
                RootDisplayName = oldItem?.StandardReferenceScopeItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = scopeItemGuid.ToString(),
                        EntityDisplayName = oldItem?.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = oldItem?.StandardReferenceScopeGuid.ToString() ?? string.Empty,
                        ActionType = "DELETE",
                        OldEntity = oldItem,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceScopeItemByAdminAsync(Guid scopeItemGuid, bool trackChanges)
        {
            await _repository.StandardReferenceScopeItem.DeleteStandardReferenceScopeItemAsync(scopeItemGuid);
        }

        public async Task<IEnumerable<StandardReferenceScopeItemDto>> SearchStandardReferenceScopeItemAsync(
            string? scopeItemInitial, string? scopeItemInitialSearchType, Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid)
        {
            var data = await _repository.StandardReferenceScopeItem.SearchStandardReferenceScopeItemAsync(
                scopeItemInitial, scopeItemInitialSearchType, standardReferenceScopeGuid, standardReferenceScopeItemGuid);
            return data.Adapt<IEnumerable<StandardReferenceScopeItemDto>>();
        }

        public async Task<IEnumerable<StandardReferenceScopeItemDto>> GetAllByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid)
        {
            var data = await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(standardReferenceScopeGuid);
            return data.Adapt<IEnumerable<StandardReferenceScopeItemDto>>();
        }

        public async Task<StandardReferenceScopeItemDto> GetByScopeGuidAndItemGuidAsync(Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid)
        {
            var data = await _repository.StandardReferenceScopeItem.GetByScopeGuidAndItemGuidAsync(standardReferenceScopeGuid, standardReferenceScopeItemGuid)
                ?? throw new KeyNotFoundException($"StandardReferenceScopeItem with Guid '{standardReferenceScopeItemGuid}' for StandardReferenceScope Guid '{standardReferenceScopeGuid}' not found.");
            return data.Adapt<StandardReferenceScopeItemDto>();
        }
    }
}
