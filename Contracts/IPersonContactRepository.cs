using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonContactRepository
    {
        Task<PersonContact?> GetPersonContactAsync(Guid personContactGuid, bool trackChanges);
        Task<PersonContact?> GetByPersonGuidAndPersonContactGuidAsync(Guid personGuid, Guid personContactGuid);
        Task<IEnumerable<PersonContact>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonContactAsync(PersonContact personContact, IDbTransaction? transaction = null);
        Task UpdatePersonContactAsync(PersonContact personContact, IDbTransaction? transaction = null);
        Task SoftDeletePersonContactAsync(PersonContact personContact, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonContactAsync(Guid personContactGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonContact>> SearchPersonContactAsync(
            string? srContactType,
            string? srContactTypeSearchType,
            string? contactValue,
            string? contactValueSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? remark,
            string? remarkSearchType,
            Guid personGuid, Guid personContactGuid,
            IDbTransaction? transaction = null);
    }
}
