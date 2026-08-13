using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeCertificationRepository
    {
        Task<EmployeeCertification?> GetEmployeeCertificationAsync(Guid employeeCertificationGuid, bool trackChanges);
        Task<EmployeeCertification?> GetByEmployeeGuidAndEmployeeCertificationGuidAsync(Guid employeeGuid, Guid employeeCertificationGuid);
        Task<IEnumerable<EmployeeCertification>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeCertificationAsync(EmployeeCertification employeeCertification, IDbTransaction? transaction = null);
        Task UpdateEmployeeCertificationAsync(EmployeeCertification employeeCertification, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeCertificationAsync(EmployeeCertification employeeCertification, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeCertificationAsync(Guid employeeCertificationGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeCertification>> SearchEmployeeCertificationAsync(
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? issuedDate,
            string? issuedDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            string? issuedby,
            string? issuedbySearchType,
            string? score,
            string? scoreSearchType,
            string? isLifetime,
            string? isLifetimeSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeCertificationGuid,
            IDbTransaction? transaction = null);
    }
}
