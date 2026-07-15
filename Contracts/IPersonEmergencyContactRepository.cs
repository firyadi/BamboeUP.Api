using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonEmergencyContactRepository
    {
        Task<PersonEmergencyContact?> GetPersonEmergencyContactAsync(Guid personEmergencyContactGuid, bool trackChanges);
        Task<PersonEmergencyContact?> GetByPersonGuidAndPersonEmergencyContactGuidAsync(Guid personGuid, Guid personEmergencyContactGuid);
        Task<IEnumerable<PersonEmergencyContact>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, IDbTransaction? transaction = null);
        Task UpdatePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, IDbTransaction? transaction = null);
        Task SoftDeletePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonEmergencyContactAsync(Guid personEmergencyContactGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonEmergencyContact>> SearchPersonEmergencyContactAsync(
            string? contactName,
            string? contactNameSearchType,
            string? srRelationship,
            string? srRelationshipSearchType,
            string? phone,
            string? phoneSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            Guid personGuid, Guid personEmergencyContactGuid,
            IDbTransaction? transaction = null);
    }
}
