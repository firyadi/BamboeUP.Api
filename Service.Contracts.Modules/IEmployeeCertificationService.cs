using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeCertificationService
    {
        Task<IEnumerable<EmployeeCertificationDto>> GetAllEmployeeCertificationsAsync(bool trackChanges);
        Task<EmployeeCertificationDto?> GetEmployeeCertificationByGuidAsync(Guid employeeCertificationGuid, bool trackChanges);
        Task<EmployeeCertificationDto> CreateEmployeeCertificationAsync(Guid employeeGuid, EmployeeCertificationForCreationDto input);
        Task UpdateEmployeeCertificationAsync(Guid employeeGuid, Guid employeeCertificationGuid, EmployeeCertificationForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeCertificationAsync(Guid employeeGuid, Guid employeeCertificationGuid, EmployeeCertificationForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeCertificationByAdminAsync(Guid employeeCertificationGuid, bool trackChanges);

        Task<IEnumerable<EmployeeCertificationDto>> SearchEmployeeCertificationAsync(
            string? certificateNumber, string? certificateNumberSearchType, string? issuedDate, string? issuedDateSearchType, string? expiredDate, string? expiredDateSearchType, string? issuedby, string? issuedbySearchType, string? score, string? scoreSearchType, string? isLifetime, string? isLifetimeSearchType, string? isCurrent, string? isCurrentSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeCertificationGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeCertificationDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeCertificationDto?> GetByEmployeeGuidAndEmployeeCertificationGuidAsync(Guid employeeGuid, Guid employeeCertificationGuid);
    }
}
