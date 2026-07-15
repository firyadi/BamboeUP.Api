using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonAddressRepository
    {
        Task<PersonAddress?> GetPersonAddressAsync(Guid personAddressGuid, bool trackChanges);
        Task<PersonAddress?> GetByPersonGuidAndPersonAddressGuidAsync(Guid personGuid, Guid personAddressGuid);
        Task<IEnumerable<PersonAddress>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null);
        Task UpdatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null);
        Task SoftDeletePersonAddressAsync(PersonAddress personAddress, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonAddressAsync(Guid personAddressGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonAddress>> SearchPersonAddressAsync(
            string? srAddressType,
            string? srAddressTypeSearchType,
            string? address,
            string? addressSearchType,
            string? countryId,
            string? countryIdSearchType,
            string? provinceId,
            string? provinceIdSearchType,
            string? cityId,
            string? cityIdSearchType,
            Guid personGuid, Guid personAddressGuid,
            IDbTransaction? transaction = null);
    }
}
