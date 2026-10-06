using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class AttendanceService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext,
        IFileStorageService fileStorageService) : IAttendanceService
    {
        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(bool trackChanges)
        {
            var entities = await repository.Attendance.GetAllAttendancesAsync(trackChanges);
            return entities.Adapt<IEnumerable<AttendanceDto>>();
        }

        public async Task<AttendanceDto?> GetAttendanceByGuidAsync(Guid attendanceGuid, bool trackChanges)
        {
            var entity = await repository.Attendance.GetAttendanceAsync(attendanceGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<AttendanceDto>();
            if (entity.CheckinFileStorageId is > 0)
            {
                var file = await fileStorageService.GetFileStorageByIdAsync(entity.CheckinFileStorageId.Value, false);
                dto.CheckinFileStorageGuid = file?.FileStorageGuid;
            }
            return dto;
        }

        public async Task<AttendanceDto> CreateAttendanceAsync(AttendanceForCreationDto input)
        {
            var model = input.Adapt<Attendance>();

            if (model.AttendanceGuid == Guid.Empty)
                model.AttendanceGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Attendance.CreateAttendanceAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Attendance",
                RootEntityKey = model.AttendanceGuid.ToString(),
                RootDisplayName = model.CheckinChannel,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Attendance",
                        EntityKey = model.AttendanceGuid.ToString(),
                        EntityDisplayName = model.CheckinChannel,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<AttendanceDto>();
        }

        public async Task UpdateAttendanceAsync(Guid attendanceGuid, AttendanceForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Attendance.GetAttendanceAsync(attendanceGuid, false)
                ?? throw new KeyNotFoundException($"Attendance '{{attendanceGuid}}' was not found.");

            var model = input.Adapt<Attendance>();
            model.AttendanceGuid = attendanceGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Attendance.UpdateAttendanceAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Attendance",
                RootEntityKey = model.AttendanceGuid.ToString(),
                RootDisplayName = model.CheckinChannel,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Attendance",
                        EntityKey = model.AttendanceGuid.ToString(),
                        EntityDisplayName = model.CheckinChannel,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteAttendanceAsync(Guid attendanceGuid, AttendanceForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Attendance.GetAttendanceAsync(attendanceGuid, false);
            var model = new Attendance
            {
                AttendanceGuid = attendanceGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Attendance.SoftDeleteAttendanceAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Attendance",
                RootEntityKey = attendanceGuid.ToString(),
                RootDisplayName = oldEntity?.CheckinChannel,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Attendance",
                        EntityKey = attendanceGuid.ToString(),
                        EntityDisplayName = oldEntity?.CheckinChannel,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteAttendanceByAdminAsync(Guid attendanceGuid, bool trackChanges)
        {
            await repository.Attendance.DeleteAttendanceAsync(attendanceGuid);
        }

        public async Task<IEnumerable<AttendanceDto>> SearchAttendanceAsync(
            string? attendanceDate, string? attendanceDateSearchType, string? checkinTime, string? checkinTimeSearchType, string? checkOutTime, string? checkOutTimeSearchType, string? checkinChannel, string? checkinChannelSearchType, string? checkOutChannel, string? checkOutChannelSearchType, string? checkinLatitude, string? checkinLatitudeSearchType, string? checkinLongitude, string? checkinLongitudeSearchType, string? checkinAccuracyMeters, string? checkinAccuracyMetersSearchType, string? checkOutLatitude, string? checkOutLatitudeSearchType, string? checkOutLongitude, string? checkOutLongitudeSearchType, string? checkOutAccuracyMeters, string? checkOutAccuracyMetersSearchType, string? note, string? noteSearchType
, string? companyName, string? companyNameSearchType
, string? companyOfficeName, string? companyOfficeNameSearchType
, string? personName, string? personNameSearchType

            )
        {
            var data = await repository.Attendance.SearchAttendanceAsync(
                attendanceDate, attendanceDateSearchType, checkinTime, checkinTimeSearchType, checkOutTime, checkOutTimeSearchType, checkinChannel, checkinChannelSearchType, checkOutChannel, checkOutChannelSearchType, checkinLatitude, checkinLatitudeSearchType, checkinLongitude, checkinLongitudeSearchType, checkinAccuracyMeters, checkinAccuracyMetersSearchType, checkOutLatitude, checkOutLatitudeSearchType, checkOutLongitude, checkOutLongitudeSearchType, checkOutAccuracyMeters, checkOutAccuracyMetersSearchType, note, noteSearchType
, companyName, companyNameSearchType
, companyOfficeName, companyOfficeNameSearchType
, personName, personNameSearchType

                );
            return data.Adapt<IEnumerable<AttendanceDto>>();
        }
    }
}
