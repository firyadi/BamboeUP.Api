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
    public partial class PositionLevelService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IPositionLevelService
    {
        public async Task<IEnumerable<PositionLevelDto>> GetAllPositionLevelsAsync(bool trackChanges)
        {
            var entities = await repository.PositionLevel.GetAllPositionLevelsAsync(trackChanges);
            return entities.Adapt<IEnumerable<PositionLevelDto>>();
        }

        public async Task<PositionLevelDto?> GetPositionLevelByGuidAsync(Guid positionLevelGuid, bool trackChanges)
        {
            var entity = await repository.PositionLevel.GetPositionLevelAsync(positionLevelGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<PositionLevelDto>();
            return dto;
        }

        public async Task<PositionLevelDto> CreatePositionLevelAsync(PositionLevelForCreationDto input)
        {
            var model = input.Adapt<PositionLevel>();

            if (model.PositionLevelGuid == Guid.Empty)
                model.PositionLevelGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.PositionLevel.CreatePositionLevelAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PositionLevel",
                RootEntityKey = model.PositionLevelGuid.ToString(),
                RootDisplayName = model.PositionLevelCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionLevel",
                        EntityKey = model.PositionLevelGuid.ToString(),
                        EntityDisplayName = model.PositionLevelCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<PositionLevelDto>();
        }

        public async Task UpdatePositionLevelAsync(Guid positionLevelGuid, PositionLevelForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.PositionLevel.GetPositionLevelAsync(positionLevelGuid, false)
                ?? throw new KeyNotFoundException($"PositionLevel '{{positionLevelGuid}}' was not found.");

            var model = input.Adapt<PositionLevel>();
            model.PositionLevelGuid = positionLevelGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.PositionLevel.UpdatePositionLevelAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PositionLevel",
                RootEntityKey = model.PositionLevelGuid.ToString(),
                RootDisplayName = model.PositionLevelCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionLevel",
                        EntityKey = model.PositionLevelGuid.ToString(),
                        EntityDisplayName = model.PositionLevelCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeletePositionLevelAsync(Guid positionLevelGuid, PositionLevelForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.PositionLevel.GetPositionLevelAsync(positionLevelGuid, false);
            var model = new PositionLevel
            {
                PositionLevelGuid = positionLevelGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.PositionLevel.SoftDeletePositionLevelAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PositionLevel",
                RootEntityKey = positionLevelGuid.ToString(),
                RootDisplayName = oldEntity?.PositionLevelCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionLevel",
                        EntityKey = positionLevelGuid.ToString(),
                        EntityDisplayName = oldEntity?.PositionLevelCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeletePositionLevelByAdminAsync(Guid positionLevelGuid, bool trackChanges)
        {
            await repository.PositionLevel.DeletePositionLevelAsync(positionLevelGuid);
        }

        public async Task<IEnumerable<PositionLevelDto>> SearchPositionLevelAsync(
            string? positionLevelCode, string? positionLevelCodeSearchType, string? positionLevelName, string? positionLevelNameSearchType, string? ranking, string? rankingSearchType, string? positionLevelDescription, string? positionLevelDescriptionSearchType

            )
        {
            var data = await repository.PositionLevel.SearchPositionLevelAsync(
                positionLevelCode, positionLevelCodeSearchType, positionLevelName, positionLevelNameSearchType, ranking, rankingSearchType, positionLevelDescription, positionLevelDescriptionSearchType

                );
            return data.Adapt<IEnumerable<PositionLevelDto>>();
        }
    }
}
