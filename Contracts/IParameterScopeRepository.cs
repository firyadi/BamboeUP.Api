using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IParameterscopeRepository
    {
        Task<Parameterscope> GetParameterscopeAsync(Guid parameterscopeGuid, bool trackChanges);
        Task<Parameterscope> GetByParameterGuidAndParameterscopeGuidAsync(Guid parameterGuid, Guid parameterscopeGuid);
        Task<IEnumerable<Parameterscope>> GetAllByParameterGuidAsync(Guid parameterGuid);

        Task CreateParameterscopeAsync(Parameterscope parameterscope, IDbTransaction? transaction = null);
        Task UpdateParameterscopeAsync(Parameterscope parameterscope, IDbTransaction? transaction = null);
        Task SoftDeleteParameterscopeAsync(Parameterscope parameterscope, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteParameterscopeAsync(Guid parameterscopeGuid, IDbTransaction? transaction = null);
        Task DeleteByParameterGuidAsync(Guid parameterGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<Parameterscope>> SearchParameterscopeAsync(
            string? overridevalue, string? overridevalueSearchType,
            Guid parameterGuid, Guid parameterscopeGuid,
            IDbTransaction? transaction = null);

        Task<string?> GetEffectiveParameterValueAsync(string parameterName, long? companyId, long? officeId);
    }
}
