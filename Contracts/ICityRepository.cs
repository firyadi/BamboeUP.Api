using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface ICityRepository
    {
        Task<long?> GetProvinceIdByGuidAsync(Guid provinceGuid);
        Task<long?> GetCityIdByGuidAsync(Guid cityGuid);
        Task<City> GetCityAsync(Guid provinceGuid, Guid cityGuid, bool trackChanges);
        Task<IEnumerable<City>> GetAllCitiesAsync(Guid provinceGuid, bool trackChanges);
        Task CreateCityAsync(City city, IDbTransaction transaction = null);
        Task UpdateCityAsync(City city, IDbTransaction transaction = null);
        Task DeleteCityAsync(Guid cityGuid, IDbTransaction transaction = null);
        Task SoftDeleteCityAsync(City city, long deletedBy, IDbTransaction transaction = null);

        Task<IEnumerable<City>> SearchCityAsync(
            Guid provinceGuid,
            string? cityName, string? cityNameSearchType,
            IDbTransaction? transaction = null
        );
    }
}
