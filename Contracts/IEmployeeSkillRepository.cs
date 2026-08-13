using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IEmployeeSkillRepository
    {
        Task<EmployeeSkill?> GetEmployeeSkillAsync(Guid employeeSkillGuid, bool trackChanges);
        Task<EmployeeSkill?> GetByEmployeeGuidAndEmployeeSkillGuidAsync(Guid employeeGuid, Guid employeeSkillGuid);
        Task<IEnumerable<EmployeeSkill>> GetAllByEmployeeGuidAsync(Guid employeeGuid);

        Task CreateEmployeeSkillAsync(EmployeeSkill employeeSkill, IDbTransaction? transaction = null);
        Task UpdateEmployeeSkillAsync(EmployeeSkill employeeSkill, IDbTransaction? transaction = null);
        Task SoftDeleteEmployeeSkillAsync(EmployeeSkill employeeSkill, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteEmployeeSkillAsync(Guid employeeSkillGuid, IDbTransaction? transaction = null);
        Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<EmployeeSkill>> SearchEmployeeSkillAsync(
            string? yearsofExperience,
            string? yearsofExperienceSearchType,
            string? lastUsedDate,
            string? lastUsedDateSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? isCertified,
            string? isCertifiedSearchType,
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? certificateIssueDate,
            string? certificateIssueDateSearchType,
            string? certificateExpiredDate,
            string? certificateExpiredDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeSkillGuid,
            IDbTransaction? transaction = null);
    }
}
