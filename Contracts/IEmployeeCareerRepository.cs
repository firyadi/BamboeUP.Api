using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeCareerRepository
    {
        Task<EmployeeCareer?> GetEmployeeCareerAsync(Guid employeeCareerGuid, bool trackChanges);
        Task<EmployeeCareer?> GetByEmployeeGuidAndEmployeeCareerGuidAsync(Guid employeeGuid, Guid employeeCareerGuid);
        Task<IEnumerable<EmployeeCareer>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null);
        Task UpdateEmployeeCareerAsync(EmployeeCareer employeeCareer, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeCareerAsync(EmployeeCareer employeeCareer, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeCareerAsync(Guid employeeCareerGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeCareer>> SearchEmployeeCareerAsync(
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? referenceNo,
            string? referenceNoSearchType,
            string? referenceDate,
            string? referenceDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeCareerGuid,
            IDbTransaction? transaction = null);
    }
}
