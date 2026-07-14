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
    public partial class CostCenterService : ICostCenterService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public CostCenterService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public CostCenterService(
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

        public async Task<IEnumerable<CostCenterDto>> GetAllCostCentersAsync(bool trackChanges)
        {
            var entities = await _repository.CostCenter.GetAllCostCentersAsync(trackChanges);
            return entities.Adapt<IEnumerable<CostCenterDto>>();
        }

        public async Task<CostCenterDto> GetCostCenterByGuidAsync(Guid costCenterGuid, bool trackChanges)
        {
            var entity = await _repository.CostCenter.GetCostCenterAsync(costCenterGuid, trackChanges)
                ?? throw new KeyNotFoundException($"CostCenter with Guid '{costCenterGuid}' not found.");
            return entity.Adapt<CostCenterDto>();
        }

        public async Task<CostCenterDto> CreateCostCenterAsync(CostCenterForCreationDto input)
        {
            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<CostCenter>();
            model.ParentCostCenterId = input.ParentCostCenterId is null or 0 ? null : input.ParentCostCenterId;

            if (model.ParentCostCenterId == null)
            {
                model.LevelDepth = 1;
                model.HierarchyPath = "";
            }
            else
            {
                var parent = await _repository.CostCenter.GetCostCenterByIdAsync(model.ParentCostCenterId.Value, false);
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
                await _repository.CostCenter.CreateCostCenterAsync(model, transaction);

            model.HierarchyPath = string.IsNullOrEmpty(model.HierarchyPath)
                ? $"/{model.CostCenterId}/"
                : $"{model.HierarchyPath}{model.CostCenterId}/";
            await _repository.CostCenter.UpdateCostCenterAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "CostCenter",
                        EntityKey = model.CostCenterGuid.ToString(),
                        EntityDisplayName = model.CostCenterCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // Dapper doesn't automatically insert children, so we must manually insert nested Details
                                if (input.CostCenterScopes != null && input.CostCenterScopes.Any())
                {
                    foreach (var detailDto in input.CostCenterScopes)
                    {
                        var detail = detailDto.Adapt<CostCenterScope>();
                        detail.CostCenterId = model.CostCenterId;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await _repository.CostCenterScope.CreateCostCenterScopeAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "CostCenterScope",
                            EntityKey = detail.CostCenterScopeGuid.ToString(),
                            EntityDisplayName = detail.ScopeType,
                            ParentTableName = "CostCenter",
                            ParentEntityKey = model.CostCenterGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // ##HeaderDetailCreateBlock##
                if (input.CostCenterAssignments != null && input.CostCenterAssignments.Any())
                {
                    foreach (var detailDto in input.CostCenterAssignments)
                    {
                        var detail = detailDto.Adapt<CostCenterAssignment>();
                        detail.CostCenterId = model.CostCenterId;
                    detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await _repository.CostCenterAssignment.CreateCostCenterAssignmentAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "CostCenterAssignment",
                            EntityKey = detail.CostCenterAssignmentGuid.ToString(),
                            EntityDisplayName = detail.EffectiveDate.ToString(),
                            ParentTableName = "CostCenter",
                            ParentEntityKey = model.CostCenterGuid.ToString(),
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
                    RootTableName = "CostCenter",
                    RootEntityKey = model.CostCenterGuid.ToString(),
                    RootDisplayName = model.CostCenterCode,
                    UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await _transactionManager.CommitAsync();
                return model.Adapt<CostCenterDto>();
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in CreateCostCenterAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateCostCenterAsync(Guid costCenterGuid, CostCenterForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldCostCenter = await _repository.CostCenter.GetCostCenterAsync(costCenterGuid, false)
                ?? throw new KeyNotFoundException($"CostCenter '{{costCenterGuid}}' was not found.");
            var oldCostCenterAssignments = (await _repository.CostCenterAssignment.GetAllByCostCenterGuidAsync(costCenterGuid)).ToList();
                        var oldCostCenterScopes = (await _repository.CostCenterScope.GetAllByCostCenterGuidAsync(costCenterGuid)).ToList();
            // ##HeaderDetailUpdateFetch##

            await _transactionManager.BeginTransactionAsync();
            var transaction = _transactionManager.GetTransaction();
            try
            {
                // 2. Update Header
                var model = input.Adapt<CostCenter>();
            model.ParentCostCenterId = input.ParentCostCenterId is null or 0 ? null : input.ParentCostCenterId;
                model.CostCenterGuid = costCenterGuid;
                model.CostCenterId = oldCostCenter.CostCenterId;

            if (model.ParentCostCenterId != oldCostCenter.ParentCostCenterId)
            {
                if (model.ParentCostCenterId == null)
                {
                    model.LevelDepth = 1;
                    model.HierarchyPath = $"/{model.CostCenterId}/";
                }
                else
                {
                    var parent = await _repository.CostCenter.GetCostCenterByIdAsync(model.ParentCostCenterId.Value, false);
                    if (parent != null)
                    {
                        if (!string.IsNullOrEmpty(oldCostCenter.HierarchyPath) && parent.HierarchyPath?.StartsWith(oldCostCenter.HierarchyPath) == true)
                        {
                            throw new InvalidOperationException("Cannot set parent to a descendant node.");
                        }
                        
                        model.LevelDepth = parent.LevelDepth + 1;
                        model.HierarchyPath = $"{parent.HierarchyPath}{model.CostCenterId}/";
                    }
                    else
                    {
                        model.LevelDepth = 1;
                        model.HierarchyPath = $"/{model.CostCenterId}/";
                    }
                }

                var depthDiff = model.LevelDepth - oldCostCenter.LevelDepth;
                if (!string.IsNullOrEmpty(oldCostCenter.HierarchyPath))
                {
                    var descendants = (await _repository.CostCenter.SearchCostCenterAsync(
                        null, null, null, null, null, null, null, null, null, null, oldCostCenter.HierarchyPath, "StartsWith", transaction))
                        .Where(d => d.CostCenterId != oldCostCenter.CostCenterId)
                        .ToList();

                    foreach (var descendant in descendants)
                    {
                        descendant.HierarchyPath = model.HierarchyPath + descendant.HierarchyPath!.Substring(oldCostCenter.HierarchyPath.Length);
                        descendant.LevelDepth += depthDiff;
                        await _repository.CostCenter.UpdateCostCenterAsync(descendant, transaction);
                    }
                }
            }
            else
            {
                model.LevelDepth = oldCostCenter.LevelDepth;
                model.HierarchyPath = oldCostCenter.HierarchyPath;
            }
                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await _repository.CostCenter.UpdateCostCenterAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "CostCenter",
                        EntityKey = model.CostCenterGuid.ToString(),
                        EntityDisplayName = model.CostCenterCode,
                        ActionType = "UPDATE",
                        OldEntity = oldCostCenter,
                        NewEntity = model
                    }
                };

                // 3. Child alignment/diff logic
                                if (input.CostCenterScopes != null)
                {
                    var newCostCenterScopes = input.CostCenterScopes.ToList();
                    var oldCostCenterScopeDict = oldCostCenterScopes.ToDictionary(o => o.CostCenterScopeGuid);

                    foreach (var detailDto in newCostCenterScopes)
                    {
                        var detail = detailDto.Adapt<CostCenterScope>();
                        detail.CostCenterId = model.CostCenterId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.CostCenterScopeGuid != Guid.Empty && oldCostCenterScopeDict.ContainsKey(detailDto.CostCenterScopeGuid))
                        {
                            detail.CostCenterScopeGuid = detailDto.CostCenterScopeGuid;
                            detail.CostCenterScopeId = oldCostCenterScopeDict[detailDto.CostCenterScopeGuid].CostCenterScopeId;
                            await _repository.CostCenterScope.UpdateCostCenterScopeAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterScope",
                                EntityKey = detail.CostCenterScopeGuid.ToString(),
                                EntityDisplayName = detail.ScopeType,
                                ParentTableName = "CostCenter",
                                ParentEntityKey = model.CostCenterGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldCostCenterScopeDict[detailDto.CostCenterScopeGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await _repository.CostCenterScope.CreateCostCenterScopeAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterScope",
                                EntityKey = detail.CostCenterScopeGuid.ToString(),
                                EntityDisplayName = detail.ScopeType,
                                ParentTableName = "CostCenter",
                                ParentEntityKey = model.CostCenterGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldCostCenterScope in oldCostCenterScopes)
                    {
                        if (!newCostCenterScopes.Any(o => o.CostCenterScopeGuid == oldCostCenterScope.CostCenterScopeGuid))
                        {
                            await _repository.CostCenterScope.SoftDeleteCostCenterScopeAsync(
                                new CostCenterScope
                                {
                                    CostCenterScopeGuid = oldCostCenterScope.CostCenterScopeGuid,
                                    CostCenterScopeId = oldCostCenterScope.CostCenterScopeId,
                                    CostCenterId = oldCostCenterScope.CostCenterId
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterScope",
                                EntityKey = oldCostCenterScope.CostCenterScopeGuid.ToString(),
                                EntityDisplayName = oldCostCenterScope.ScopeType,
                                ParentTableName = "CostCenter",
                                ParentEntityKey = model.CostCenterGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldCostCenterScope,
                                NewEntity = null
                            });
                        }
                    }
                }

                // ##HeaderDetailUpdateDiff##
                if (input.CostCenterAssignments != null)
                {
                    var newCostCenterAssignments = input.CostCenterAssignments.ToList();
                    var oldCostCenterAssignmentDict = oldCostCenterAssignments.ToDictionary(o => o.CostCenterAssignmentGuid);

                    foreach (var detailDto in newCostCenterAssignments)
                    {
                        var detail = detailDto.Adapt<CostCenterAssignment>();
                        detail.CostCenterId = model.CostCenterId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.CostCenterAssignmentGuid != Guid.Empty && oldCostCenterAssignmentDict.ContainsKey(detailDto.CostCenterAssignmentGuid))
                        {
                            detail.CostCenterAssignmentGuid = detailDto.CostCenterAssignmentGuid;
                            detail.UpdatedById = input.UpdatedById;
                            await _repository.CostCenterAssignment.UpdateCostCenterAssignmentAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterAssignment",
                                EntityKey = detail.CostCenterAssignmentGuid.ToString(),
                                EntityDisplayName = detail.EffectiveDate.ToString(),
                                ParentTableName = "CostCenter",
                                ParentEntityKey = costCenterGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldCostCenterAssignmentDict[detailDto.CostCenterAssignmentGuid],
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.CostCenterAssignmentGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await _repository.CostCenterAssignment.CreateCostCenterAssignmentAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterAssignment",
                                EntityKey = detail.CostCenterAssignmentGuid.ToString(),
                                EntityDisplayName = detail.EffectiveDate.ToString(),
                                ParentTableName = "CostCenter",
                                ParentEntityKey = costCenterGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldCostCenterAssignment in oldCostCenterAssignments)
                    {
                        if (!newCostCenterAssignments.Any(o => o.CostCenterAssignmentGuid == oldCostCenterAssignment.CostCenterAssignmentGuid))
                        {
                            await _repository.CostCenterAssignment.SoftDeleteCostCenterAssignmentAsync(
                                new CostCenterAssignment
                                {
                                    CostCenterAssignmentGuid = oldCostCenterAssignment.CostCenterAssignmentGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "CostCenterAssignment",
                                EntityKey = oldCostCenterAssignment.CostCenterAssignmentGuid.ToString(),
                                EntityDisplayName = oldCostCenterAssignment.EffectiveDate.ToString(),
                                ParentTableName = "CostCenter",
                                ParentEntityKey = costCenterGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldCostCenterAssignment,
                                NewEntity = null
                            });
                        }
                    }
                }

                // 4. Single audit session for all entities
                await _audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "CostCenter",
                    RootEntityKey = model.CostCenterGuid.ToString(),
                    RootDisplayName = model.CostCenterCode,
                    UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await _transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transactionManager.RollbackAsync();
                _logger.LogError($"Error in UpdateCostCenterAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCostCenterAsync(Guid costCenterGuid, CostCenterForDeleteDto input, bool trackChanges)
        {
            var oldCostCenter = await _repository.CostCenter.GetCostCenterAsync(costCenterGuid, false);
            var model = new CostCenter
            {
                CostCenterGuid = costCenterGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.CostCenter.SoftDeleteCostCenterAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "CostCenter",
                RootEntityKey = costCenterGuid.ToString(),
                RootDisplayName = oldCostCenter?.CostCenterCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CostCenter",
                        EntityKey = costCenterGuid.ToString(),
                        EntityDisplayName = oldCostCenter?.CostCenterCode,
                        ActionType = "DELETE",
                        OldEntity = oldCostCenter,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteCostCenterByAdminAsync(Guid costCenterGuid, bool trackChanges)
        {
            await _repository.CostCenter.DeleteCostCenterAsync(costCenterGuid);
        }

        public async Task<IEnumerable<CostCenterDto>> SearchCostCenterAsync(
            string? costCenterCode, string? costCenterCodeSearchType, string? costCenterName, string? costCenterNameSearchType, string? costCenterDescription, string? costCenterDescriptionSearchType, string? parentCostCenterId, string? parentCostCenterIdSearchType, string? levelDepth, string? levelDepthSearchType, string? hierarchyPath, string? hierarchyPathSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await _repository.CostCenter.SearchCostCenterAsync(
costCenterCode, costCenterCodeSearchType, costCenterName, costCenterNameSearchType, costCenterDescription, costCenterDescriptionSearchType, parentCostCenterId, parentCostCenterIdSearchType, levelDepth, levelDepthSearchType, hierarchyPath, hierarchyPathSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<CostCenterDto>>();
        }
    }
}
