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
    public partial class OvertimeRequestService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IOvertimeRequestService
    {
        public async Task<IEnumerable<OvertimeRequestDto>> GetAllOvertimeRequestsAsync(bool trackChanges)
        {
            var entities = await repository.OvertimeRequest.GetAllOvertimeRequestsAsync(trackChanges);
            return entities.Adapt<IEnumerable<OvertimeRequestDto>>();
        }

        public async Task<OvertimeRequestDto?> GetOvertimeRequestByGuidAsync(Guid overtimeRequestGuid, bool trackChanges)
        {
            var entity = await repository.OvertimeRequest.GetOvertimeRequestAsync(overtimeRequestGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<OvertimeRequestDto>();
            return dto;
        }

        public async Task<OvertimeRequestDto> CreateOvertimeRequestAsync(OvertimeRequestForCreationDto input)
        {
            var model = input.Adapt<OvertimeRequest>();

            if (model.OvertimeRequestGuid == Guid.Empty)
                model.OvertimeRequestGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.OvertimeRequest.CreateOvertimeRequestAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "OvertimeRequest",
                RootEntityKey = model.OvertimeRequestGuid.ToString(),
                RootDisplayName = model.RequestNo,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "OvertimeRequest",
                        EntityKey = model.OvertimeRequestGuid.ToString(),
                        EntityDisplayName = model.RequestNo,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<OvertimeRequestDto>();
        }

        public async Task UpdateOvertimeRequestAsync(Guid overtimeRequestGuid, OvertimeRequestForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.OvertimeRequest.GetOvertimeRequestAsync(overtimeRequestGuid, false)
                ?? throw new KeyNotFoundException($"OvertimeRequest '{{overtimeRequestGuid}}' was not found.");

            var model = input.Adapt<OvertimeRequest>();
            model.OvertimeRequestGuid = overtimeRequestGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.OvertimeRequest.UpdateOvertimeRequestAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "OvertimeRequest",
                RootEntityKey = model.OvertimeRequestGuid.ToString(),
                RootDisplayName = model.RequestNo,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById?.ToString() ?? "0",
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "OvertimeRequest",
                        EntityKey = model.OvertimeRequestGuid.ToString(),
                        EntityDisplayName = model.RequestNo,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteOvertimeRequestAsync(Guid overtimeRequestGuid, OvertimeRequestForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.OvertimeRequest.GetOvertimeRequestAsync(overtimeRequestGuid, false);
            var model = new OvertimeRequest
            {
                OvertimeRequestGuid = overtimeRequestGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.OvertimeRequest.SoftDeleteOvertimeRequestAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "OvertimeRequest",
                RootEntityKey = overtimeRequestGuid.ToString(),
                RootDisplayName = oldEntity?.RequestNo,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "OvertimeRequest",
                        EntityKey = overtimeRequestGuid.ToString(),
                        EntityDisplayName = oldEntity?.RequestNo,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteOvertimeRequestByAdminAsync(Guid overtimeRequestGuid, bool trackChanges)
        {
            await repository.OvertimeRequest.DeleteOvertimeRequestAsync(overtimeRequestGuid);
        }

        public async Task<IEnumerable<OvertimeRequestDto>> SearchOvertimeRequestAsync(
            string? requestNo, string? requestNoSearchType, string? requestDate, string? requestDateSearchType, string? overtimeDate, string? overtimeDateSearchType, string? reason, string? reasonSearchType, string? workDescription, string? workDescriptionSearchType, string? plannedStartTime, string? plannedStartTimeSearchType, string? plannedEndTime, string? plannedEndTimeSearchType, string? totalPlannedMinutes, string? totalPlannedMinutesSearchType, string? actualStartTime, string? actualStartTimeSearchType, string? actualEndTime, string? actualEndTimeSearchType, string? totalActualMinutes, string? totalActualMinutesSearchType, string? approvedTime, string? approvedTimeSearchType, string? rejectionReason, string? rejectionReasonSearchType, string? note, string? noteSearchType
, string? approvedbyName, string? approvedbyNameSearchType
, string? companyName, string? companyNameSearchType
, string? companyOfficeName, string? companyOfficeNameSearchType
, string? personName, string? personNameSearchType

            )
        {
            var data = await repository.OvertimeRequest.SearchOvertimeRequestAsync(
                requestNo, requestNoSearchType, requestDate, requestDateSearchType, overtimeDate, overtimeDateSearchType, reason, reasonSearchType, workDescription, workDescriptionSearchType, plannedStartTime, plannedStartTimeSearchType, plannedEndTime, plannedEndTimeSearchType, totalPlannedMinutes, totalPlannedMinutesSearchType, actualStartTime, actualStartTimeSearchType, actualEndTime, actualEndTimeSearchType, totalActualMinutes, totalActualMinutesSearchType, approvedTime, approvedTimeSearchType, rejectionReason, rejectionReasonSearchType, note, noteSearchType
, approvedbyName, approvedbyNameSearchType
, companyName, companyNameSearchType
, companyOfficeName, companyOfficeNameSearchType
, personName, personNameSearchType

                );
            return data.Adapt<IEnumerable<OvertimeRequestDto>>();
        }
    }
}
