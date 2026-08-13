using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeTrainingRepository
    {
        Task<EmployeeTraining?> GetEmployeeTrainingAsync(Guid employeeTrainingGuid, bool trackChanges);
        Task<EmployeeTraining?> GetByEmployeeGuidAndEmployeeTrainingGuidAsync(Guid employeeGuid, Guid employeeTrainingGuid);
        Task<IEnumerable<EmployeeTraining>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeTrainingAsync(EmployeeTraining employeeTraining, IDbTransaction? transaction = null);
        Task UpdateEmployeeTrainingAsync(EmployeeTraining employeeTraining, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeTrainingAsync(EmployeeTraining employeeTraining, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeTrainingAsync(Guid employeeTrainingGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeTraining>> SearchEmployeeTrainingAsync(
            string? startDate,
            string? startDateSearchType,
            string? endDate,
            string? endDateSearchType,
            string? durationHour,
            string? durationHourSearchType,
            string? score,
            string? scoreSearchType,
            string? grade,
            string? gradeSearchType,
            string? isPassed,
            string? isPassedSearchType,
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeTrainingGuid,
            IDbTransaction? transaction = null);
    }
}
