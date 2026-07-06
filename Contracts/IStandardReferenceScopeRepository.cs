using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IStandardReferenceScopeRepository
    {
        Task<StandardReferenceScope> GetStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, bool trackChanges);
        Task<IEnumerable<StandardReferenceScope>> GetAllStandardReferenceScopesAsync(bool trackChanges);
        Task CreateStandardReferenceScopeAsync(StandardReferenceScope scope, IDbTransaction? transaction = null);
        Task UpdateStandardReferenceScopeAsync(StandardReferenceScope scope, IDbTransaction? transaction = null);
        Task DeleteStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, IDbTransaction? transaction = null);
        Task SoftDeleteStandardReferenceScopeAsync(StandardReferenceScope scope, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteByStandardReferenceGuidAsync(Guid standardReferenceGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<StandardReferenceScope>> SearchStandardReferenceScopeAsync(
            Guid companyGuid, Guid companyOfficeGuid,
            Guid standardReferenceGuid, Guid standardReferenceScopeGuid,
            IDbTransaction? transaction = null);

        Task<IEnumerable<StandardReferenceScope>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid);
        Task<StandardReferenceScope> GetByStandardReferenceGuidAndScopeGuidAsync(Guid standardReferenceGuid, Guid standardReferenceScopeGuid);
    }
}
