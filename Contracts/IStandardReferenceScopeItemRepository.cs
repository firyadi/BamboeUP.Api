using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IStandardReferenceScopeItemRepository
    {
        Task<StandardReferenceScopeItem?> GetStandardReferenceScopeItemAsync(Guid scopeItemGuid, bool trackChanges);
        Task<IEnumerable<StandardReferenceScopeItem>> GetAllStandardReferenceScopeItemsAsync(bool trackChanges);
        Task CreateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null);
        Task UpdateStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, IDbTransaction? transaction = null);
        Task DeleteStandardReferenceScopeItemAsync(Guid scopeItemGuid, IDbTransaction? transaction = null);
        Task SoftDeleteStandardReferenceScopeItemAsync(StandardReferenceScopeItem scopeItem, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<StandardReferenceScopeItem>> SearchStandardReferenceScopeItemAsync(
            string? scopeItemInitial, string? scopeItemInitialSearchType,
            Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid,
            IDbTransaction? transaction = null);

        Task<IEnumerable<StandardReferenceScopeItem>> GetAllByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid);
        Task<StandardReferenceScopeItem?> GetByScopeGuidAndItemGuidAsync(Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid);
    }
}
