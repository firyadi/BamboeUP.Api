using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonAddressService
    {
        Task<IEnumerable<PersonAddressDto>> GetAllPersonAddressesAsync(bool trackChanges);
        Task<PersonAddressDto?> GetPersonAddressByGuidAsync(Guid personAddressGuid, bool trackChanges);
        Task<PersonAddressDto> CreatePersonAddressAsync(Guid personGuid, PersonAddressForCreationDto input);
        Task UpdatePersonAddressAsync(Guid personGuid, Guid personAddressGuid, PersonAddressForUpdateDto input, bool trackChanges);
        Task DeletePersonAddressAsync(Guid personGuid, Guid personAddressGuid, PersonAddressForDeleteDto input, bool trackChanges);
        Task DeletePersonAddressByAdminAsync(Guid personAddressGuid, bool trackChanges);

        Task<IEnumerable<PersonAddressDto>> SearchPersonAddressAsync(
            string? srAddressType, string? srAddressTypeSearchType, string? address, string? addressSearchType,
            Guid personGuid, Guid personAddressGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonAddressDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonAddressDto?> GetByPersonGuidAndPersonAddressGuidAsync(Guid personGuid, Guid personAddressGuid);
    }
}
