using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IDistrictRepository
    {
        Task<long?> GetCityIdByGuidAsync(Guid cityGuid);
        Task<long?> GetDistrictIdByGuidAsync(Guid districtGuid);
        Task<District?> GetDistrictAsync(Guid cityGuid, Guid districtGuid, bool trackChanges);
        Task<IEnumerable<District>> GetAllDistrictsAsync(Guid cityGuid, bool trackChanges);
        Task CreateDistrictAsync(District district, IDbTransaction? transaction = null);
        Task UpdateDistrictAsync(District district, IDbTransaction? transaction = null);
        Task DeleteDistrictAsync(Guid districtGuid, IDbTransaction? transaction = null);
        Task SoftDeleteDistrictAsync(District district, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<District>> SearchDistrictAsync(
            Guid cityGuid,
            string? districtName, string? districtNameSearchType,
            IDbTransaction? transaction = null
        );
    }
}
