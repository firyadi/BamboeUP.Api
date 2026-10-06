using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IAttendanceRepository
    {
        Task<Attendance?> GetAttendanceAsync(Guid attendanceGuid, bool trackChanges);
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync(bool trackChanges);

        Task CreateAttendanceAsync(Attendance attendance, IDbTransaction? transaction = null);
        Task UpdateAttendanceAsync(Attendance attendance, IDbTransaction? transaction = null);
        Task DeleteAttendanceAsync(Guid attendanceGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAttendanceAsync(Attendance attendance, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Attendance>> SearchAttendanceAsync(
            string? attendanceDate,
            string? attendanceDateSearchType,
            string? checkinTime,
            string? checkinTimeSearchType,
            string? checkOutTime,
            string? checkOutTimeSearchType,
            string? checkinChannel,
            string? checkinChannelSearchType,
            string? checkOutChannel,
            string? checkOutChannelSearchType,
            string? checkinLatitude,
            string? checkinLatitudeSearchType,
            string? checkinLongitude,
            string? checkinLongitudeSearchType,
            string? checkinAccuracyMeters,
            string? checkinAccuracyMetersSearchType,
            string? checkOutLatitude,
            string? checkOutLatitudeSearchType,
            string? checkOutLongitude,
            string? checkOutLongitudeSearchType,
            string? checkOutAccuracyMeters,
            string? checkOutAccuracyMetersSearchType,
            string? note,
            string? noteSearchType,
string? companyName, string? companyNameSearchType,
string? companyOfficeName, string? companyOfficeNameSearchType,
string? personName, string? personNameSearchType,

            IDbTransaction? transaction = null);
    }
}
