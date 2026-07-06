using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IParameterscopeService
    {
        Task<IEnumerable<ParameterscopeDto>> GetAllCompanyOfficesAsync(bool trackChanges); // fallback
        Task<ParameterscopeDto> GetParameterscopeByGuidAsync(Guid parameterscopeGuid, bool trackChanges);
        Task<ParameterscopeDto> CreateParameterscopeAsync(ParameterscopeForCreationDto input);
        Task UpdateParameterscopeAsync(Guid parameterscopeGuid, ParameterscopeForUpdateDto input, bool trackChanges);
        Task DeleteParameterscopeAsync(Guid parameterscopeGuid, ParameterscopeForDeleteDto input, bool trackChanges);
        Task DeleteParameterscopeByAdminAsync(Guid parameterscopeGuid, bool trackChanges);

        Task<IEnumerable<ParameterscopeDto>> SearchParameterscopeAsync(
            string? overridevalue, string? overridevalueSearchType,
            Guid parameterGuid, Guid parameterscopeGuid);

        // Detail (child) helpers
        Task<IEnumerable<ParameterscopeDto>> GetAllByParameterGuidAsync(Guid parameterGuid);
        Task<ParameterscopeDto> GetByParameterGuidAndParameterscopeGuidAsync(Guid parameterGuid, Guid parameterscopeGuid);

        // AppParameterManager helper
        Task<string?> GetEffectiveParameterValueAsync(string parameterName, long? companyId, long? officeId);
    }
}
