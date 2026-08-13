using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonFamilyRepository
    {
        Task<PersonFamily?> GetPersonFamilyAsync(Guid personFamilyGuid, bool trackChanges);
        Task<PersonFamily?> GetByPersonGuidAndPersonFamilyGuidAsync(Guid personGuid, Guid personFamilyGuid);
        Task<IEnumerable<PersonFamily>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null);
        Task UpdatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null);
        Task SoftDeletePersonFamilyAsync(PersonFamily personFamily, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonFamilyAsync(Guid personFamilyGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonFamily>> SearchPersonFamilyAsync(
            string? familyName,
            string? familyNameSearchType,
            string? dateBirth,
            string? dateBirthSearchType,
            string? address,
            string? addressSearchType,
            string? zipCode,
            string? zipCodeSearchType,
            string? phone,
            string? phoneSearchType,
            Guid personGuid, Guid personFamilyGuid,
            IDbTransaction? transaction = null);
    }
}
