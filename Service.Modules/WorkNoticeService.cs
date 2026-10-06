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
    public partial class WorkNoticeService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IWorkNoticeService
    {
        public async Task<IEnumerable<WorkNoticeDto>> GetAllWorkNoticesAsync(bool trackChanges)
        {
            var entities = await repository.WorkNotice.GetAllWorkNoticesAsync(trackChanges);
            return entities.Adapt<IEnumerable<WorkNoticeDto>>();
        }

        public async Task<WorkNoticeDto?> GetWorkNoticeByGuidAsync(Guid workNoticeGuid, bool trackChanges)
        {
            var entity = await repository.WorkNotice.GetWorkNoticeAsync(workNoticeGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<WorkNoticeDto>();
            return dto;
        }

        public async Task<WorkNoticeDto> CreateWorkNoticeAsync(WorkNoticeForCreationDto input)
        {
            var model = input.Adapt<WorkNotice>();

            if (model.WorkNoticeGuid == Guid.Empty)
                model.WorkNoticeGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.WorkNotice.CreateWorkNoticeAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "WorkNotice",
                RootEntityKey = model.WorkNoticeGuid.ToString(),
                RootDisplayName = model.WorkNoticeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "WorkNotice",
                        EntityKey = model.WorkNoticeGuid.ToString(),
                        EntityDisplayName = model.WorkNoticeCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<WorkNoticeDto>();
        }

        public async Task UpdateWorkNoticeAsync(Guid workNoticeGuid, WorkNoticeForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.WorkNotice.GetWorkNoticeAsync(workNoticeGuid, false)
                ?? throw new KeyNotFoundException($"WorkNotice '{{workNoticeGuid}}' was not found.");

            var model = input.Adapt<WorkNotice>();
            model.WorkNoticeGuid = workNoticeGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.WorkNotice.UpdateWorkNoticeAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "WorkNotice",
                RootEntityKey = model.WorkNoticeGuid.ToString(),
                RootDisplayName = model.WorkNoticeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "WorkNotice",
                        EntityKey = model.WorkNoticeGuid.ToString(),
                        EntityDisplayName = model.WorkNoticeCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteWorkNoticeAsync(Guid workNoticeGuid, WorkNoticeForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.WorkNotice.GetWorkNoticeAsync(workNoticeGuid, false);
            var model = new WorkNotice
            {
                WorkNoticeGuid = workNoticeGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.WorkNotice.SoftDeleteWorkNoticeAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "WorkNotice",
                RootEntityKey = workNoticeGuid.ToString(),
                RootDisplayName = oldEntity?.WorkNoticeCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "WorkNotice",
                        EntityKey = workNoticeGuid.ToString(),
                        EntityDisplayName = oldEntity?.WorkNoticeCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteWorkNoticeByAdminAsync(Guid workNoticeGuid, bool trackChanges)
        {
            await repository.WorkNotice.DeleteWorkNoticeAsync(workNoticeGuid);
        }

        public async Task<IEnumerable<WorkNoticeDto>> SearchWorkNoticeAsync(
            string? workNoticeCode, string? workNoticeCodeSearchType, string? workNoticeName, string? workNoticeNameSearchType, string? workNoticeDate, string? workNoticeDateSearchType, string? isEditAble, string? isEditAbleSearchType, string? note, string? noteSearchType

            )
        {
            var data = await repository.WorkNotice.SearchWorkNoticeAsync(
                workNoticeCode, workNoticeCodeSearchType, workNoticeName, workNoticeNameSearchType, workNoticeDate, workNoticeDateSearchType, isEditAble, isEditAbleSearchType, note, noteSearchType

                );
            return data.Adapt<IEnumerable<WorkNoticeDto>>();
        }
    }
}
