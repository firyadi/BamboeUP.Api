using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonFamilyService
    {
        Task<IEnumerable<PersonFamilyDto>> GetAllPersonFamiliesAsync(bool trackChanges);
        Task<PersonFamilyDto?> GetPersonFamilyByGuidAsync(Guid personFamilyGuid, bool trackChanges);
        Task<PersonFamilyDto> CreatePersonFamilyAsync(Guid personGuid, PersonFamilyForCreationDto input);
        Task UpdatePersonFamilyAsync(Guid personGuid, Guid personFamilyGuid, PersonFamilyForUpdateDto input, bool trackChanges);
        Task DeletePersonFamilyAsync(Guid personGuid, Guid personFamilyGuid, PersonFamilyForDeleteDto input, bool trackChanges);
        Task DeletePersonFamilyByAdminAsync(Guid personFamilyGuid, bool trackChanges);

        Task<IEnumerable<PersonFamilyDto>> SearchPersonFamilyAsync(
            string? familyName, string? familyNameSearchType, string? dateBirth, string? dateBirthSearchType, string? address, string? addressSearchType, string? zipCode, string? zipCodeSearchType, string? phone, string? phoneSearchType,
            Guid personGuid, Guid personFamilyGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonFamilyDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonFamilyDto?> GetByPersonGuidAndPersonFamilyGuidAsync(Guid personGuid, Guid personFamilyGuid);
    }
}
