using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Shell
{
    public partial class OrganizationUnitService : IOrganizationUnitService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;
        private readonly IUserScopeService _userScope;

        public OrganizationUnitService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
            _userScope = null!;
        }

        public OrganizationUnitService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext,
            IUserScopeService userScope)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
            _userScope = userScope;
        }

        public async Task<IEnumerable<OrganizationUnitDto>> GetAllOrganizationUnitsAsync(bool trackChanges)
        {
            return await _userScope.GetAccessibleOrganizationUnitsAsync();
        }

        public async Task<OrganizationUnitDto> GetOrganizationUnitByGuidAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            var entity = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"OrganizationUnit with GUID {organizationUnitGuid} not found.");
            await _userScope.EnsureCanAccessOrganizationUnitAsync(entity.OrganizationUnitId);
            return entity.Adapt<OrganizationUnitDto>();
        }

        public async Task<OrganizationUnitDto> CreateOrganizationUnitAsync(OrganizationUnitForCreationDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<OrganizationUnit>();
            model.ParentOrganizationUnitId = input.ParentOrganizationUnitId is null or 0 ? null : input.ParentOrganizationUnitId;

            if (model.ParentOrganizationUnitId == null)
            {
                model.LevelDepth = 1;
                model.HierarchyPath = "";
            }
            else
            {
                var parent = await _repository.OrganizationUnit.GetOrganizationUnitByIdAsync(model.ParentOrganizationUnitId.Value, false);
                if (parent != null)
                {
                    model.LevelDepth = parent.LevelDepth + 1;
                    model.HierarchyPath = parent.HierarchyPath;
                }
                else
                {
                    model.LevelDepth = 1;
                    model.HierarchyPath = "";
                }
            }
                model.StatusId = 1;
                model.CreatedTime = DateTime.UtcNow;
                await _repository.OrganizationUnit.CreateOrganizationUnitAsync(model, transaction);

            model.HierarchyPath = string.IsNullOrEmpty(model.HierarchyPath)
                ? $"/{model.OrganizationUnitId}/"
                : $"{model.HierarchyPath}{model.OrganizationUnitId}/";
            await _repository.OrganizationUnit.UpdateOrganizationUnitAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "OrganizationUnit",
                        EntityKey = model.OrganizationUnitGuid.ToString(),
                        EntityDisplayName = model.OrganizationUnitCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // Dapper doesn't automatically insert children, so we must manually insert nested Details
                if (input.OrganizationUnitScopes != null && input.OrganizationUnitScopes.Any())
                {
                    foreach (var detailDto in input.OrganizationUnitScopes)
                    {
                        var detail = detailDto.Adapt<OrganizationUnitScope>();
                        detail.OrganizationUnitId = model.OrganizationUnitId;
                    detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await _repository.OrganizationUnitScope.CreateOrganizationUnitScopeAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "OrganizationUnitScope",
                            EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                            EntityDisplayName = detail.ScopeType,
                            ParentTableName = "OrganizationUnit",
                            ParentEntityKey = model.OrganizationUnitGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // Single audit session for all entities
                await _audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "OrganizationUnit",
                    RootEntityKey = model.OrganizationUnitGuid.ToString(),
                    RootDisplayName = model.OrganizationUnitCode,
                    UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await _transactionManager.CommitAsync();
                return model.Adapt<OrganizationUnitDto>();
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in CreateOrganizationUnitAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldOrganizationUnit = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, false)
                ?? throw new KeyNotFoundException($"OrganizationUnit '{{organizationUnitGuid}}' was not found.");
            await _userScope.EnsureCanAccessOrganizationUnitAsync(oldOrganizationUnit.OrganizationUnitId);
            var oldOrganizationUnitScopes = (await _repository.OrganizationUnitScope.GetAllByOrganizationUnitGuidAsync(organizationUnitGuid)).ToList();

            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                // 2. Update Header
                var model = input.Adapt<OrganizationUnit>();
            model.ParentOrganizationUnitId = input.ParentOrganizationUnitId is null or 0 ? null : input.ParentOrganizationUnitId;
                model.OrganizationUnitGuid = organizationUnitGuid;
                model.OrganizationUnitId = oldOrganizationUnit.OrganizationUnitId;

            if (model.ParentOrganizationUnitId != oldOrganizationUnit.ParentOrganizationUnitId)
            {
                if (model.ParentOrganizationUnitId == null)
                {
                    model.LevelDepth = 1;
                    model.HierarchyPath = $"/{model.OrganizationUnitId}/";
                }
                else
                {
                    var parent = await _repository.OrganizationUnit.GetOrganizationUnitByIdAsync(model.ParentOrganizationUnitId.Value, false);
                    if (parent != null)
                    {
                        if (!string.IsNullOrEmpty(oldOrganizationUnit.HierarchyPath) && parent.HierarchyPath?.StartsWith(oldOrganizationUnit.HierarchyPath) == true)
                        {
                            throw new InvalidOperationException("Cannot set parent to a descendant node.");
                        }
                        
                        model.LevelDepth = parent.LevelDepth + 1;
                        model.HierarchyPath = $"{parent.HierarchyPath}{model.OrganizationUnitId}/";
                    }
                    else
                    {
                        model.LevelDepth = 1;
                        model.HierarchyPath = $"/{model.OrganizationUnitId}/";
                    }
                }

                var depthDiff = model.LevelDepth - oldOrganizationUnit.LevelDepth;
                if (!string.IsNullOrEmpty(oldOrganizationUnit.HierarchyPath))
                {
                    var descendants = (await _repository.OrganizationUnit.SearchOrganizationUnitAsync(
                        null, null, null, null, null, null, null, null,
                        oldOrganizationUnit.HierarchyPath, "StartsWith", transaction))
                        .Where(d => d.OrganizationUnitId != oldOrganizationUnit.OrganizationUnitId)
                        .ToList();

                    foreach (var descendant in descendants)
                    {
                        descendant.HierarchyPath = model.HierarchyPath + descendant.HierarchyPath!.Substring(oldOrganizationUnit.HierarchyPath.Length);
                        descendant.LevelDepth += depthDiff;
                        await _repository.OrganizationUnit.UpdateOrganizationUnitAsync(descendant, transaction);
                    }
                }
            }
            else
            {
                model.LevelDepth = oldOrganizationUnit.LevelDepth;
                model.HierarchyPath = oldOrganizationUnit.HierarchyPath;
            }
                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await _repository.OrganizationUnit.UpdateOrganizationUnitAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "OrganizationUnit",
                        EntityKey = model.OrganizationUnitGuid.ToString(),
                        EntityDisplayName = model.OrganizationUnitCode,
                        ActionType = "UPDATE",
                        OldEntity = oldOrganizationUnit,
                        NewEntity = model
                    }
                };

                // 3. Child alignment/diff logic
                if (input.OrganizationUnitScopes != null)
                {
                    var newOrganizationUnitScopes = input.OrganizationUnitScopes.ToList();
                    var oldOrganizationUnitScopeDict = oldOrganizationUnitScopes.ToDictionary(o => o.OrganizationUnitScopeGuid);

                    foreach (var detailDto in newOrganizationUnitScopes)
                    {
                        var detail = detailDto.Adapt<OrganizationUnitScope>();
                        detail.OrganizationUnitId = model.OrganizationUnitId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.OrganizationUnitScopeGuid != Guid.Empty && oldOrganizationUnitScopeDict.ContainsKey(detailDto.OrganizationUnitScopeGuid))
                        {
                            detail.OrganizationUnitScopeGuid = detailDto.OrganizationUnitScopeGuid;
                            detail.UpdatedById = input.UpdatedById;
                            await _repository.OrganizationUnitScope.UpdateOrganizationUnitScopeAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "OrganizationUnitScope",
                                EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                                EntityDisplayName = detail.ScopeType,
                                ParentTableName = "OrganizationUnit",
                                ParentEntityKey = organizationUnitGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldOrganizationUnitScopeDict[detailDto.OrganizationUnitScopeGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.OrganizationUnitScopeGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await _repository.OrganizationUnitScope.CreateOrganizationUnitScopeAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "OrganizationUnitScope",
                                EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                                EntityDisplayName = detail.ScopeType,
                                ParentTableName = "OrganizationUnit",
                                ParentEntityKey = organizationUnitGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldOrganizationUnitScope in oldOrganizationUnitScopes)
                    {
                        if (!newOrganizationUnitScopes.Any(o => o.OrganizationUnitScopeGuid == oldOrganizationUnitScope.OrganizationUnitScopeGuid))
                        {
                            await _repository.OrganizationUnitScope.SoftDeleteOrganizationUnitScopeAsync(
                                new OrganizationUnitScope
                                {
                                    OrganizationUnitScopeGuid = oldOrganizationUnitScope.OrganizationUnitScopeGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "OrganizationUnitScope",
                                EntityKey = oldOrganizationUnitScope.OrganizationUnitScopeGuid.ToString(),
                                EntityDisplayName = oldOrganizationUnitScope.ScopeType,
                                ParentTableName = "OrganizationUnit",
                                ParentEntityKey = organizationUnitGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldOrganizationUnitScope,
                                NewEntity = null
                            });
                        }
                    }
                }

                // 4. Single audit session for all entities
                await _audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "OrganizationUnit",
                    RootEntityKey = model.OrganizationUnitGuid.ToString(),
                    RootDisplayName = model.OrganizationUnitCode,
                    UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await _transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in UpdateOrganizationUnitAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForDeleteDto input, bool trackChanges)
        {
            var oldOrganizationUnit = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, false);
            if (oldOrganizationUnit != null)
                await _userScope.EnsureCanAccessOrganizationUnitAsync(oldOrganizationUnit.OrganizationUnitId);
            var model = new OrganizationUnit
            {
                OrganizationUnitGuid = organizationUnitGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.OrganizationUnit.SoftDeleteOrganizationUnitAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "OrganizationUnit",
                RootEntityKey = organizationUnitGuid.ToString(),
                RootDisplayName = oldOrganizationUnit?.OrganizationUnitCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "OrganizationUnit",
                        EntityKey = organizationUnitGuid.ToString(),
                        EntityDisplayName = oldOrganizationUnit?.OrganizationUnitCode,
                        ActionType = "DELETE",
                        OldEntity = oldOrganizationUnit,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteOrganizationUnitByAdminAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            await _repository.OrganizationUnit.DeleteOrganizationUnitAsync(organizationUnitGuid);
        }

        public async Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitAsync(
            string? organizationUnitCode, string? organizationUnitCodeSearchType, string? organizationUnitName, string? organizationUnitNameSearchType, string? srOrganizationLevel, string? srOrganizationLevelSearchType, string? levelDepth, string? levelDepthSearchType, string? hierarchyPath, string? hierarchyPathSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var accessibleIds = (await _userScope.GetAccessibleOrganizationUnitsAsync())
                .Select(o => o.OrganizationUnitId)
                .ToHashSet();

            var data = await _repository.OrganizationUnit.SearchOrganizationUnitAsync(
organizationUnitCode, organizationUnitCodeSearchType, organizationUnitName, organizationUnitNameSearchType, srOrganizationLevel, srOrganizationLevelSearchType, levelDepth, levelDepthSearchType, hierarchyPath, hierarchyPathSearchType
                // ── FK Virtual Search Args ──
            );
            return data
                .Where(o => accessibleIds.Contains(o.OrganizationUnitId))
                .Adapt<IEnumerable<OrganizationUnitDto>>();
        }
    }
}
