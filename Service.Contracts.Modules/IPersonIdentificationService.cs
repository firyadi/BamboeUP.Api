using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonIdentificationService
    {
        Task<IEnumerable<PersonIdentificationDto>> GetAllPersonIdentificationsAsync(bool trackChanges);
        Task<PersonIdentificationDto?> GetPersonIdentificationByGuidAsync(Guid personIdentificationGuid, bool trackChanges);
        Task<PersonIdentificationDto> CreatePersonIdentificationAsync(Guid personGuid, PersonIdentificationForCreationDto input);
        Task UpdatePersonIdentificationAsync(Guid personGuid, Guid personIdentificationGuid, PersonIdentificationForUpdateDto input, bool trackChanges);
        Task DeletePersonIdentificationAsync(Guid personGuid, Guid personIdentificationGuid, PersonIdentificationForDeleteDto input, bool trackChanges);
        Task DeletePersonIdentificationByAdminAsync(Guid personIdentificationGuid, bool trackChanges);

        Task<IEnumerable<PersonIdentificationDto>> SearchPersonIdentificationAsync(
            string? srIdentificationTypeId, string? srIdentificationTypeIdSearchType, string? identificationValue, string? identificationValueSearchType,
            Guid personGuid, Guid personIdentificationGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonIdentificationDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonIdentificationDto?> GetByPersonGuidAndPersonIdentificationGuidAsync(Guid personGuid, Guid personIdentificationGuid);
    }
}
