using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonContactService
    {
        Task<IEnumerable<PersonContactDto>> GetAllPersonContactsAsync(bool trackChanges);
        Task<PersonContactDto?> GetPersonContactByGuidAsync(Guid personContactGuid, bool trackChanges);
        Task<PersonContactDto> CreatePersonContactAsync(Guid personGuid, PersonContactForCreationDto input);
        Task UpdatePersonContactAsync(Guid personGuid, Guid personContactGuid, PersonContactForUpdateDto input, bool trackChanges);
        Task DeletePersonContactAsync(Guid personGuid, Guid personContactGuid, PersonContactForDeleteDto input, bool trackChanges);
        Task DeletePersonContactByAdminAsync(Guid personContactGuid, bool trackChanges);

        Task<IEnumerable<PersonContactDto>> SearchPersonContactAsync(
            string? srContactType, string? srContactTypeSearchType, string? contactValue, string? contactValueSearchType, string? isPrimary, string? isPrimarySearchType, string? remark, string? remarkSearchType,
            Guid personGuid, Guid personContactGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonContactDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonContactDto?> GetByPersonGuidAndPersonContactGuidAsync(Guid personGuid, Guid personContactGuid);
    }
}
