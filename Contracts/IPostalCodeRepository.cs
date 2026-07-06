using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IPostalCodeRepository
    {
        Task<PostalCode> GetPostalCodeAsync(Guid subDistrictGuid, Guid postalCodeGuid, bool trackChanges);
        Task<IEnumerable<PostalCode>> GetAllPostalCodesAsync(Guid subDistrictGuid, bool trackChanges);
        Task CreatePostalCodeAsync(PostalCode postalCode, IDbTransaction transaction = null);
        Task UpdatePostalCodeAsync(PostalCode postalCode, IDbTransaction transaction = null);
        Task DeletePostalCodeAsync(Guid postalCodeGuid, IDbTransaction transaction = null);
        Task SoftDeletePostalCodeAsync(PostalCode postalCode, long deletedBy, IDbTransaction transaction = null);

        Task<IEnumerable<PostalCode>> SearchPostalCodeAsync(
            Guid subDistrictGuid,
            string? postalCodeValue, string? postalCodeValueSearchType,
            IDbTransaction? transaction = null
        );
    }
}
