using Contracts;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell;

public class UserScopeService : IUserScopeService
{
    private const string CacheKeyPrefix = "UserScope";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IRepositoryManager _repository;
    private readonly IUserContext _userContext;
    private readonly IMemoryCache _cache;
    private readonly bool _useScopedMasterData;

    public UserScopeService(
        IRepositoryManager repository,
        IUserContext userContext,
        IMemoryCache memoryCache,
        IConfiguration configuration)
    {
        _repository = repository;
        _userContext = userContext;
        _cache = memoryCache;
        _useScopedMasterData = configuration.GetValue("UseScopedMasterData", true);
    }

    private bool ShouldBypassScope() =>
        !_useScopedMasterData || _userContext.IsAdmin || !_userContext.IsAuthenticated;

    public async Task<IEnumerable<CompanyDto>> GetAccessibleCompaniesAsync()
    {
        if (ShouldBypassScope())
        {
            var all = await _repository.Company.GetAllCompaniesAsync(false);
            return all.Adapt<IEnumerable<CompanyDto>>();
        }

        var cacheKey = $"{CacheKeyPrefix}:Companies:{_userContext.UserId}";
        if (_cache.TryGetValue(cacheKey, out List<CompanyDto>? cached) && cached != null)
            return cached;

        var entities = await _repository.UserScope.GetAccessibleCompaniesAsync(_userContext.UserId);
        var result = entities.Adapt<List<CompanyDto>>();
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<IEnumerable<CompanyOfficeDto>> GetAccessibleOfficesAsync(long companyId)
    {
        if (ShouldBypassScope())
        {
            var company = await _repository.Company.GetCompanyByIdAsync(companyId, false);
            if (company == null)
                return Enumerable.Empty<CompanyOfficeDto>();

            var offices = await _repository.CompanyOffice.GetAllByCompanyGuidAsync(company.CompanyGuid);
            return offices.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        var cacheKey = $"{CacheKeyPrefix}:Offices:{_userContext.UserId}:{companyId}";
        if (_cache.TryGetValue(cacheKey, out List<CompanyOfficeDto>? cached) && cached != null)
            return cached;

        var scoped = await _repository.UserScope.GetAccessibleOfficesAsync(_userContext.UserId, companyId);
        var result = scoped.Adapt<List<CompanyOfficeDto>>();
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<IEnumerable<CompanyOfficeDto>> GetAccessibleOfficesByCompanyGuidAsync(Guid companyGuid)
    {
        var company = await _repository.Company.GetCompanyAsync(companyGuid, false);
        await EnsureCanAccessCompanyAsync(company.CompanyId);
        return await GetAccessibleOfficesAsync(company.CompanyId);
    }

    public async Task<IEnumerable<CompanyOfficeDto>> GetAllAccessibleOfficesAsync()
    {
        if (ShouldBypassScope())
        {
            var all = await _repository.CompanyOffice.GetAllCompanyOfficesAsync(false);
            return all.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        var cacheKey = $"{CacheKeyPrefix}:AllOffices:{_userContext.UserId}";
        if (_cache.TryGetValue(cacheKey, out List<CompanyOfficeDto>? cached) && cached != null)
            return cached;

        var scoped = await _repository.UserScope.GetAllAccessibleOfficesAsync(_userContext.UserId);
        var result = scoped.Adapt<List<CompanyOfficeDto>>();
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<bool> CanAccessCompanyAsync(long companyId)
    {
        if (ShouldBypassScope())
            return true;

        return await _repository.UserScope.CanAccessCompanyAsync(_userContext.UserId, companyId);
    }

    public async Task<bool> CanAccessOfficeAsync(long companyId, long companyOfficeId)
    {
        if (ShouldBypassScope())
            return true;

        return await _repository.UserScope.CanAccessOfficeAsync(_userContext.UserId, companyId, companyOfficeId);
    }

    public async Task EnsureCanAccessCompanyAsync(long companyId)
    {
        if (!await CanAccessCompanyAsync(companyId))
            throw new UnauthorizedAccessException("You do not have access to this company.");
    }

    public async Task EnsureCanAccessOfficeAsync(long companyId, long companyOfficeId)
    {
        if (!await CanAccessOfficeAsync(companyId, companyOfficeId))
            throw new UnauthorizedAccessException("You do not have access to this office.");
    }

    public async Task<IEnumerable<OrganizationUnitDto>> GetAccessibleOrganizationUnitsAsync()
    {
        if (ShouldBypassScope())
        {
            var all = await _repository.OrganizationUnit.GetAllOrganizationUnitsAsync(false);
            return all.Adapt<IEnumerable<OrganizationUnitDto>>();
        }

        var cacheKey = $"{CacheKeyPrefix}:OrgUnits:{_userContext.UserId}";
        if (_cache.TryGetValue(cacheKey, out List<OrganizationUnitDto>? cached) && cached != null)
            return cached;

        var scoped = await _repository.UserScope.GetAccessibleOrganizationUnitsAsync(_userContext.UserId);
        var result = scoped.Adapt<List<OrganizationUnitDto>>();
        _cache.Set(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task EnsureCanAccessOrganizationUnitAsync(long organizationUnitId)
    {
        if (ShouldBypassScope())
            return;

        if (!await _repository.UserScope.CanAccessOrganizationUnitAsync(_userContext.UserId, organizationUnitId))
            throw new UnauthorizedAccessException("You do not have access to this organization unit.");
    }

    public async Task EnsureCanAccessOrganizationUnitGuidAsync(Guid organizationUnitGuid)
    {
        if (ShouldBypassScope())
            return;

        var entity = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, false);
        await EnsureCanAccessOrganizationUnitAsync(entity.OrganizationUnitId);
    }

    public void InvalidateCacheForUser(long userId)
    {
        _cache.Remove($"{CacheKeyPrefix}:Companies:{userId}");
        _cache.Remove($"{CacheKeyPrefix}:AllOffices:{userId}");
        _cache.Remove($"{CacheKeyPrefix}:OrgUnits:{userId}");
    }
}
