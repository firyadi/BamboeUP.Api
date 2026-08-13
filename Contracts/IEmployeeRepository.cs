using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeAsync(Guid employeeGuid, bool trackChanges);
        Task<Employee?> GetEmployeeByIdAsync(long employeeId, bool trackChanges);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool trackChanges);

        Task CreateEmployeeAsync(Employee employee, IDbTransaction? transaction = null);
        Task UpdateEmployeeAsync(Employee employee, IDbTransaction? transaction = null);
        Task DeleteEmployeeAsync(Guid employeeGuid, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeAsync(Employee employee, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<Employee>> SearchEmployeeAsync(
            string? employeeNo,
            string? employeeNoSearchType,
            string? hireDate,
            string? hireDateSearchType,
            string? probationEndDate,
            string? probationEndDateSearchType,
            string? confirmationDate,
            string? confirmationDateSearchType,
            string? terminationDate,
            string? terminationDateSearchType,
            IDbTransaction? transaction = null);
    }
}
