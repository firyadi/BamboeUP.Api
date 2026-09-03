using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IMeetingRoomRepository
    {
        Task<MeetingRoom?> GetMeetingRoomAsync(Guid meetingRoomGuid, bool trackChanges);
        Task<IEnumerable<MeetingRoom>> GetAllMeetingRoomsAsync(bool trackChanges);

        Task CreateMeetingRoomAsync(MeetingRoom meetingRoom, IDbTransaction? transaction = null);
        Task UpdateMeetingRoomAsync(MeetingRoom meetingRoom, IDbTransaction? transaction = null);
        Task DeleteMeetingRoomAsync(Guid meetingRoomGuid, IDbTransaction? transaction = null);
        Task SoftDeleteMeetingRoomAsync(MeetingRoom meetingRoom, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<MeetingRoom>> SearchMeetingRoomAsync(
            string? roomCode,
            string? roomCodeSearchType,
            string? roomName,
            string? roomNameSearchType,
            string? capacity,
            string? capacitySearchType,
            string? location,
            string? locationSearchType,
            string? note,
            string? noteSearchType,

            IDbTransaction? transaction = null);
    }
}
