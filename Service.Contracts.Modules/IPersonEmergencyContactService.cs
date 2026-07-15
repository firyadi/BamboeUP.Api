using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonEmergencyContactService
    {
        Task<IEnumerable<PersonEmergencyContactDto>> GetAllPersonEmergencyContactsAsync(bool trackChanges);
        Task<PersonEmergencyContactDto?> GetPersonEmergencyContactByGuidAsync(Guid personEmergencyContactGuid, bool trackChanges);
        Task<PersonEmergencyContactDto> CreatePersonEmergencyContactAsync(Guid personGuid, PersonEmergencyContactForCreationDto input);
        Task UpdatePersonEmergencyContactAsync(Guid personGuid, Guid personEmergencyContactGuid, PersonEmergencyContactForUpdateDto input, bool trackChanges);
        Task DeletePersonEmergencyContactAsync(Guid personGuid, Guid personEmergencyContactGuid, PersonEmergencyContactForDeleteDto input, bool trackChanges);
        Task DeletePersonEmergencyContactByAdminAsync(Guid personEmergencyContactGuid, bool trackChanges);

        Task<IEnumerable<PersonEmergencyContactDto>> SearchPersonEmergencyContactAsync(
            string? contactName, string? contactNameSearchType, string? srRelationship, string? srRelationshipSearchType, string? phone, string? phoneSearchType, string? isPrimary, string? isPrimarySearchType,
            Guid personGuid, Guid personEmergencyContactGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonEmergencyContactDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonEmergencyContactDto?> GetByPersonGuidAndPersonEmergencyContactGuidAsync(Guid personGuid, Guid personEmergencyContactGuid);
    }
}
