using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IRoomBookingService
    {
        Task<IEnumerable<RoomBookingDto>> GetAllRoomBookingsAsync(bool trackChanges);
        Task<RoomBookingDto?> GetRoomBookingByGuidAsync(Guid roomBookingGuid, bool trackChanges);
        Task<RoomBookingDto> CreateRoomBookingAsync(Guid personGuid, RoomBookingForCreationDto input);
        Task UpdateRoomBookingAsync(Guid personGuid, Guid roomBookingGuid, RoomBookingForUpdateDto input, bool trackChanges);
        Task DeleteRoomBookingAsync(Guid personGuid, Guid roomBookingGuid, RoomBookingForDeleteDto input, bool trackChanges);
        Task DeleteRoomBookingByAdminAsync(Guid roomBookingGuid, bool trackChanges);

        Task<IEnumerable<RoomBookingDto>> SearchRoomBookingAsync(
            string? roomBookingCode, string? roomBookingCodeSearchType, string? subject, string? subjectSearchType, string? startDateTime, string? startDateTimeSearchType, string? endDateTime, string? endDateTimeSearchType, string? note, string? noteSearchType,
            Guid personGuid, Guid roomBookingGuid);

        // Detail (child) helpers
        Task<IEnumerable<RoomBookingDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<RoomBookingDto?> GetByPersonGuidAndRoomBookingGuidAsync(Guid personGuid, Guid roomBookingGuid);
    }
}
