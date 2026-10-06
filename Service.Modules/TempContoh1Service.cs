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
    public partial class TempContoh1Service(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : ITempContoh1Service
    {
        public async Task<IEnumerable<TempContoh1Dto>> GetAllTempContoh1sAsync(bool trackChanges)
        {
            var entities = await repository.TempContoh1.GetAllTempContoh1sAsync(trackChanges);
            return entities.Adapt<IEnumerable<TempContoh1Dto>>();
        }

        public async Task<TempContoh1Dto?> GetTempContoh1ByGuidAsync(Guid tempContoh1Guid, bool trackChanges)
        {
            var entity = await repository.TempContoh1.GetTempContoh1Async(tempContoh1Guid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<TempContoh1Dto>();
            return dto;
        }

        public async Task<TempContoh1Dto> CreateTempContoh1Async(TempContoh1ForCreationDto input)
        {
            var model = input.Adapt<TempContoh1>();

            if (model.TempContoh1Guid == Guid.Empty)
                model.TempContoh1Guid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.TempContoh1.CreateTempContoh1Async(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "TempContoh1",
                RootEntityKey = model.TempContoh1Guid.ToString(),
                RootDisplayName = model.PersonId.ToString(),
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "TempContoh1",
                        EntityKey = model.TempContoh1Guid.ToString(),
                        EntityDisplayName = model.PersonId.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<TempContoh1Dto>();
        }

        public async Task UpdateTempContoh1Async(Guid tempContoh1Guid, TempContoh1ForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.TempContoh1.GetTempContoh1Async(tempContoh1Guid, false)
                ?? throw new KeyNotFoundException($"TempContoh1 '{{tempContoh1Guid}}' was not found.");

            var model = input.Adapt<TempContoh1>();
            model.TempContoh1Guid = tempContoh1Guid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.TempContoh1.UpdateTempContoh1Async(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "TempContoh1",
                RootEntityKey = model.TempContoh1Guid.ToString(),
                RootDisplayName = model.PersonId.ToString(),
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "TempContoh1",
                        EntityKey = model.TempContoh1Guid.ToString(),
                        EntityDisplayName = model.PersonId.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteTempContoh1Async(Guid tempContoh1Guid, TempContoh1ForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.TempContoh1.GetTempContoh1Async(tempContoh1Guid, false);
            var model = new TempContoh1
            {
                TempContoh1Guid = tempContoh1Guid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.TempContoh1.SoftDeleteTempContoh1Async(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "TempContoh1",
                RootEntityKey = tempContoh1Guid.ToString(),
                RootDisplayName = oldEntity?.PersonId.ToString(),
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "TempContoh1",
                        EntityKey = tempContoh1Guid.ToString(),
                        EntityDisplayName = oldEntity?.PersonId.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteTempContoh1ByAdminAsync(Guid tempContoh1Guid, bool trackChanges)
        {
            await repository.TempContoh1.DeleteTempContoh1Async(tempContoh1Guid);
        }

        public async Task<IEnumerable<TempContoh1Dto>> SearchTempContoh1Async(
            string? personName, string? personNameSearchType,
            string? awardTypeName, string? awardTypeNameSearchType
            )
        {
            var data = await repository.TempContoh1.SearchTempContoh1Async(
                personName, personNameSearchType,
                awardTypeName, awardTypeNameSearchType
                );
            return data.Adapt<IEnumerable<TempContoh1Dto>>();
        }
    }
}
