using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IStandardReferenceItemService
    {
        Task<IEnumerable<StandardReferenceItemDto>> GetAllStandardReferenceItemsAsync(bool trackChanges);
        Task<StandardReferenceItemDto> GetStandardReferenceItemByGuidAsync(Guid standardReferenceItemGuid, bool trackChanges);
        Task<StandardReferenceItemDto> CreateStandardReferenceItemAsync(StandardReferenceItemForCreationDto input);
        Task UpdateStandardReferenceItemAsync(Guid standardReferenceItemGuid, StandardReferenceItemForUpdateDto input, bool trackChanges);
        Task DeleteStandardReferenceItemAsync(Guid standardReferenceItemGuid, StandardReferenceItemForDeleteDto input, bool trackChanges);
        Task DeleteStandardReferenceItemByAdminAsync(Guid standardReferenceItemGuid, bool trackChanges);

        Task<IEnumerable<StandardReferenceItemDto>> SearchStandardReferenceItemAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType, string? standardReferenceItemInitial, string? standardReferenceItemInitialSearchType, string? standardReferenceItemName, string? standardReferenceItemNameSearchType, string? note, string? noteSearchType, Guid companyGuid, Guid companyOfficeGuid
        );

        // Detail helpers
        Task<IEnumerable<StandardReferenceItemDto>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid);
        Task<StandardReferenceItemDto> GetByStandardReferenceGuidAndStandardReferenceItemGuidAsync(Guid standardReferenceGuid, Guid standardReferenceItemGuid);
    }
}
