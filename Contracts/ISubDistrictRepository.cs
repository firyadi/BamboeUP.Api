using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface ISubDistrictRepository
    {
        Task<long?> GetDistrictIdByGuidAsync(Guid districtGuid);
        Task<long?> GetSubDistrictIdByGuidAsync(Guid subDistrictGuid);
        Task<SubDistrict> GetSubDistrictAsync(Guid districtGuid, Guid subDistrictGuid, bool trackChanges);
        Task<IEnumerable<SubDistrict>> GetAllSubDistrictsAsync(Guid districtGuid, bool trackChanges);
        Task CreateSubDistrictAsync(SubDistrict subDistrict, IDbTransaction? transaction = null);
        Task UpdateSubDistrictAsync(SubDistrict subDistrict, IDbTransaction? transaction = null);
        Task DeleteSubDistrictAsync(Guid subDistrictGuid, IDbTransaction? transaction = null);
        Task SoftDeleteSubDistrictAsync(SubDistrict subDistrict, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<SubDistrict>> SearchSubDistrictAsync(
            Guid districtGuid,
            string? subDistrictName, string? subDistrictNameSearchType,
            IDbTransaction? transaction = null
        );
    }
}
