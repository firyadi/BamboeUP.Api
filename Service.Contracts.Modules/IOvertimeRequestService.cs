using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IOvertimeRequestService
    {
        Task<IEnumerable<OvertimeRequestDto>> GetAllOvertimeRequestsAsync(bool trackChanges);
        Task<OvertimeRequestDto?> GetOvertimeRequestByGuidAsync(Guid overtimeRequestGuid, bool trackChanges);
        Task<OvertimeRequestDto> CreateOvertimeRequestAsync(OvertimeRequestForCreationDto input);
        Task UpdateOvertimeRequestAsync(Guid overtimeRequestGuid, OvertimeRequestForUpdateDto input, bool trackChanges);
        Task DeleteOvertimeRequestAsync(Guid overtimeRequestGuid, OvertimeRequestForDeleteDto input, bool trackChanges);
        Task DeleteOvertimeRequestByAdminAsync(Guid overtimeRequestGuid, bool trackChanges);

        Task<IEnumerable<OvertimeRequestDto>> SearchOvertimeRequestAsync(
            string? requestNo, string? requestNoSearchType, string? requestDate, string? requestDateSearchType, string? overtimeDate, string? overtimeDateSearchType, string? reason, string? reasonSearchType, string? workDescription, string? workDescriptionSearchType, string? plannedStartTime, string? plannedStartTimeSearchType, string? plannedEndTime, string? plannedEndTimeSearchType, string? totalPlannedMinutes, string? totalPlannedMinutesSearchType, string? actualStartTime, string? actualStartTimeSearchType, string? actualEndTime, string? actualEndTimeSearchType, string? totalActualMinutes, string? totalActualMinutesSearchType, string? approvedTime, string? approvedTimeSearchType, string? rejectionReason, string? rejectionReasonSearchType, string? note, string? noteSearchType
, string? approvedbyName, string? approvedbyNameSearchType
, string? companyName, string? companyNameSearchType
, string? companyOfficeName, string? companyOfficeNameSearchType
, string? personName, string? personNameSearchType

        );
    }
}
