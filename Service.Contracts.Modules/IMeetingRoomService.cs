using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IMeetingRoomService
    {
        Task<IEnumerable<MeetingRoomDto>> GetAllMeetingRoomsAsync(bool trackChanges);
        Task<MeetingRoomDto?> GetMeetingRoomByGuidAsync(Guid meetingRoomGuid, bool trackChanges);
        Task<MeetingRoomDto> CreateMeetingRoomAsync(MeetingRoomForCreationDto input);
        Task UpdateMeetingRoomAsync(Guid meetingRoomGuid, MeetingRoomForUpdateDto input, bool trackChanges);
        Task DeleteMeetingRoomAsync(Guid meetingRoomGuid, MeetingRoomForDeleteDto input, bool trackChanges);
        Task DeleteMeetingRoomByAdminAsync(Guid meetingRoomGuid, bool trackChanges);

        Task<IEnumerable<MeetingRoomDto>> SearchMeetingRoomAsync(
            string? roomCode, string? roomCodeSearchType, string? roomName, string? roomNameSearchType, string? capacity, string? capacitySearchType, string? location, string? locationSearchType, string? note, string? noteSearchType

        );
    }
}
