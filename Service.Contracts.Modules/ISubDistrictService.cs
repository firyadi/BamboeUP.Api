using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public interface ISubDistrictService
    {
        Task<IEnumerable<SubDistrictDto>> GetAllSubDistrictsAsync(Guid districtGuid, bool trackChanges);
        Task<SubDistrictDto> GetSubDistrictByGuidAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges);
        Task<SubDistrictDto> CreateSubDistrictAsync(Guid districtGuid, SubDistrictForCreationDto subDistrictForCreation);
        Task UpdateSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, SubDistrictForUpdateDto subDistrictForUpdate, bool trackChanges);
        Task DeleteSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, SubDistrictForDeleteDto subDistrictForDelete, bool trackChanges);
        Task DeleteSubDistrictByAdminAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges);

        Task<IEnumerable<SubDistrictDto>> SearchSubDistrictAsync(
            Guid districtGuid, string? subDistrictName, string? subDistrictNameSearchType
        );
    }
}
