using Shared.DataTransferObjects;

namespace Service.Contracts.Shell;

public interface IUserScopeService
{
    Task<IEnumerable<CompanyDto>> GetAccessibleCompaniesAsync();
    Task<IEnumerable<CompanyOfficeDto>> GetAccessibleOfficesAsync(long companyId);
    Task<IEnumerable<CompanyOfficeDto>> GetAccessibleOfficesByCompanyGuidAsync(Guid companyGuid);
    Task<IEnumerable<CompanyOfficeDto>> GetAllAccessibleOfficesAsync();
    Task<bool> CanAccessCompanyAsync(long companyId);
    Task<bool> CanAccessOfficeAsync(long companyId, long companyOfficeId);
    Task EnsureCanAccessCompanyAsync(long companyId);
    Task EnsureCanAccessOfficeAsync(long companyId, long companyOfficeId);
    Task<IEnumerable<OrganizationUnitDto>> GetAccessibleOrganizationUnitsAsync();
    Task EnsureCanAccessOrganizationUnitAsync(long organizationUnitId);
    Task EnsureCanAccessOrganizationUnitGuidAsync(Guid organizationUnitGuid);
}
