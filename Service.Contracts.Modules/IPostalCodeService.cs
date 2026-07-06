using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface IPostalCodeService
    {
        Task<IEnumerable<PostalCodeDto>> GetAllPostalCodesAsync(Guid subDistrictGuid, bool trackChanges);
        Task<PostalCodeDto> GetPostalCodeByGuidAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges);
        Task<PostalCodeDto> CreatePostalCodeAsync(Guid subDistrictGuid, PostalCodeForCreationDto postalCodeForCreation);
        Task UpdatePostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, PostalCodeForUpdateDto postalCodeForUpdate, bool trackChanges);
        Task DeletePostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, PostalCodeForDeleteDto postalCodeForDelete, bool trackChanges);
        Task DeletePostalCodeByAdminAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges);

        Task<IEnumerable<PostalCodeDto>> SearchPostalCodeAsync(
            Guid subDistrictGuid, string? postalCodeValue, string? postalCodeValueSearchType
        );
    }
}
