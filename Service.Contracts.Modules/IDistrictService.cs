using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface IDistrictService
    {
        Task<IEnumerable<DistrictDto>> GetAllDistrictsAsync(Guid cityGuid, bool trackChanges);
        Task<DistrictDto> GetDistrictByGuidAsync(Guid cityGuid, Guid districtGuid, bool trackChanges);
        Task<DistrictDto> CreateDistrictAsync(Guid cityGuid, DistrictForCreationDto districtForCreation);
        Task UpdateDistrictAsync(Guid cityGuid, Guid districtGuid, DistrictForUpdateDto districtForUpdate, bool trackChanges);
        Task DeleteDistrictAsync(Guid cityGuid, Guid districtGuid, DistrictForDeleteDto districtForDelete, bool trackChanges);
        Task DeleteDistrictByAdminAsync(Guid cityGuid, Guid districtGuid, bool trackChanges);

        Task<IEnumerable<DistrictDto>> SearchDistrictAsync(
            Guid cityGuid, string? districtName, string? districtNameSearchType
        );
    }
}
