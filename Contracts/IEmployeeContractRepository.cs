using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeContractRepository
    {
        Task<EmployeeContract?> GetEmployeeContractAsync(Guid employeeContractGuid, bool trackChanges);
        Task<EmployeeContract?> GetByEmployeeGuidAndEmployeeContractGuidAsync(Guid employeeGuid, Guid employeeContractGuid);
        Task<IEnumerable<EmployeeContract>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null);
        Task UpdateEmployeeContractAsync(EmployeeContract employeeContract, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeContractAsync(EmployeeContract employeeContract, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeContractAsync(Guid employeeContractGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeContract>> SearchEmployeeContractAsync(
            string? contractNo,
            string? contractNoSearchType,
            string? contractStartDate,
            string? contractStartDateSearchType,
            string? contractEndDate,
            string? contractEndDateSearchType,
            string? effectiveDate,
            string? effectiveDateSearchType,
            string? renewalSequence,
            string? renewalSequenceSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeContractGuid,
            IDbTransaction? transaction = null);
    }
}
