using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IStandardReferenceScopeItemService
    {
        Task<IEnumerable<StandardReferenceScopeItemDto>> GetAllStandardReferenceScopeItemsAsync(bool trackChanges);
        Task<StandardReferenceScopeItemDto> GetStandardReferenceScopeItemByGuidAsync(Guid scopeItemGuid, bool trackChanges);
        Task<StandardReferenceScopeItemDto> CreateStandardReferenceScopeItemAsync(StandardReferenceScopeItemForCreationDto input);
        Task UpdateStandardReferenceScopeItemAsync(Guid scopeItemGuid, StandardReferenceScopeItemForUpdateDto input, bool trackChanges);
        Task DeleteStandardReferenceScopeItemAsync(Guid scopeItemGuid, StandardReferenceScopeItemForDeleteDto input, bool trackChanges);
        Task DeleteStandardReferenceScopeItemByAdminAsync(Guid scopeItemGuid, bool trackChanges);

        Task<IEnumerable<StandardReferenceScopeItemDto>> SearchStandardReferenceScopeItemAsync(
            string? scopeItemInitial, string? scopeItemInitialSearchType,
            Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid
        );

        Task<IEnumerable<StandardReferenceScopeItemDto>> GetAllByStandardReferenceScopeGuidAsync(Guid standardReferenceScopeGuid);
        Task<StandardReferenceScopeItemDto> GetByScopeGuidAndItemGuidAsync(Guid standardReferenceScopeGuid, Guid standardReferenceScopeItemGuid);
    }
}
