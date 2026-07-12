using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public class StandardReferenceItemService : IStandardReferenceItemService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public StandardReferenceItemService(
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

        public async Task<IEnumerable<StandardReferenceItemDto>> GetAllStandardReferenceItemsAsync(bool trackChanges)
        {
            var entities = await _repository.StandardReferenceItem.GetAllStandardReferenceItemsAsync(trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceItemDto>>();
        }

        public async Task<StandardReferenceItemDto> GetStandardReferenceItemByGuidAsync(Guid standardReferenceItemGuid, bool trackChanges)
        {
            var entity = await _repository.StandardReferenceItem.GetStandardReferenceItemAsync(standardReferenceItemGuid, trackChanges);
            return entity.Adapt<StandardReferenceItemDto>();
        }

        public async Task<StandardReferenceItemDto> CreateStandardReferenceItemAsync(StandardReferenceItemForCreationDto input)
        {
            var model = input.Adapt<StandardReferenceItem>();
            model.StatusId = 1;

            var parent = await _repository.StandardReference.GetStandardReferenceAsync(input.StandardReferenceGuid, false);
            if (parent != null) model.StandardReferenceId = parent.StandardReferenceId;

            await _repository.StandardReferenceItem.CreateStandardReferenceItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "StandardReferenceItem",
                RootEntityKey = model.StandardReferenceItemGuid.ToString(),
                RootDisplayName = model.StandardReferenceItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = model.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = model.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = model.StandardReferenceGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<StandardReferenceItemDto>();
        }

        public async Task UpdateStandardReferenceItemAsync(Guid standardReferenceItemGuid, StandardReferenceItemForUpdateDto input, bool trackChanges)
        {
            var oldItem = await _repository.StandardReferenceItem.GetStandardReferenceItemAsync(standardReferenceItemGuid, false);

            var model = input.Adapt<StandardReferenceItem>();
            model.StandardReferenceItemGuid = standardReferenceItemGuid;
            model.StandardReferenceItemId = oldItem.StandardReferenceItemId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;

            var parent = await _repository.StandardReference.GetStandardReferenceAsync(input.StandardReferenceGuid, false);
            if (parent != null) model.StandardReferenceId = parent.StandardReferenceId;

            await _repository.StandardReferenceItem.UpdateStandardReferenceItemAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "StandardReferenceItem",
                RootEntityKey = model.StandardReferenceItemGuid.ToString(),
                RootDisplayName = model.StandardReferenceItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (model.UpdatedById ?? 0).ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = model.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = model.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = model.StandardReferenceGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldItem,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceItemAsync(Guid standardReferenceItemGuid, StandardReferenceItemForDeleteDto input, bool trackChanges)
        {
            var oldItem = await _repository.StandardReferenceItem.GetStandardReferenceItemAsync(standardReferenceItemGuid, false);
            var model = new StandardReferenceItem { StandardReferenceItemGuid = standardReferenceItemGuid };
            await _repository.StandardReferenceItem.SoftDeleteStandardReferenceItemAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "StandardReferenceItem",
                RootEntityKey = standardReferenceItemGuid.ToString(),
                RootDisplayName = oldItem?.StandardReferenceItemName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = standardReferenceItemGuid.ToString(),
                        EntityDisplayName = oldItem?.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = oldItem?.StandardReferenceGuid.ToString() ?? string.Empty,
                        ActionType = "DELETE",
                        OldEntity = oldItem,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceItemByAdminAsync(Guid standardReferenceItemGuid, bool trackChanges)
        {
            await _repository.StandardReferenceItem.DeleteStandardReferenceItemAsync(standardReferenceItemGuid);
        }

        public async Task<IEnumerable<StandardReferenceItemDto>> SearchStandardReferenceItemAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType, string? standardReferenceItemInitial, string? standardReferenceItemInitialSearchType, string? standardReferenceItemName, string? standardReferenceItemNameSearchType, string? note, string? noteSearchType, Guid companyGuid, Guid companyOfficeGuid)
        {
            var data = await _repository.StandardReferenceItem.SearchStandardReferenceItemAsync(
                standardReferenceInitial, standardReferenceInitialSearchType, standardReferenceItemInitial, standardReferenceItemInitialSearchType, standardReferenceItemName, standardReferenceItemNameSearchType, note, noteSearchType, companyGuid, companyOfficeGuid);
            return data.Adapt<IEnumerable<StandardReferenceItemDto>>();
        }

        public async Task<IEnumerable<StandardReferenceItemDto>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid)
        {
            var result = await _repository.StandardReferenceItem.GetAllByStandardReferenceGuidAsync(standardReferenceGuid);
            return result.Adapt<IEnumerable<StandardReferenceItemDto>>();
        }

        public async Task<StandardReferenceItemDto> GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(Guid standardReferenceGuid, Guid standardReferenceItemGuid)
        {
            var result = await _repository.StandardReferenceItem.GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(standardReferenceGuid, standardReferenceItemGuid);
            return result.Adapt<StandardReferenceItemDto>();
        }
    }
}
