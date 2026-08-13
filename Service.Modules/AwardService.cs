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
    public partial class AwardService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IAwardService
    {
        public AwardService(IRepositoryManager repository, ILoggerManager logger)
            : this(repository, logger, null!, null!, null!)
        {
        }

        public async Task<IEnumerable<AwardDto>> GetAllAwardsAsync(bool trackChanges)
        {
            var entities = await repository.Award.GetAllAwardsAsync(trackChanges);
            return entities.Adapt<IEnumerable<AwardDto>>();
        }

        public async Task<AwardDto?> GetAwardByGuidAsync(Guid awardGuid, bool trackChanges)
        {
            var entity = await repository.Award.GetAwardAsync(awardGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<AwardDto>();

            var detailEntities = await repository.AwardCriteria.GetAllByAwardIdAsync(entity.AwardId);
            var detailDtos = detailEntities.Adapt<List<AwardCriteriaDto>>();

            foreach (var detailDto in detailDtos)
            {
                var subEntities = await repository.AwardCriteriaRule.GetAllByAwardCriteriaIdAsync(detailDto.AwardCriteriaId);
                detailDto.AwardCriteriaRules = subEntities.Adapt<List<AwardCriteriaRuleDto>>();
            }

            dto.AwardCriteria = detailDtos;
            return dto;
        }

        public async Task<AwardDto> CreateAwardAsync(AwardForCreationDto input)
        {
            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<Award>();

                model.AwardGuid = model.AwardGuid == Guid.Empty ? Guid.NewGuid() : model.AwardGuid;
                model.StatusId = 1;
                model.CreatedTime = DateTime.UtcNow;
                await repository.Award.CreateAwardAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Award",
                        EntityKey = model.AwardGuid.ToString(),
                        EntityDisplayName = model.AwardCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                };

                if (input.AwardCriteria != null && input.AwardCriteria.Any())
                {
                    foreach (var detailDto in input.AwardCriteria)
                    {
                        var detail = detailDto.Adapt<AwardCriteria>();
                        detail.AwardId = model.AwardId;
                        detail.AwardCriteriaGuid = detail.AwardCriteriaGuid == Guid.Empty
                            ? Guid.NewGuid()
                            : detail.AwardCriteriaGuid;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await repository.AwardCriteria.CreateAwardCriteriaAsync(detail, transaction);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "AwardCriteria",
                            EntityKey = detail.AwardCriteriaGuid.ToString(),
                            EntityDisplayName = detail.CriteriaCode,
                            ParentTableName = "Award",
                            ParentEntityKey = model.AwardGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });

                        if (detailDto.AwardCriteriaRules != null && detailDto.AwardCriteriaRules.Any())
                        {
                            foreach (var subDetailDto in detailDto.AwardCriteriaRules)
                            {
                                var subDetail = subDetailDto.Adapt<AwardCriteriaRule>();
                                subDetail.AwardCriteriaId = detail.AwardCriteriaId;
                                subDetail.AwardCriteriaRuleGuid = subDetail.AwardCriteriaRuleGuid == Guid.Empty
                                    ? Guid.NewGuid()
                                    : subDetail.AwardCriteriaRuleGuid;
                                subDetail.StatusId = 1;
                                subDetail.CreatedTime = DateTime.UtcNow;
                                await repository.AwardCriteriaRule.CreateAwardCriteriaRuleAsync(subDetail, transaction);

                                entries.Add(new AuditLogEntry
                                {
                                    TableName = "AwardCriteriaRule",
                                    EntityKey = subDetail.AwardCriteriaRuleGuid.ToString(),
                                    EntityDisplayName = subDetail.RuleCode,
                                    ParentTableName = "AwardCriteria",
                                    ParentEntityKey = detail.AwardCriteriaGuid.ToString(),
                                    ActionType = "CREATE",
                                    OldEntity = null,
                                    NewEntity = subDetail
                                });
                            }
                        }
                    }
                }

                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "CREATE",
                    RootTableName = "Award",
                    RootEntityKey = model.AwardGuid.ToString(),
                    RootDisplayName = model.AwardCode,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
                return model.Adapt<AwardDto>();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in CreateAwardAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAwardAsync(Guid awardGuid, AwardForUpdateDto input, bool trackChanges)
        {
            var oldAward = await repository.Award.GetAwardAsync(awardGuid, false)
                ?? throw new KeyNotFoundException($"Award '{{awardGuid}}' was not found.");
            var oldAwardCriteria = (await repository.AwardCriteria.GetAllByAwardIdAsync(oldAward.AwardId)).ToList();

            var oldSubDetailsDict = new Dictionary<long, List<AwardCriteriaRule>>();
            foreach (var oldDetail in oldAwardCriteria)
            {
                var subList = (await repository.AwardCriteriaRule.GetAllByAwardCriteriaIdAsync(oldDetail.AwardCriteriaId)).ToList();
                oldSubDetailsDict[oldDetail.AwardCriteriaId] = subList;
            }

            await transactionManager.BeginTransactionAsync();
            var transaction = transactionManager.GetTransaction();
            try
            {
                var model = input.Adapt<Award>();

                model.AwardGuid = awardGuid;
                model.AwardId = oldAward.AwardId;
                model.StatusId = 2;
                model.UpdatedById = input.UpdatedById;
                model.UpdatedTime = DateTime.UtcNow;
                await repository.Award.UpdateAwardAsync(model, transaction);

                var entries = new List<AuditLogEntry>
                {
                    new()
                    {
                        TableName = "Award",
                        EntityKey = model.AwardGuid.ToString(),
                        EntityDisplayName = model.AwardCode,
                        ActionType = "UPDATE",
                        OldEntity = oldAward,
                        NewEntity = model
                    }
                };

                if (input.AwardCriteria != null)
                {
                    var newAwardCriteria = input.AwardCriteria.ToList();
                    var oldAwardCriteriaDict = oldAwardCriteria.ToDictionary(o => o.AwardCriteriaGuid);

                    foreach (var detailDto in newAwardCriteria)
                    {
                        var detail = detailDto.Adapt<AwardCriteria>();
                        detail.AwardId = model.AwardId;
                        detail.StatusId = 2;
                        detail.UpdatedTime = DateTime.UtcNow;

                        if (detailDto.AwardCriteriaGuid != Guid.Empty && oldAwardCriteriaDict.TryGetValue(detailDto.AwardCriteriaGuid, out var oldDetailRow))
                        {
                            detail.AwardCriteriaGuid = detailDto.AwardCriteriaGuid;
                            // DTOs do not expose internal IDs. Preserve the existing numeric
                            // key so child Rules always retain their actual parent relationship.
                            detail.AwardCriteriaId = oldDetailRow.AwardCriteriaId;
                            detail.UpdatedById = input.UpdatedById;
                            await repository.AwardCriteria.UpdateAwardCriteriaAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "AwardCriteria",
                                EntityKey = detail.AwardCriteriaGuid.ToString(),
                                EntityDisplayName = detail.CriteriaCode,
                                ParentTableName = "Award",
                                ParentEntityKey = awardGuid.ToString(),
                                ActionType = "UPDATE",
                                OldEntity = oldDetailRow,
                                NewEntity = detail
                            });

                            if (detailDto.AwardCriteriaRules != null)
                            {
                                var oldSubDetails = oldSubDetailsDict.TryGetValue(oldDetailRow.AwardCriteriaId, out var existingSubs)
                                    ? existingSubs
                                    : new List<AwardCriteriaRule>();
                                var oldSubDetailDict = oldSubDetails.ToDictionary(s => s.AwardCriteriaRuleGuid);
                                var newSubDetails = detailDto.AwardCriteriaRules.ToList();

                                foreach (var subDto in newSubDetails)
                                {
                                    var subDetail = subDto.Adapt<AwardCriteriaRule>();
                                    subDetail.AwardCriteriaId = detail.AwardCriteriaId;
                                    subDetail.StatusId = 2;
                                    subDetail.UpdatedTime = DateTime.UtcNow;

                                    if (subDto.AwardCriteriaRuleGuid != Guid.Empty && oldSubDetailDict.TryGetValue(subDto.AwardCriteriaRuleGuid, out var oldSubRow))
                                    {
                                        subDetail.AwardCriteriaRuleGuid = subDto.AwardCriteriaRuleGuid;
                                        subDetail.UpdatedById = input.UpdatedById;
                                        await repository.AwardCriteriaRule.UpdateAwardCriteriaRuleAsync(subDetail, transaction);

                                        entries.Add(new AuditLogEntry
                                        {
                                            TableName = "AwardCriteriaRule",
                                            EntityKey = subDetail.AwardCriteriaRuleGuid.ToString(),
                                            EntityDisplayName = subDetail.RuleCode,
                                            ParentTableName = "AwardCriteria",
                                            ParentEntityKey = detail.AwardCriteriaGuid.ToString(),
                                            ActionType = "UPDATE",
                                            OldEntity = oldSubRow,
                                            NewEntity = subDetail
                                        });
                                    }
                                    else
                                    {
                                        subDetail.AwardCriteriaRuleGuid = Guid.NewGuid();
                                        subDetail.CreatedById = input.UpdatedById;
                                        subDetail.StatusId = 1;
                                        subDetail.CreatedTime = DateTime.UtcNow;
                                        await repository.AwardCriteriaRule.CreateAwardCriteriaRuleAsync(subDetail, transaction);

                                        entries.Add(new AuditLogEntry
                                        {
                                            TableName = "AwardCriteriaRule",
                                            EntityKey = subDetail.AwardCriteriaRuleGuid.ToString(),
                                            EntityDisplayName = subDetail.RuleCode,
                                            ParentTableName = "AwardCriteria",
                                            ParentEntityKey = detail.AwardCriteriaGuid.ToString(),
                                            ActionType = "CREATE",
                                            OldEntity = null,
                                            NewEntity = subDetail
                                        });
                                    }
                                }

                                foreach (var oldSub in oldSubDetails)
                                {
                                    if (!newSubDetails.Any(s => s.AwardCriteriaRuleGuid == oldSub.AwardCriteriaRuleGuid))
                                    {
                                        await repository.AwardCriteriaRule.SoftDeleteAwardCriteriaRuleAsync(
                                            new AwardCriteriaRule
                                            {
                                                AwardCriteriaRuleGuid = oldSub.AwardCriteriaRuleGuid,
                                                DeletedById = input.UpdatedById,
                                                DeletedTime = DateTime.UtcNow
                                            },
                                            input.UpdatedById,
                                            transaction);

                                        entries.Add(new AuditLogEntry
                                        {
                                            TableName = "AwardCriteriaRule",
                                            EntityKey = oldSub.AwardCriteriaRuleGuid.ToString(),
                                            EntityDisplayName = oldSub.RuleCode,
                                            ParentTableName = "AwardCriteria",
                                            ParentEntityKey = detail.AwardCriteriaGuid.ToString(),
                                            ActionType = "DELETE",
                                            OldEntity = oldSub,
                                            NewEntity = null
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            detail.AwardCriteriaGuid = Guid.NewGuid();
                            detail.CreatedById = input.UpdatedById;
                            detail.StatusId = 1;
                            detail.CreatedTime = DateTime.UtcNow;
                            await repository.AwardCriteria.CreateAwardCriteriaAsync(detail, transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "AwardCriteria",
                                EntityKey = detail.AwardCriteriaGuid.ToString(),
                                EntityDisplayName = detail.CriteriaCode,
                                ParentTableName = "Award",
                                ParentEntityKey = awardGuid.ToString(),
                                ActionType = "CREATE",
                                OldEntity = null,
                                NewEntity = detail
                            });

                            if (detailDto.AwardCriteriaRules != null)
                            {
                                foreach (var subDto in detailDto.AwardCriteriaRules)
                                {
                                    var subDetail = subDto.Adapt<AwardCriteriaRule>();
                                    subDetail.AwardCriteriaId = detail.AwardCriteriaId;
                                    subDetail.AwardCriteriaRuleGuid = Guid.NewGuid();
                                    subDetail.CreatedById = input.UpdatedById;
                                    subDetail.StatusId = 1;
                                    subDetail.CreatedTime = DateTime.UtcNow;
                                    await repository.AwardCriteriaRule.CreateAwardCriteriaRuleAsync(subDetail, transaction);

                                    entries.Add(new AuditLogEntry
                                    {
                                        TableName = "AwardCriteriaRule",
                                        EntityKey = subDetail.AwardCriteriaRuleGuid.ToString(),
                                        EntityDisplayName = subDetail.RuleCode,
                                        ParentTableName = "AwardCriteria",
                                        ParentEntityKey = detail.AwardCriteriaGuid.ToString(),
                                        ActionType = "CREATE",
                                        OldEntity = null,
                                        NewEntity = subDetail
                                    });
                                }
                            }
                        }
                    }

                    foreach (var oldDetail in oldAwardCriteria)
                    {
                        if (!newAwardCriteria.Any(o => o.AwardCriteriaGuid == oldDetail.AwardCriteriaGuid))
                        {
                            // Keep hierarchy status consistent: a deleted Detail must not
                            // leave active SubDetails behind.
                            if (oldSubDetailsDict.TryGetValue(oldDetail.AwardCriteriaId, out var orphanedSubDetails))
                            {
                                foreach (var oldSub in orphanedSubDetails)
                                {
                                    await repository.AwardCriteriaRule.SoftDeleteAwardCriteriaRuleAsync(
                                        new AwardCriteriaRule
                                        {
                                            AwardCriteriaRuleGuid = oldSub.AwardCriteriaRuleGuid,
                                            DeletedById = input.UpdatedById,
                                            DeletedTime = DateTime.UtcNow
                                        },
                                        input.UpdatedById,
                                        transaction);

                                    entries.Add(new AuditLogEntry
                                    {
                                        TableName = "AwardCriteriaRule",
                                        EntityKey = oldSub.AwardCriteriaRuleGuid.ToString(),
                                        EntityDisplayName = oldSub.RuleCode,
                                        ParentTableName = "AwardCriteria",
                                        ParentEntityKey = oldDetail.AwardCriteriaGuid.ToString(),
                                        ActionType = "DELETE",
                                        OldEntity = oldSub,
                                        NewEntity = null
                                    });
                                }
                            }

                            await repository.AwardCriteria.SoftDeleteAwardCriteriaAsync(
                                new AwardCriteria
                                {
                                    AwardCriteriaGuid = oldDetail.AwardCriteriaGuid,
                                    DeletedById = input.UpdatedById,
                                    DeletedTime = DateTime.UtcNow
                                },
                                input.UpdatedById,
                                transaction);

                            entries.Add(new AuditLogEntry
                            {
                                TableName = "AwardCriteria",
                                EntityKey = oldDetail.AwardCriteriaGuid.ToString(),
                                EntityDisplayName = oldDetail.CriteriaCode,
                                ParentTableName = "Award",
                                ParentEntityKey = awardGuid.ToString(),
                                ActionType = "DELETE",
                                OldEntity = oldDetail,
                                NewEntity = null
                            });
                        }
                    }
                }

                await audit.LogSessionAsync(new AuditSessionInput
                {
                    SessionType = "UPDATE",
                    RootTableName = "Award",
                    RootEntityKey = model.AwardGuid.ToString(),
                    RootDisplayName = model.AwardCode,
                    UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                    Entries = entries
                });

                await transactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await transactionManager.RollbackAsync();
                logger.LogError($"Error in UpdateAwardAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAwardAsync(Guid awardGuid, AwardForDeleteDto input, bool trackChanges)
        {
            var oldAward = await repository.Award.GetAwardAsync(awardGuid, false);
            var model = new Award
            {
                AwardGuid = awardGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Award.SoftDeleteAwardAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Award",
                RootEntityKey = awardGuid.ToString(),
                RootDisplayName = oldAward?.AwardCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Award",
                        EntityKey = awardGuid.ToString(),
                        EntityDisplayName = oldAward?.AwardCode,
                        ActionType = "DELETE",
                        OldEntity = oldAward,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteAwardByAdminAsync(Guid awardGuid, bool trackChanges)
        {
            await repository.Award.DeleteAwardAsync(awardGuid);
        }

        public async Task<IEnumerable<AwardDto>> SearchAwardAsync(
            string? awardCode, string? awardCodeSearchType, string? awardName, string? awardNameSearchType, string? validFrom, string? validFromSearchType, string? validto, string? validtoSearchType, string? awardPrize, string? awardPrizeSearchType, string? note, string? noteSearchType
            // ── FK Virtual Search Params ──
        )
        {
            var data = await repository.Award.SearchAwardAsync(
awardCode, awardCodeSearchType, awardName, awardNameSearchType, validFrom, validFromSearchType, validto, validtoSearchType, awardPrize, awardPrizeSearchType, note, noteSearchType
                // ── FK Virtual Search Args ──
            );
            return data.Adapt<IEnumerable<AwardDto>>();
        }
    }
}
