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
    public class StandardReferenceService : IStandardReferenceService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public StandardReferenceService(
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

        public async Task<IEnumerable<StandardReferenceDto>> GetAllStandardReferencesAsync(bool trackChanges)
        {
            var entities = await _repository.StandardReference.GetAllStandardReferencesAsync(trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceDto>>();
        }

        public async Task<IEnumerable<StandardReferenceDto>> GetStandardReferencesForParentSelectionAsync(Guid? currentRecordGuid, bool trackChanges)
        {
            var entities = await _repository.StandardReference.GetStandardReferencesForParentSelectionAsync(currentRecordGuid, trackChanges);
            return entities.Adapt<IEnumerable<StandardReferenceDto>>();
        }

        public async Task<StandardReferenceDto> GetStandardReferenceByGuidAsync(Guid standardReferenceGuid, bool trackChanges)
        {
            var entity = await _repository.StandardReference.GetStandardReferenceAsync(standardReferenceGuid, trackChanges);
            if (entity == null) return null!;

            var dto = entity.Adapt<StandardReferenceDto>();

            // Load default items
            var items = await _repository.StandardReferenceItem.GetAllByStandardReferenceGuidAsync(standardReferenceGuid);
            dto.StandardReferenceItems = items.Adapt<IEnumerable<StandardReferenceItemDto>>();

            // Load scopes
            var scopes = await _repository.StandardReferenceScope.GetAllByStandardReferenceGuidAsync(standardReferenceGuid);
            var scopeDtos = new List<StandardReferenceScopeDto>();
            foreach (var scope in scopes)
            {
                var scopeDto = scope.Adapt<StandardReferenceScopeDto>();
                // Load scope items
                var scopeItems = await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(scope.StandardReferenceScopeGuid);
                scopeDto.StandardReferenceScopeItems = scopeItems.Adapt<IEnumerable<StandardReferenceScopeItemDto>>();
                scopeDtos.Add(scopeDto);
            }
            dto.StandardReferenceScopes = scopeDtos;

            return dto;
        }

        public async Task<StandardReferenceDto> CreateStandardReferenceAsync(StandardReferenceForCreationDto input)
        {
            var model = input.Adapt<StandardReference>();
            model.StatusId = 1;
            await _repository.StandardReference.CreateStandardReferenceAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "StandardReference",
                    EntityKey = model.StandardReferenceGuid.ToString(),
                    EntityDisplayName = model.StandardReferenceName,
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = model
                }
            };

            // Insert default items
            if (input.StandardReferenceItems != null && input.StandardReferenceItems.Any())
            {
                foreach (var itemDto in input.StandardReferenceItems)
                {
                    var item = itemDto.Adapt<StandardReferenceItem>();
                    item.StandardReferenceId = model.StandardReferenceId;
                    item.StandardReferenceGuid = model.StandardReferenceGuid;
                    item.StatusId = 1;

                    await _repository.StandardReferenceItem.CreateStandardReferenceItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = item.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = model.StandardReferenceGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = item
                    });
                }
            }

            // Insert scopes
            if (input.StandardReferenceScopes != null && input.StandardReferenceScopes.Any())
            {
                foreach (var scopeDto in input.StandardReferenceScopes)
                {
                    var scope = scopeDto.Adapt<StandardReferenceScope>();
                    scope.StandardReferenceId = model.StandardReferenceId;
                    scope.StandardReferenceGuid = model.StandardReferenceGuid;
                    scope.StatusId = 1;

                    // Resolve CompanyId from CompanyGuid
                    if (scope.CompanyGuid.HasValue && scope.CompanyGuid.Value != Guid.Empty)
                    {
                        var company = await _repository.Company.GetCompanyAsync(scope.CompanyGuid.Value, false);
                        if (company != null) scope.CompanyId = company.CompanyId;
                    }

                    // Resolve CompanyOfficeId from CompanyOfficeGuid
                    if (scope.CompanyOfficeGuid.HasValue && scope.CompanyOfficeGuid.Value != Guid.Empty)
                    {
                        var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(scope.CompanyOfficeGuid.Value, false);
                        if (office != null) scope.CompanyOfficeId = office.CompanyOfficeId;
                    }

                    await _repository.StandardReferenceScope.CreateStandardReferenceScopeAsync(scope);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScope",
                        EntityKey = scope.StandardReferenceScopeGuid.ToString(),
                        EntityDisplayName = scope.CompanyName ?? "Scope",
                        ParentTableName = "StandardReference",
                        ParentEntityKey = model.StandardReferenceGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = scope
                    });

                    // Insert scope items
                    if (scopeDto.StandardReferenceScopeItems != null && scopeDto.StandardReferenceScopeItems.Any())
                    {
                        foreach (var scopeItemDto in scopeDto.StandardReferenceScopeItems)
                        {
                            var scopeItem = scopeItemDto.Adapt<StandardReferenceScopeItem>();
                            scopeItem.StandardReferenceScopeId = scope.StandardReferenceScopeId;
                            scopeItem.StandardReferenceScopeGuid = scope.StandardReferenceScopeGuid;
                            scopeItem.StatusId = 1;

                            await _repository.StandardReferenceScopeItem.CreateStandardReferenceScopeItemAsync(scopeItem);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "StandardReferenceScopeItem",
                                EntityKey = scopeItem.StandardReferenceScopeItemGuid.ToString(),
                                EntityDisplayName = scopeItem.StandardReferenceScopeItemName,
                                ParentTableName = "StandardReferenceScope",
                                ParentEntityKey = scope.StandardReferenceScopeGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = scopeItem
                            });
                        }
                    }
                }
            }

            // Log single audit session
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "StandardReference",
                RootEntityKey = model.StandardReferenceGuid.ToString(),
                RootDisplayName = model.StandardReferenceName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = entries
            });

            return model.Adapt<StandardReferenceDto>();
        }

        public async Task UpdateStandardReferenceAsync(Guid standardReferenceGuid, StandardReferenceForUpdateDto input, bool trackChanges)
        {
            var oldHeader = await _repository.StandardReference.GetStandardReferenceAsync(standardReferenceGuid, false);
            if (oldHeader is null)
                throw new KeyNotFoundException($"StandardReference with GUID {standardReferenceGuid} not found.");
            var oldItems = (await _repository.StandardReferenceItem.GetAllByStandardReferenceGuidAsync(standardReferenceGuid)).ToList();
            var oldScopes = (await _repository.StandardReferenceScope.GetAllByStandardReferenceGuidAsync(standardReferenceGuid)).ToList();

            var model = input.Adapt<StandardReference>();
            model.StandardReferenceGuid = standardReferenceGuid;
            model.StandardReferenceId = oldHeader.StandardReferenceId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.StandardReference.UpdateStandardReferenceAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "StandardReference",
                    EntityKey = model.StandardReferenceGuid.ToString(),
                    EntityDisplayName = model.StandardReferenceName,
                    ActionType = "UPDATE",
                    OldEntity = oldHeader,
                    NewEntity = model
                }
            };

            // 1. Align default items
            var newItems = input.StandardReferenceItems?.ToList() ?? new List<StandardReferenceItemForUpdateDto>();
            var oldItemDict = oldItems.ToDictionary(o => o.StandardReferenceItemGuid);

            foreach (var itemDto in newItems)
            {
                var item = itemDto.Adapt<StandardReferenceItem>();
                item.StandardReferenceGuid = standardReferenceGuid;
                item.StandardReferenceId = model.StandardReferenceId;
                item.StatusId = 2;
                item.UpdatedTime = DateTime.UtcNow;

                if (itemDto.StandardReferenceItemGuid != Guid.Empty && oldItemDict.ContainsKey(itemDto.StandardReferenceItemGuid))
                {
                    // Update
                    var oldItem = oldItemDict[itemDto.StandardReferenceItemGuid];
                    item.StandardReferenceItemId = oldItem.StandardReferenceItemId;
                    item.UpdatedById = input.UpdatedById;
                    await _repository.StandardReferenceItem.UpdateStandardReferenceItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = item.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldItem,
                        NewEntity = item
                    });
                }
                else
                {
                    // Create
                    item.StandardReferenceItemGuid = Guid.NewGuid();
                    item.CreatedById = input.UpdatedById ?? 0;
                    item.StatusId = 1;
                    await _repository.StandardReferenceItem.CreateStandardReferenceItemAsync(item);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = item.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = item.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = item
                    });
                }
            }

            // Soft-delete missing items
            foreach (var oldItem in oldItems)
            {
                if (!newItems.Any(n => n.StandardReferenceItemGuid == oldItem.StandardReferenceItemGuid))
                {
                    await _repository.StandardReferenceItem.SoftDeleteStandardReferenceItemAsync(oldItem, input.UpdatedById ?? 0);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceItem",
                        EntityKey = oldItem.StandardReferenceItemGuid.ToString(),
                        EntityDisplayName = oldItem.StandardReferenceItemName,
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldItem,
                        NewEntity = null
                    });
                }
            }

            // 2. Align Scopes
            var newScopes = input.StandardReferenceScopes?.ToList() ?? new List<StandardReferenceScopeForUpdateDto>();
            var oldScopeDict = oldScopes.ToDictionary(o => o.StandardReferenceScopeGuid);

            foreach (var scopeDto in newScopes)
            {
                var scope = scopeDto.Adapt<StandardReferenceScope>();
                scope.StandardReferenceGuid = standardReferenceGuid;
                scope.StandardReferenceId = model.StandardReferenceId;
                scope.StatusId = 2;
                scope.UpdatedTime = DateTime.UtcNow;

                // Resolve IDs
                if (scope.CompanyGuid.HasValue && scope.CompanyGuid.Value != Guid.Empty)
                {
                    var company = await _repository.Company.GetCompanyAsync(scope.CompanyGuid.Value, false);
                    if (company != null) scope.CompanyId = company.CompanyId;
                }
                if (scope.CompanyOfficeGuid.HasValue && scope.CompanyOfficeGuid.Value != Guid.Empty)
                {
                    var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(scope.CompanyOfficeGuid.Value, false);
                    if (office != null) scope.CompanyOfficeId = office.CompanyOfficeId;
                }

                List<StandardReferenceScopeItem>? oldScopeItems = null;

                if (scopeDto.StandardReferenceScopeGuid != Guid.Empty && oldScopeDict.ContainsKey(scopeDto.StandardReferenceScopeGuid))
                {
                    // Scope Update
                    var oldScope = oldScopeDict[scopeDto.StandardReferenceScopeGuid];
                    scope.StandardReferenceScopeId = oldScope.StandardReferenceScopeId;
                    scope.UpdatedById = input.UpdatedById;
                    await _repository.StandardReferenceScope.UpdateStandardReferenceScopeAsync(scope);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScope",
                        EntityKey = scope.StandardReferenceScopeGuid.ToString(),
                        EntityDisplayName = scope.CompanyName ?? "Scope",
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldScope,
                        NewEntity = scope
                    });

                    // Get old scope items for this scope
                    oldScopeItems = (await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(scope.StandardReferenceScopeGuid)).ToList();
                }
                else
                {
                    // Scope Create
                    scope.StandardReferenceScopeGuid = Guid.NewGuid();
                    scope.CreatedById = input.UpdatedById ?? 0;
                    scope.StatusId = 1;
                    await _repository.StandardReferenceScope.CreateStandardReferenceScopeAsync(scope);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScope",
                        EntityKey = scope.StandardReferenceScopeGuid.ToString(),
                        EntityDisplayName = scope.CompanyName ?? "Scope",
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = scope
                    });

                    oldScopeItems = new List<StandardReferenceScopeItem>();
                }

                // 2.1 Align ScopeItems for this Scope
                var newScopeItems = scopeDto.StandardReferenceScopeItems?.ToList() ?? new List<StandardReferenceScopeItemForUpdateDto>();
                var oldScopeItemDict = oldScopeItems.ToDictionary(o => o.StandardReferenceScopeItemGuid);

                foreach (var scopeItemDto in newScopeItems)
                {
                    var scopeItem = scopeItemDto.Adapt<StandardReferenceScopeItem>();
                    scopeItem.StandardReferenceScopeGuid = scope.StandardReferenceScopeGuid;
                    scopeItem.StandardReferenceScopeId = scope.StandardReferenceScopeId;
                    scopeItem.StatusId = 2;
                    scopeItem.UpdatedTime = DateTime.UtcNow;

                    if (scopeItemDto.StandardReferenceScopeItemGuid != Guid.Empty && oldScopeItemDict.ContainsKey(scopeItemDto.StandardReferenceScopeItemGuid))
                    {
                        // ScopeItem Update
                        var oldScopeItem = oldScopeItemDict[scopeItemDto.StandardReferenceScopeItemGuid];
                        scopeItem.StandardReferenceScopeItemId = oldScopeItem.StandardReferenceScopeItemId;
                        scopeItem.UpdatedById = input.UpdatedById;
                        await _repository.StandardReferenceScopeItem.UpdateStandardReferenceScopeItemAsync(scopeItem);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "StandardReferenceScopeItem",
                            EntityKey = scopeItem.StandardReferenceScopeItemGuid.ToString(),
                            EntityDisplayName = scopeItem.StandardReferenceScopeItemName,
                            ParentTableName = "StandardReferenceScope",
                            ParentEntityKey = scope.StandardReferenceScopeGuid.ToString(),
                            ActionType = "UPDATE",
                            OldEntity = oldScopeItem,
                            NewEntity = scopeItem
                        });
                    }
                    else
                    {
                        // ScopeItem Create
                        scopeItem.StandardReferenceScopeItemGuid = Guid.NewGuid();
                        scopeItem.CreatedById = input.UpdatedById ?? 0;
                        scopeItem.StatusId = 1;
                        await _repository.StandardReferenceScopeItem.CreateStandardReferenceScopeItemAsync(scopeItem);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "StandardReferenceScopeItem",
                            EntityKey = scopeItem.StandardReferenceScopeItemGuid.ToString(),
                            EntityDisplayName = scopeItem.StandardReferenceScopeItemName,
                            ParentTableName = "StandardReferenceScope",
                            ParentEntityKey = scope.StandardReferenceScopeGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = scopeItem
                        });
                    }
                }

                // Soft-delete missing scope items
                foreach (var oldScopeItem in oldScopeItems)
                {
                    if (!newScopeItems.Any(n => n.StandardReferenceScopeItemGuid == oldScopeItem.StandardReferenceScopeItemGuid))
                    {
                        await _repository.StandardReferenceScopeItem.SoftDeleteStandardReferenceScopeItemAsync(oldScopeItem, input.UpdatedById ?? 0);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "StandardReferenceScopeItem",
                            EntityKey = oldScopeItem.StandardReferenceScopeItemGuid.ToString(),
                            EntityDisplayName = oldScopeItem.StandardReferenceScopeItemName,
                            ParentTableName = "StandardReferenceScope",
                            ParentEntityKey = scope.StandardReferenceScopeGuid.ToString(),
                            ActionType = "DELETE",
                            OldEntity = oldScopeItem,
                            NewEntity = null
                        });
                    }
                }
            }

            // Soft-delete missing scopes
            foreach (var oldScope in oldScopes)
            {
                if (!newScopes.Any(n => n.StandardReferenceScopeGuid == oldScope.StandardReferenceScopeGuid))
                {
                    // Soft-delete scope items belonging to this deleted scope first
                    var oldScopeItems = await _repository.StandardReferenceScopeItem.GetAllByStandardReferenceScopeGuidAsync(oldScope.StandardReferenceScopeGuid);
                    foreach (var scopeItem in oldScopeItems)
                    {
                        await _repository.StandardReferenceScopeItem.SoftDeleteStandardReferenceScopeItemAsync(scopeItem, input.UpdatedById ?? 0);
                        entries.Add(new AuditLogEntry
                        {
                            TableName = "StandardReferenceScopeItem",
                            EntityKey = scopeItem.StandardReferenceScopeItemGuid.ToString(),
                            EntityDisplayName = scopeItem.StandardReferenceScopeItemName,
                            ParentTableName = "StandardReferenceScope",
                            ParentEntityKey = oldScope.StandardReferenceScopeGuid.ToString(),
                            ActionType = "DELETE",
                            OldEntity = scopeItem,
                            NewEntity = null
                        });
                    }

                    await _repository.StandardReferenceScope.SoftDeleteStandardReferenceScopeAsync(oldScope, input.UpdatedById ?? 0);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "StandardReferenceScope",
                        EntityKey = oldScope.StandardReferenceScopeGuid.ToString(),
                        EntityDisplayName = oldScope.CompanyName ?? "Scope",
                        ParentTableName = "StandardReference",
                        ParentEntityKey = standardReferenceGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldScope,
                        NewEntity = null
                    });
                }
            }

            // Log session
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "StandardReference",
                RootEntityKey = model.StandardReferenceGuid.ToString(),
                RootDisplayName = model.StandardReferenceName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (input.UpdatedById ?? 0).ToString(),
                Entries = entries
            });
        }

        public async Task DeleteStandardReferenceAsync(Guid standardReferenceGuid, StandardReferenceForDeleteDto input, bool trackChanges)
        {
            var oldHeader = await _repository.StandardReference.GetStandardReferenceAsync(standardReferenceGuid, false);
            var model = new StandardReference { StandardReferenceGuid = standardReferenceGuid };
            await _repository.StandardReference.SoftDeleteStandardReferenceAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "StandardReference",
                RootEntityKey = standardReferenceGuid.ToString(),
                RootDisplayName = oldHeader?.StandardReferenceName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "StandardReference",
                        EntityKey = standardReferenceGuid.ToString(),
                        EntityDisplayName = oldHeader?.StandardReferenceName,
                        ActionType = "DELETE",
                        OldEntity = oldHeader,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteStandardReferenceByAdminAsync(Guid standardReferenceGuid, bool trackChanges)
        {
            await _repository.StandardReference.DeleteStandardReferenceAsync(standardReferenceGuid);
        }

        public async Task<IEnumerable<StandardReferenceDto>> SearchStandardReferenceAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType, string? standardReferenceName, string? standardReferenceNameSearchType, string? note, string? noteSearchType, Guid companyGuid, Guid companyOfficeGuid)
        {
            var data = await _repository.StandardReference.SearchStandardReferenceAsync(
                standardReferenceInitial, standardReferenceInitialSearchType, standardReferenceName, standardReferenceNameSearchType, note, noteSearchType, companyGuid, companyOfficeGuid);
            return data.Adapt<IEnumerable<StandardReferenceDto>>();
        }
    }
}