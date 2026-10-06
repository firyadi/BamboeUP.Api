using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(bool trackChanges);
        Task<AttendanceDto?> GetAttendanceByGuidAsync(Guid attendanceGuid, bool trackChanges);
        Task<AttendanceDto> CreateAttendanceAsync(AttendanceForCreationDto input);
        Task UpdateAttendanceAsync(Guid attendanceGuid, AttendanceForUpdateDto input, bool trackChanges);
        Task DeleteAttendanceAsync(Guid attendanceGuid, AttendanceForDeleteDto input, bool trackChanges);
        Task DeleteAttendanceByAdminAsync(Guid attendanceGuid, bool trackChanges);

        Task<IEnumerable<AttendanceDto>> SearchAttendanceAsync(
            string? attendanceDate, string? attendanceDateSearchType, string? checkinTime, string? checkinTimeSearchType, string? checkOutTime, string? checkOutTimeSearchType, string? checkinChannel, string? checkinChannelSearchType, string? checkOutChannel, string? checkOutChannelSearchType, string? checkinLatitude, string? checkinLatitudeSearchType, string? checkinLongitude, string? checkinLongitudeSearchType, string? checkinAccuracyMeters, string? checkinAccuracyMetersSearchType, string? checkOutLatitude, string? checkOutLatitudeSearchType, string? checkOutLongitude, string? checkOutLongitudeSearchType, string? checkOutAccuracyMeters, string? checkOutAccuracyMetersSearchType, string? note, string? noteSearchType
, string? companyName, string? companyNameSearchType
, string? companyOfficeName, string? companyOfficeNameSearchType
, string? personName, string? personNameSearchType

        );
    }
}
