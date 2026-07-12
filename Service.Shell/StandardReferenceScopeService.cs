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
    public class StandardReferenceScopeService : IStandardReferenceScopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public StandardReferenceScopeService(
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

        public async Task<IEnumerable<StandardReferenceScopeDto>> GetAllStandardReferenceScopesAsync(bool trackChanges)
        {
            var entities = await _repository.StandardReferenceScope.GetAllStandardReferenceScopesAsync(trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceScopeDto>>();
        }

        public async Task<StandardReferenceScopeDto> GetStandardReferenceScopeByGuidAsync(Guid standardReferenceScopeGuid, bool trackChanges)
        {
            var entity = await _repository.StandardReferenceScope.GetStandardReferenceScopeAsync(standardReferenceScopeGuid, trackChanges);
            if (entity == null) return null!;

            var dto = entity.Adapt<StandardReferenceScopeDto>();
            var items = await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(standardReferenceScopeGuid);
            dto.StandardReferenceScopeItems = items.Adapt<IEnumerable<StandardReferenceScopeItemDto>>();

            return dto;
        }

        public async Task<StandardReferenceScopeDto> CreateStandardReferenceScopeAsync(StandardReferenceScopeForCreationDto input)
        {
            var model = input.Adapt<StandardReferenceScope>();
            model.StatusId = 1;

            if (model.CompanyGuid.HasValue && model.CompanyGuid.Value != Guid.Empty)
            {
                var company = await _repository.Company.GetCompanyAsync(model.CompanyGuid.Value, false);
                if (company != null) model.CompanyId = company.CompanyId;
            }
            if (model.CompanyOfficeGuid.HasValue && model.CompanyOfficeGuid.Value != Guid.Empty)
            {
                var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(model.CompanyOfficeGuid.Value, false);
                if (office != null) model.CompanyOfficeId = office.CompanyOfficeId;
            }

            // Resolve StandardReferenceId
            var parent = await _repository.StandardReference.GetStandardReferenceAsync(input.StandardReferenceGuid, false);
            if (parent != null) model.StandardReferenceId = parent.StandardReferenceId;

            await _repository.StandardReferenceScope.CreateStandardReferenceScopeAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "StandardReferenceScope",
                    EntityKey = model.StandardReferenceScopeGuid.ToString(),
                    EntityDisplayName = model.CompanyName ?? "Scope",
                    ParentTableName = "StandardReference",
                    ParentEntityKey = model.StandardReferenceGuid.ToString(),
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = model
                }
            };

            // Insert child items if any
            if (input.StandardReferenceScopeItems != null && input.StandardReferenceScopeItems.Any())
            {
                foreach (var itemDto in input.StandardReferenceScopeItems)
                {
                    var item = itemDto.Adapt<StandardReferenceScopeItem>();
                    item.StandardReferenceScopeId = model.StandardReferenceScopeId;
                    item.StandardReferenceScopeGuid = model.StandardReferenceScopeGuid;
                    item.StatusId = 1;

                    await _repository.StandardReferenceScopeItem.CreateStandardReferenceScopeItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = item.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = model.StandardReferenceScopeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = item
                    });
                }
            }

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "StandardReferenceScope",
                RootEntityKey = model.StandardReferenceScopeGuid.ToString(),
                RootDisplayName = model.CompanyName ?? "Scope",
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = entries
            });

            return model.Adapt<StandardReferenceScopeDto>();
        }

        public async Task UpdateStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, StandardReferenceScopeForUpdateDto input, bool trackChanges)
        {
            var oldScope = await _repository.StandardReferenceScope.GetStandardReferenceScopeAsync(standardReferenceScopeGuid, false);
            var oldItems = (await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(standardReferenceScopeGuid)).ToList();

            var model = input.Adapt<StandardReferenceScope>();
            model.StandardReferenceScopeGuid = standardReferenceScopeGuid;
            model.StandardReferenceScopeId = oldScope.StandardReferenceScopeId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;

            if (model.CompanyGuid.HasValue && model.CompanyGuid.Value != Guid.Empty)
            {
                var company = await _repository.Company.GetCompanyAsync(model.CompanyGuid.Value, false);
                if (company != null) model.CompanyId = company.CompanyId;
            }
            if (model.CompanyOfficeGuid.HasValue && model.CompanyOfficeGuid.Value != Guid.Empty)
            {
                var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(model.CompanyOfficeGuid.Value, false);
                if (office != null) model.CompanyOfficeId = office.CompanyOfficeId;
            }

            var parent = await _repository.StandardReference.GetStandardReferenceAsync(input.StandardReferenceGuid, false);
            if (parent != null) model.StandardReferenceId = parent.StandardReferenceId;

            await _repository.StandardReferenceScope.UpdateStandardReferenceScopeAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "StandardReferenceScope",
                    EntityKey = model.StandardReferenceScopeGuid.ToString(),
                    EntityDisplayName = model.CompanyName ?? "Scope",
                    ParentTableName = "StandardReference",
                    ParentEntityKey = model.StandardReferenceGuid.ToString(),
                    ActionType = "UPDATE",
                    OldEntity = oldScope,
                    NewEntity = model
                }
            };

            // Align items
            var newItems = input.StandardReferenceScopeItems?.ToList() ?? new List<StandardReferenceScopeItemForUpdateDto>();
            var oldItemDict = oldItems.ToDictionary(o => o.StandardReferenceScopeItemGuid);

            foreach (var itemDto in newItems)
            {
                var item = itemDto.Adapt<StandardReferenceScopeItem>();
                item.StandardReferenceScopeGuid = standardReferenceScopeGuid;
                item.StandardReferenceScopeId = model.StandardReferenceScopeId;
                item.StatusId = 2;
                item.UpdatedTime = DateTime.UtcNow;

                if (itemDto.StandardReferenceScopeItemGuid != Guid.Empty && oldItemDict.ContainsKey(itemDto.StandardReferenceScopeItemGuid))
                {
                    var oldItem = oldItemDict[itemDto.StandardReferenceScopeItemGuid];
                    item.StandardReferenceScopeItemId = oldItem.StandardReferenceScopeItemId;
                    item.UpdatedById = input.UpdatedById;
                    await _repository.StandardReferenceScopeItem.UpdateStandardReferenceScopeItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = item.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = standardReferenceScopeGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldItem,
                        NewEntity = item
                    });
                }
                else
                {
                    item.StandardReferenceScopeItemGuid = Guid.NewGuid();
                    item.CreatedById = input.UpdatedById ?? 0;
                    item.StatusId = 1;
                    await _repository.StandardReferenceScopeItem.CreateStandardReferenceScopeItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = item.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = standardReferenceScopeGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = item
                    });
                }
            }

            foreach (var oldItem in oldItems)
            {
                if (!newItems.Any(n => n.StandardReferenceScopeItemGuid == oldItem.StandardReferenceScopeItemGuid))
                {
                    await _repository.StandardReferenceScopeItem.SoftDeleteStandardReferenceScopeItemAsync(oldItem, input.UpdatedById ?? 0);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScopeItem",
                        EntityKey = oldItem.StandardReferenceScopeItemGuid.ToString(),
                        EntityDisplayName = oldItem.StandardReferenceScopeItemName,
                        ParentTableName = "StandardReferenceScope",
                        ParentEntityKey = standardReferenceScopeGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldItem,
                        NewEntity = null
                    });
                }
            }

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "StandardReferenceScope",
                RootEntityKey = model.StandardReferenceScopeGuid.ToString(),
                RootDisplayName = model.CompanyName ?? "Scope",
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (input.UpdatedById ?? 0).ToString(),
                Entries = entries
            });
        }

        public async Task DeleteStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, StandardReferenceScopeForDeleteDto input, bool trackChanges)
        {
            var oldScope = await _repository.StandardReferenceScope.GetStandardReferenceScopeAsync(standardReferenceScopeGuid, false);
            var model = new StandardReferenceScope { StandardReferenceScopeGuid = standardReferenceScopeGuid };
            await _repository.StandardReferenceScope.SoftDeleteStandardReferenceScopeAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "StandardReferenceScope",
                RootEntityKey = standardReferenceScopeGuid.ToString(),
                RootDisplayName = oldScope?.CompanyName ?? "Scope",
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReferenceScope",
                        EntityKey = standardReferenceScopeGuid.ToString(),
                        EntityDisplayName = oldScope?.CompanyName ?? "Scope",
                        ActionType = "DELETE",
                        OldEntity = oldScope,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceScopeByAdminAsync(Guid standardReferenceScopeGuid, bool trackChanges)
        {
            await _repository.StandardReferenceScope.DeleteStandardReferenceScopeAsync(standardReferenceScopeGuid);
        }

        public async Task<IEnumerable<StandardReferenceScopeDto>> SearchStandardReferenceScopeAsync(
            Guid companyGuid, Guid companyOfficeGuid, Guid standardReferenceGuid, Guid standardReferenceScopeGuid)
        {
            var data = await _repository.StandardReferenceScope.SearchStandardReferenceScopeAsync(
                companyGuid, companyOfficeGuid, standardReferenceGuid, standardReferenceScopeGuid);
            return data.Adapt<IEnumerable<StandardReferenceScopeDto>>();
        }

        public async Task<IEnumerable<StandardReferenceScopeDto>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid)
        {
            var data = await _repository.StandardReferenceScope.GetAllByStandardReferenceGuidAsync(standardReferenceGuid);
            return data.Adapt<IEnumerable<StandardReferenceScopeDto>>();
        }

        public async Task<StandardReferenceScopeDto> GetByStandardReferenceGuidAndScopeGuidAsync(Guid standardReferenceGuid, Guid standardReferenceScopeGuid)
        {
            var data = await _repository.StandardReferenceScope.GetByStandardReferenceGuidAndScopeGuidAsync(standardReferenceGuid, standardReferenceScopeGuid);
            return data.Adapt<StandardReferenceScopeDto>();
        }
    }
}
