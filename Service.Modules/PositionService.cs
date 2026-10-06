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
    public partial class PositionService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IPositionService
    {
        public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync(bool trackChanges)
        {
            var entities = await repository.Position.GetAllPositionsAsync(trackChanges);
            return entities.Adapt<IEnumerable<PositionDto>>();
        }

        public async Task<PositionDto?> GetPositionByGuidAsync(Guid positionGuid, bool trackChanges)
        {
            var entity = await repository.Position.GetPositionAsync(positionGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<PositionDto>();
            return dto;
        }

        public async Task<PositionDto> CreatePositionAsync(PositionForCreationDto input)
        {
            var model = input.Adapt<Position>();

            if (model.PositionGuid == Guid.Empty)
                model.PositionGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Position.CreatePositionAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Position",
                RootEntityKey = model.PositionGuid.ToString(),
                RootDisplayName = model.PositionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Position",
                        EntityKey = model.PositionGuid.ToString(),
                        EntityDisplayName = model.PositionCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<PositionDto>();
        }

        public async Task UpdatePositionAsync(Guid positionGuid, PositionForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Position.GetPositionAsync(positionGuid, false)
                ?? throw new KeyNotFoundException($"Position '{{positionGuid}}' was not found.");

            var model = input.Adapt<Position>();
            model.PositionGuid = positionGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Position.UpdatePositionAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Position",
                RootEntityKey = model.PositionGuid.ToString(),
                RootDisplayName = model.PositionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Position",
                        EntityKey = model.PositionGuid.ToString(),
                        EntityDisplayName = model.PositionCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeletePositionAsync(Guid positionGuid, PositionForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Position.GetPositionAsync(positionGuid, false);
            var model = new Position
            {
                PositionGuid = positionGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Position.SoftDeletePositionAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Position",
                RootEntityKey = positionGuid.ToString(),
                RootDisplayName = oldEntity?.PositionCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Position",
                        EntityKey = positionGuid.ToString(),
                        EntityDisplayName = oldEntity?.PositionCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeletePositionByAdminAsync(Guid positionGuid, bool trackChanges)
        {
            await repository.Position.DeletePositionAsync(positionGuid);
        }

        public async Task<IEnumerable<PositionDto>> SearchPositionAsync(
            string? positionCode, string? positionCodeSearchType, string? positionName, string? positionNameSearchType, string? summary, string? summarySearchType, string? validFrom, string? validFromSearchType, string? validto, string? validtoSearchType, string? positionDescription, string? positionDescriptionSearchType
, string? positionGradeName, string? positionGradeNameSearchType
, string? positionLevelName, string? positionLevelNameSearchType

            )
        {
            var data = await repository.Position.SearchPositionAsync(
                positionCode, positionCodeSearchType, positionName, positionNameSearchType, summary, summarySearchType, validFrom, validFromSearchType, validto, validtoSearchType, positionDescription, positionDescriptionSearchType
, positionGradeName, positionGradeNameSearchType
, positionLevelName, positionLevelNameSearchType

                );
            return data.Adapt<IEnumerable<PositionDto>>();
        }
    }
}
