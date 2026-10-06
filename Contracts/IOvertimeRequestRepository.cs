using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IOvertimeRequestRepository
    {
        Task<OvertimeRequest?> GetOvertimeRequestAsync(Guid overtimeRequestGuid, bool trackChanges);
        Task<IEnumerable<OvertimeRequest>> GetAllOvertimeRequestsAsync(bool trackChanges);

        Task CreateOvertimeRequestAsync(OvertimeRequest overtimeRequest, IDbTransaction? transaction = null);
        Task UpdateOvertimeRequestAsync(OvertimeRequest overtimeRequest, IDbTransaction? transaction = null);
        Task DeleteOvertimeRequestAsync(Guid overtimeRequestGuid, IDbTransaction? transaction = null);
        Task SoftDeleteOvertimeRequestAsync(OvertimeRequest overtimeRequest, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<OvertimeRequest>> SearchOvertimeRequestAsync(
            string? requestNo,
            string? requestNoSearchType,
            string? requestDate,
            string? requestDateSearchType,
            string? overtimeDate,
            string? overtimeDateSearchType,
            string? reason,
            string? reasonSearchType,
            string? workDescription,
            string? workDescriptionSearchType,
            string? plannedStartTime,
            string? plannedStartTimeSearchType,
            string? plannedEndTime,
            string? plannedEndTimeSearchType,
            string? totalPlannedMinutes,
            string? totalPlannedMinutesSearchType,
            string? actualStartTime,
            string? actualStartTimeSearchType,
            string? actualEndTime,
            string? actualEndTimeSearchType,
            string? totalActualMinutes,
            string? totalActualMinutesSearchType,
            string? approvedTime,
            string? approvedTimeSearchType,
            string? rejectionReason,
            string? rejectionReasonSearchType,
            string? note,
            string? noteSearchType,
string? approvedbyName, string? approvedbyNameSearchType,
string? companyName, string? companyNameSearchType,
string? companyOfficeName, string? companyOfficeNameSearchType,
string? personName, string? personNameSearchType,

            IDbTransaction? transaction = null);
    }
}
