using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonEducationService
    {
        Task<IEnumerable<PersonEducationDto>> GetAllPersonEducationsAsync(bool trackChanges);
        Task<PersonEducationDto?> GetPersonEducationByGuidAsync(Guid personEducationGuid, bool trackChanges);
        Task<PersonEducationDto> CreatePersonEducationAsync(Guid personGuid, PersonEducationForCreationDto input);
        Task UpdatePersonEducationAsync(Guid personGuid, Guid personEducationGuid, PersonEducationForUpdateDto input, bool trackChanges);
        Task DeletePersonEducationAsync(Guid personGuid, Guid personEducationGuid, PersonEducationForDeleteDto input, bool trackChanges);
        Task DeletePersonEducationByAdminAsync(Guid personEducationGuid, bool trackChanges);

        Task<IEnumerable<PersonEducationDto>> SearchPersonEducationAsync(
            string? srEducationLevel, string? srEducationLevelSearchType, string? institutionName, string? institutionNameSearchType,
            Guid personGuid, Guid personEducationGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonEducationDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonEducationDto?> GetByPersonGuidAndPersonEducationGuidAsync(Guid personGuid, Guid personEducationGuid);
    }
}
