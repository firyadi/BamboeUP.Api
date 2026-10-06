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
    public partial class PositionGradeService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IPositionGradeService
    {
        public async Task<IEnumerable<PositionGradeDto>> GetAllPositionGradesAsync(bool trackChanges)
        {
            var entities = await repository.PositionGrade.GetAllPositionGradesAsync(trackChanges);
            return entities.Adapt<IEnumerable<PositionGradeDto>>();
        }

        public async Task<PositionGradeDto?> GetPositionGradeByGuidAsync(Guid positionGradeGuid, bool trackChanges)
        {
            var entity = await repository.PositionGrade.GetPositionGradeAsync(positionGradeGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<PositionGradeDto>();
            return dto;
        }

        public async Task<PositionGradeDto> CreatePositionGradeAsync(PositionGradeForCreationDto input)
        {
            var model = input.Adapt<PositionGrade>();

            if (model.PositionGradeGuid == Guid.Empty)
                model.PositionGradeGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.PositionGrade.CreatePositionGradeAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PositionGrade",
                RootEntityKey = model.PositionGradeGuid.ToString(),
                RootDisplayName = model.PositionGradeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionGrade",
                        EntityKey = model.PositionGradeGuid.ToString(),
                        EntityDisplayName = model.PositionGradeCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<PositionGradeDto>();
        }

        public async Task UpdatePositionGradeAsync(Guid positionGradeGuid, PositionGradeForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.PositionGrade.GetPositionGradeAsync(positionGradeGuid, false)
                ?? throw new KeyNotFoundException($"PositionGrade '{{positionGradeGuid}}' was not found.");

            var model = input.Adapt<PositionGrade>();
            model.PositionGradeGuid = positionGradeGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.PositionGrade.UpdatePositionGradeAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PositionGrade",
                RootEntityKey = model.PositionGradeGuid.ToString(),
                RootDisplayName = model.PositionGradeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionGrade",
                        EntityKey = model.PositionGradeGuid.ToString(),
                        EntityDisplayName = model.PositionGradeCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeletePositionGradeAsync(Guid positionGradeGuid, PositionGradeForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.PositionGrade.GetPositionGradeAsync(positionGradeGuid, false);
            var model = new PositionGrade
            {
                PositionGradeGuid = positionGradeGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.PositionGrade.SoftDeletePositionGradeAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PositionGrade",
                RootEntityKey = positionGradeGuid.ToString(),
                RootDisplayName = oldEntity?.PositionGradeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "PositionGrade",
                        EntityKey = positionGradeGuid.ToString(),
                        EntityDisplayName = oldEntity?.PositionGradeCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeletePositionGradeByAdminAsync(Guid positionGradeGuid, bool trackChanges)
        {
            await repository.PositionGrade.DeletePositionGradeAsync(positionGradeGuid);
        }

        public async Task<IEnumerable<PositionGradeDto>> SearchPositionGradeAsync(
            string? positionGradeCode, string? positionGradeCodeSearchType, string? positionGradeName, string? positionGradeNameSearchType, string? interval, string? intervalSearchType, string? ranking, string? rankingSearchType, string? positionGradeDescription, string? positionGradeDescriptionSearchType

            )
        {
            var data = await repository.PositionGrade.SearchPositionGradeAsync(
                positionGradeCode, positionGradeCodeSearchType, positionGradeName, positionGradeNameSearchType, interval, intervalSearchType, ranking, rankingSearchType, positionGradeDescription, positionGradeDescriptionSearchType

                );
            return data.Adapt<IEnumerable<PositionGradeDto>>();
        }
    }
}
