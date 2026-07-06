using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public partial interface IParameterService
    {
        Task<IEnumerable<ParameterDto>> GetAllParametersAsync(bool trackChanges);
        Task<ParameterDto> GetParameterByGuidAsync(Guid parameterGuid, bool trackChanges);
        Task<ParameterDto> CreateParameterAsync(ParameterForCreationDto input);
        Task UpdateParameterAsync(Guid parameterGuid, ParameterForUpdateDto input, bool trackChanges);
        Task DeleteParameterAsync(Guid parameterGuid, ParameterForDeleteDto input, bool trackChanges);
        Task DeleteParameterByAdminAsync(Guid parameterGuid, bool trackChanges);

        Task<IEnumerable<ParameterDto>> SearchParameterAsync(
            string? parametername, string? parameternameSearchType,
            string? parametervalue, string? parametervalueSearchType

        );
    }
}
