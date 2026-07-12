using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface ICountryRepository
    {
        Task<Country> GetCountryAsync(Guid countryGuid, bool trackChanges);
        Task<IEnumerable<Country>> GetAllCountriesAsync(bool trackChanges);
        Task CreateCountryAsync(Country country, IDbTransaction? transaction = null);
        Task UpdateCountryAsync(Country country, IDbTransaction? transaction = null);
        Task DeleteCountryAsync(Guid countryGuid, IDbTransaction? transaction = null);
        Task SoftDeleteCountryAsync(Country country, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<Country>> SearchCountryAsync(
            string? countryName, string? countryNameSearchType,
            string? countryIso, string? countryIsoSearchType,
            IDbTransaction? transaction = null
        );
    }
}
