using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeLanguageRepository
    {
        Task<EmployeeLanguage?> GetEmployeeLanguageAsync(Guid employeeLanguageGuid, bool trackChanges);
        Task<EmployeeLanguage?> GetByEmployeeGuidAndEmployeeLanguageGuidAsync(Guid employeeGuid, Guid employeeLanguageGuid);
        Task<IEnumerable<EmployeeLanguage>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null);
        Task UpdateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeLanguageAsync(Guid employeeLanguageGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeLanguage>> SearchEmployeeLanguageAsync(
            string? certificationName,
            string? certificationNameSearchType,
            string? score,
            string? scoreSearchType,
            string? issuedDate,
            string? issuedDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeLanguageGuid,
            IDbTransaction? transaction = null);
    }
}
