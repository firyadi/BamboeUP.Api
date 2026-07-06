using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IProvinceRepository
    {
        Task<Province> GetProvinceAsync(Guid countryGuid, Guid provinceGuid, bool trackChanges);
        Task<IEnumerable<Province>> GetAllProvincesAsync(Guid countryGuid, bool trackChanges);
        Task CreateProvinceAsync(Province province, IDbTransaction transaction = null);
        Task UpdateProvinceAsync(Province province, IDbTransaction transaction = null);
        Task DeleteProvinceAsync(Guid provinceGuid, IDbTransaction transaction = null);
        Task SoftDeleteProvinceAsync(Province province, long deletedBy, IDbTransaction transaction = null);

        Task<IEnumerable<Province>> SearchProvinceAsync(
            Guid countryGuid,
            string? provinceName, string? provinceNameSearchType,
            IDbTransaction? transaction = null
        );
    }
}
