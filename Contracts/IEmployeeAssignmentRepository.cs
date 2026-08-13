using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeAssignmentRepository
    {
        Task<EmployeeAssignment?> GetEmployeeAssignmentAsync(Guid employeeAssignmentGuid, bool trackChanges);
        Task<EmployeeAssignment?> GetByEmployeeGuidAndEmployeeAssignmentGuidAsync(Guid employeeGuid, Guid employeeAssignmentGuid);
        Task<IEnumerable<EmployeeAssignment>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null);
        Task UpdateEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeAssignmentAsync(EmployeeAssignment employeeAssignment, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeAssignmentAsync(Guid employeeAssignmentGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeAssignment>> SearchEmployeeAssignmentAsync(
            string? effectiveFrom,
            string? effectiveFromSearchType,
            string? effectiveto,
            string? effectivetoSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeAssignmentGuid,
            IDbTransaction? transaction = null);
    }
}
