using Entities.Models;

namespace Contracts;

public interface IUserScopeRepository
{
    Task<IEnumerable<Company>> GetAccessibleCompaniesAsync(long userId);
    Task<IEnumerable<CompanyOffice>> GetAccessibleOfficesAsync(long userId, long companyId);
    Task<IEnumerable<CompanyOffice>> GetAllAccessibleOfficesAsync(long userId);
    Task<bool> CanAccessCompanyAsync(long userId, long companyId);
    Task<bool> CanAccessOfficeAsync(long userId, long companyId, long companyOfficeId);
    Task<IEnumerable<OrganizationUnit>> GetAccessibleOrganizationUnitsAsync(long userId);
    Task<bool> CanAccessOrganizationUnitAsync(long userId, long organizationUnitId);
}
