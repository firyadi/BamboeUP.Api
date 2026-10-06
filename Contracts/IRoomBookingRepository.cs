using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IRoomBookingRepository
    {
        Task<RoomBooking?> GetRoomBookingAsync(Guid roomBookingGuid, bool trackChanges);
        Task<RoomBooking?> GetByPersonGuidAndRoomBookingGuidAsync(Guid personGuid, Guid roomBookingGuid);
        Task<IEnumerable<RoomBooking>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<IEnumerable<RoomBooking>> GetAllRoomBookingsAsync(bool trackChanges);
        Task<IEnumerable<RoomBooking>> GetRoomBookingsByDateRangeAsync(
            DateTime start,
            DateTime end,
            string? category = null,
            string? search = null,
            IDbTransaction? transaction = null);

        Task CreateRoomBookingAsync(RoomBooking roomBooking, IDbTransaction? transaction = null);
        Task UpdateRoomBookingAsync(RoomBooking roomBooking, IDbTransaction? transaction = null);
        Task SoftDeleteRoomBookingAsync(RoomBooking roomBooking, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteRoomBookingAsync(Guid roomBookingGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<RoomBooking>> SearchRoomBookingAsync(
            string? roomBookingCode,
            string? roomBookingCodeSearchType,
            string? subject,
            string? subjectSearchType,
            string? startDateTime,
            string? startDateTimeSearchType,
            string? endDateTime,
            string? endDateTimeSearchType,
            string? note,
            string? noteSearchType,
            Guid personGuid, Guid roomBookingGuid,
            IDbTransaction? transaction = null);
    }
}
