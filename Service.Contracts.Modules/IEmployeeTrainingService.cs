using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IEmployeeTrainingService
    {
        Task<IEnumerable<EmployeeTrainingDto>> GetAllEmployeeTrainingsAsync(bool trackChanges);
        Task<EmployeeTrainingDto?> GetEmployeeTrainingByGuidAsync(Guid employeeTrainingGuid, bool trackChanges);
        Task<EmployeeTrainingDto> CreateEmployeeTrainingAsync(Guid employeeGuid, EmployeeTrainingForCreationDto input);
        Task UpdateEmployeeTrainingAsync(Guid employeeGuid, Guid employeeTrainingGuid, EmployeeTrainingForUpdateDto input, bool trackChanges);
        Task DeleteEmployeeTrainingAsync(Guid employeeGuid, Guid employeeTrainingGuid, EmployeeTrainingForDeleteDto input, bool trackChanges);
        Task DeleteEmployeeTrainingByAdminAsync(Guid employeeTrainingGuid, bool trackChanges);

        Task<IEnumerable<EmployeeTrainingDto>> SearchEmployeeTrainingAsync(
            string? startDate, string? startDateSearchType, string? endDate, string? endDateSearchType, string? durationHour, string? durationHourSearchType, string? score, string? scoreSearchType, string? grade, string? gradeSearchType, string? isPassed, string? isPassedSearchType, string? certificateNumber, string? certificateNumberSearchType, string? remarks, string? remarksSearchType,
            Guid employeeGuid, Guid employeeTrainingGuid);

        // Detail (child) helpers
        Task<IEnumerable<EmployeeTrainingDto>> GetAllByEmployeeGuidAsync(Guid employeeGuid);
        Task<EmployeeTrainingDto?> GetByEmployeeGuidAndEmployeeTrainingGuidAsync(Guid employeeGuid, Guid employeeTrainingGuid);
    }
}
