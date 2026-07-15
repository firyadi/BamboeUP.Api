using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonEducationRepository
    {
        Task<PersonEducation?> GetPersonEducationAsync(Guid personEducationGuid, bool trackChanges);
        Task<PersonEducation?> GetByPersonGuidAndPersonEducationGuidAsync(Guid personGuid, Guid personEducationGuid);
        Task<IEnumerable<PersonEducation>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null);
        Task UpdatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null);
        Task SoftDeletePersonEducationAsync(PersonEducation personEducation, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonEducationAsync(Guid personEducationGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonEducation>> SearchPersonEducationAsync(
            string? srEducationLevel,
            string? srEducationLevelSearchType,
            string? institutionName,
            string? institutionNameSearchType,
            Guid personGuid, Guid personEducationGuid,
            IDbTransaction? transaction = null);
    }
}
