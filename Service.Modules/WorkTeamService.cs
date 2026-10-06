using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class WorkTeamService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IWorkTeamService
    {
        public WorkTeamService(IRepositoryManager repository, ILoggerManager logger)
            : this(repository, logger, null!, null!, null!)
        {
        }

        public async Task<IEnumerable<WorkTeamDto>> GetAllWorkTeamsAsync(bool trackChanges)
        {
            var entities = await repository.WorkTeam.GetAllWorkTeamsAsync(trackChanges);
            return entities.Adapt<IEnumerable<WorkTeamDto>>();
        }

        public async Task<WorkTeamDto?> GetWorkTeamByGuidAsync(Guid workTeamGuid, bool trackChanges)
        {
            var entity = await repository.WorkTeam.GetWorkTeamAsync(workTeamGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<WorkTeamDto>();
            return dto;
        }

        public async Task<WorkTeamDto> CreateWorkTeamAsync(WorkTeamForCreationDto input)
        {
            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<WorkTeam>();


                model.StatusId = 1;
                model.CreatedTime = DateTime.UtcNow;
                await repository.WorkTeam.CreateWorkTeamAsync(model, transaction);


                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "WorkTeam",
                        EntityKey = model.WorkTeamGuid.ToString(),
                        EntityDisplayName = model.WorkTeamCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                // Dapper doesn't automatically insert children, so we must manually insert nested Details
                // ##HeaderDetailCreateBlock##
                if (input.WorkTeamCapabilities != null && input.WorkTeamCapabilities.Any())
                {
                    foreach (var detailDto in input.WorkTeamCapabilities)
                    {
                        var detail = detailDto.Adapt<WorkTeamCapability>();
                        detail.WorkTeamId = model.WorkTeamId;
                    detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.WorkTeamCapability.CreateWorkTeamCapabilityAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "WorkTeamCapability",
                            EntityKey = detail.WorkTeamCapabilityGuid.ToString(),
                            EntityDisplayName = detail.WorkTeamCapabilityGuid.ToString(),
                            ParentTableName = "WorkTeam",
                            ParentEntityKey = model.WorkTeamGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                // Single audit session for all entities
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "WorkTeam",
                    RootEntityKey = model.WorkTeamGuid.ToString(),
                    RootDisplayName = model.WorkTeamCode,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
                return model.Adapt<WorkTeamDto>();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in CreateWorkTeamAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateWorkTeamAsync(Guid workTeamGuid, WorkTeamForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldWorkTeam = await repository.WorkTeam.GetWorkTeamAsync(workTeamGuid, false)
                ?? throw new KeyNotFoundException($"WorkTeam '{{workTeamGuid}}' was not found.");
            var oldWorkTeamCapabilities = (await repository.WorkTeamCapability.GetAllByWorkTeamGuidAsync(workTeamGuid)).ToList();
            // ##HeaderDetailUpdateFetch##

            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                // 2. Update Header
                var model = input.Adapt<WorkTeam>();

                model.WorkTeamGuid = workTeamGuid;
                model.WorkTeamId = oldWorkTeam.WorkTeamId;

                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await repository.WorkTeam.UpdateWorkTeamAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "WorkTeam",
                        EntityKey = model.WorkTeamGuid.ToString(),
                        EntityDisplayName = model.WorkTeamCode,
                        ActionType = "UPDATE",
                        OldEntity = oldWorkTeam,
                        NewEntity = model
                    }
                };

                // 3. Child alignment/diff logic
                // ##HeaderDetailUpdateDiff##
                if (input.WorkTeamCapabilities != null)
                {
                    var newWorkTeamCapabilities = input.WorkTeamCapabilities.ToList();
                    var oldWorkTeamCapabilityDict = oldWorkTeamCapabilities.ToDictionary(o => o.WorkTeamCapabilityGuid);

                    foreach (var detailDto in newWorkTeamCapabilities)
                    {
                        var detail = detailDto.Adapt<WorkTeamCapability>();
                        detail.WorkTeamId = model.WorkTeamId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.WorkTeamCapabilityGuid != Guid.Empty && oldWorkTeamCapabilityDict.TryGetValue(detailDto.WorkTeamCapabilityGuid, out var oldDetailRow))
                        {
                            detail.WorkTeamCapabilityGuid = detailDto.WorkTeamCapabilityGuid;
                            detail.UpdatedById = input.UpdatedById;
                            await repository.WorkTeamCapability.UpdateWorkTeamCapabilityAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "WorkTeamCapability",
                                EntityKey = detail.WorkTeamCapabilityGuid.ToString(),
                                EntityDisplayName = detail.WorkTeamCapabilityGuid.ToString(),
                                ParentTableName = "WorkTeam",
                                ParentEntityKey = workTeamGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldDetailRow,
                                NewEntity = detail
                            });
                        }
                        else
                        {
                            detail.WorkTeamCapabilityGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.WorkTeamCapability.CreateWorkTeamCapabilityAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "WorkTeamCapability",
                                EntityKey = detail.WorkTeamCapabilityGuid.ToString(),
                                EntityDisplayName = detail.WorkTeamCapabilityGuid.ToString(),
                                ParentTableName = "WorkTeam",
                                ParentEntityKey = workTeamGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });
                        }
                    }

                    foreach (var oldWorkTeamCapability in oldWorkTeamCapabilities)
                    {
                        if (!newWorkTeamCapabilities.Any(o => o.WorkTeamCapabilityGuid == oldWorkTeamCapability.WorkTeamCapabilityGuid))
                        {
                            await repository.WorkTeamCapability.SoftDeleteWorkTeamCapabilityAsync(
                                new WorkTeamCapability
                                {
                                    WorkTeamCapabilityGuid = oldWorkTeamCapability.WorkTeamCapabilityGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "WorkTeamCapability",
                                EntityKey = oldWorkTeamCapability.WorkTeamCapabilityGuid.ToString(),
                                EntityDisplayName = oldWorkTeamCapability.WorkTeamCapabilityGuid.ToString(),
                                ParentTableName = "WorkTeam",
                                ParentEntityKey = workTeamGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldWorkTeamCapability,
                                NewEntity = null
                            });
                        }
                    }
                }

                // 4. Single audit session for all entities
                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "WorkTeam",
                    RootEntityKey = model.WorkTeamGuid.ToString(),
                    RootDisplayName = model.WorkTeamCode,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in UpdateWorkTeamAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteWorkTeamAsync(Guid workTeamGuid, WorkTeamForDeleteDto input, bool trackChanges)
        {
            var oldWorkTeam = await repository.WorkTeam.GetWorkTeamAsync(workTeamGuid, false);
            var model = new WorkTeam
            {
                WorkTeamGuid = workTeamGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.WorkTeam.SoftDeleteWorkTeamAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "WorkTeam",
                RootEntityKey = workTeamGuid.ToString(),
                RootDisplayName = oldWorkTeam?.WorkTeamCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "WorkTeam",
                        EntityKey = workTeamGuid.ToString(),
                        EntityDisplayName = oldWorkTeam?.WorkTeamCode,
                        ActionType = "DELETE",
                        OldEntity = oldWorkTeam,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteWorkTeamByAdminAsync(Guid workTeamGuid, bool trackChanges)
        {
            await repository.WorkTeam.DeleteWorkTeamAsync(workTeamGuid);
        }

        public async Task<IEnumerable<WorkTeamDto>> SearchWorkTeamAsync(
            string? workTeamCode, string? workTeamCodeSearchType, string? workTeamName, string? workTeamNameSearchType, string? isEditAble, string? isEditAbleSearchType, string? note, string? noteSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await repository.WorkTeam.SearchWorkTeamAsync(
workTeamCode, workTeamCodeSearchType, workTeamName, workTeamNameSearchType, isEditAble, isEditAbleSearchType, note, noteSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<WorkTeamDto>>();
        }
    }
}
