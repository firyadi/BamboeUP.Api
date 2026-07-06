using Contracts;
using Service.Contracts.Shell;

using Microsoft.Extensions.Caching.Memory;

namespace Service.Shell
{
    public class AppParameterManager : IAppParameterManager
    {
        private readonly IParameterscopeService _parameterScopeService;
        private readonly IUserContext _userContext;
        private readonly IMemoryCache _cache;

        public AppParameterManager(IServiceShellManager serviceShellManager, IUserContext userContext, IMemoryCache cache)
        {
            _parameterScopeService = serviceShellManager.ParameterScopeService;
            _userContext = userContext;
            _cache = cache;
        }

        public async Task<T> GetValueAsync<T>(string parameterName, T defaultValue)
        {
            try
            {
                var companyId = _userContext.IsAuthenticated && _userContext.CompanyId.HasValue && _userContext.CompanyId > 0
                    ? _userContext.CompanyId
                    : null;
                
                var officeId = _userContext.IsAuthenticated && _userContext.OfficeId.HasValue && _userContext.OfficeId > 0
                    ? _userContext.OfficeId
                    : null;

                string cacheKey = $"AppParam_{parameterName}_{companyId}_{officeId}";

                if (!_cache.TryGetValue(cacheKey, out string? stringValue))
                {
                    stringValue = await _parameterScopeService.GetEffectiveParameterValueAsync(parameterName, companyId, officeId);
                    
                    if (stringValue != null)
                    {
                        var cacheOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Cache for 30 minutes
                            
                        _cache.Set(cacheKey, stringValue, cacheOptions);
                    }
                }
                
                if (string.IsNullOrEmpty(stringValue))
                    return defaultValue;

                return (T)Convert.ChangeType(stringValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task<int> GetMaxResultRecordAsync()
        {
            return await GetValueAsync<int>("MaxResultRecord", 150);
        }
    }
}
