using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IStandardReferenceService
    {
        Task<IEnumerable<StandardReferenceDto>> GetAllStandardReferencesAsync(bool trackChanges);
        Task<IEnumerable<StandardReferenceDto>> GetStandardReferencesForParentSelectionAsync(Guid? currentRecordGuid, bool trackChanges);
        Task<StandardReferenceDto> GetStandardReferenceByGuidAsync(Guid standardReferenceGuid, bool trackChanges);
        Task<StandardReferenceDto> CreateStandardReferenceAsync(StandardReferenceForCreationDto input);
        Task UpdateStandardReferenceAsync(Guid standardReferenceGuid, StandardReferenceForUpdateDto input, bool trackChanges);
        Task DeleteStandardReferenceAsync(Guid standardReferenceGuid, StandardReferenceForDeleteDto input, bool trackChanges);
        Task DeleteStandardReferenceByAdminAsync(Guid standardReferenceGuid, bool trackChanges);

        Task<IEnumerable<StandardReferenceDto>> SearchStandardReferenceAsync(
            string? standardReferenceInitial, string? standardReferenceInitialSearchType, string? standardReferenceName, string? standardReferenceNameSearchType, string? note, string? noteSearchType, Guid companyGuid, Guid companyOfficeGuid
        );

    }
}
