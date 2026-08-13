using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeAssignmentService
    {
        Task<IEnumerable<EmployeeAssignmentDto>> GetAllEmployeeAssignmentsAsync(bool trackChanges);
        Task<EmployeeAssignmentDto?> GetEmployeeAssignmentByGuidAsync(Guid employeeAssignmentGuid, bool trackChanges);
        Task<EmployeeAssignmentDto> CreateEmployeeAssignmentAsync(Guid employeeGuid, EmployeeAssignmentForCreationDto input);
        Task UpdateEmployeeAssignmentAsync(Guid employeeGuid, Guid employeeAssignmentGuid, EmployeeAssignmentForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeAssignmentAsync(Guid employeeGuid, Guid employeeAssignmentGuid, EmployeeAssignmentForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeAssignmentByAdminAsync(Guid employeeAssignmentGuid, bool trackChanges);

        Task<IEnumerable<EmployeeAssignmentDto>> SearchEmployeeAssignmentAsync(
            string? effectiveFrom, string? effectiveFromSearchType, string? effectiveto, string? effectivetoSearchType, string? isPrimary, string? isPrimarySearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssignmentGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeAssignmentDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeAssignmentDto?> GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(Guid employeeGuid, Guid employeeAssignmentGuid);
    }
}
