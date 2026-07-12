using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IParameterRepository
    {
        Task<Parameter?> GetParameterAsync(Guid parameterGuid, bool trackChanges);
        Task<Parameter?> GetParameterByIdAsync(long parameterId, bool trackChanges);
        Task<IEnumerable<Parameter>> GetAllParametersAsync(bool trackChanges);

        Task CreateParameterAsync(Parameter parameter, IDbTransaction? transaction = null);
        Task UpdateParameterAsync(Parameter parameter, IDbTransaction? transaction = null);
        Task DeleteParameterAsync(Guid parameterGuid, IDbTransaction? transaction = null);
        Task SoftDeleteParameterAsync(Parameter parameter, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<Parameter>> SearchParameterAsync(
            string? parametername, string? parameternameSearchType,
            string? parametervalue, string? parametervalueSearchType,

            IDbTransaction? transaction = null);
    }
}
