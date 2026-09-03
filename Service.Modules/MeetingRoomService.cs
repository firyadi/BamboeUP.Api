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
    public partial class MeetingRoomService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IMeetingRoomService
    {
        public async Task<IEnumerable<MeetingRoomDto>> GetAllMeetingRoomsAsync(bool trackChanges)
        {
            var entities = await repository.MeetingRoom.GetAllMeetingRoomsAsync(trackChanges);
            return entities.Adapt<IEnumerable<MeetingRoomDto>>();
        }

        public async Task<MeetingRoomDto?> GetMeetingRoomByGuidAsync(Guid meetingRoomGuid, bool trackChanges)
        {
            var entity = await repository.MeetingRoom.GetMeetingRoomAsync(meetingRoomGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<MeetingRoomDto>();
            return dto;
        }

        public async Task<MeetingRoomDto> CreateMeetingRoomAsync(MeetingRoomForCreationDto input)
        {
            var model = input.Adapt<MeetingRoom>();

            if (model.MeetingRoomGuid == Guid.Empty)
                model.MeetingRoomGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.MeetingRoom.CreateMeetingRoomAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "MeetingRoom",
                RootEntityKey = model.MeetingRoomGuid.ToString(),
                RootDisplayName = model.RoomCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "MeetingRoom",
                        EntityKey = model.MeetingRoomGuid.ToString(),
                        EntityDisplayName = model.RoomCode,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<MeetingRoomDto>();
        }

        public async Task UpdateMeetingRoomAsync(Guid meetingRoomGuid, MeetingRoomForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.MeetingRoom.GetMeetingRoomAsync(meetingRoomGuid, false)
                ?? throw new KeyNotFoundException($"MeetingRoom '{{meetingRoomGuid}}' was not found.");

            var model = input.Adapt<MeetingRoom>();
            model.MeetingRoomGuid = meetingRoomGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.MeetingRoom.UpdateMeetingRoomAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "MeetingRoom",
                RootEntityKey = model.MeetingRoomGuid.ToString(),
                RootDisplayName = model.RoomCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "MeetingRoom",
                        EntityKey = model.MeetingRoomGuid.ToString(),
                        EntityDisplayName = model.RoomCode,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteMeetingRoomAsync(Guid meetingRoomGuid, MeetingRoomForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.MeetingRoom.GetMeetingRoomAsync(meetingRoomGuid, false);
            var model = new MeetingRoom
            {
                MeetingRoomGuid = meetingRoomGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.MeetingRoom.SoftDeleteMeetingRoomAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "MeetingRoom",
                RootEntityKey = meetingRoomGuid.ToString(),
                RootDisplayName = oldEntity?.RoomCode,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "MeetingRoom",
                        EntityKey = meetingRoomGuid.ToString(),
                        EntityDisplayName = oldEntity?.RoomCode,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteMeetingRoomByAdminAsync(Guid meetingRoomGuid, bool trackChanges)
        {
            await repository.MeetingRoom.DeleteMeetingRoomAsync(meetingRoomGuid);
        }

        public async Task<IEnumerable<MeetingRoomDto>> SearchMeetingRoomAsync(
            string? roomCode, string? roomCodeSearchType, string? roomName, string? roomNameSearchType, string? capacity, string? capacitySearchType, string? location, string? locationSearchType, string? note, string? noteSearchType

            )
        {
            var data = await repository.MeetingRoom.SearchMeetingRoomAsync(
                roomCode, roomCodeSearchType, roomName, roomNameSearchType, capacity, capacitySearchType, location, locationSearchType, note, noteSearchType

                );
            return data.Adapt<IEnumerable<MeetingRoomDto>>();
        }
    }
}
