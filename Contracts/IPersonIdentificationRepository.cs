using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonIdentificationRepository
    {
        Task<PersonIdentification?> GetPersonIdentificationAsync(Guid personIdentificationGuid, bool trackChanges);
        Task<PersonIdentification?> GetByPersonGuidAndPersonIdentificationGuidAsync(Guid personGuid, Guid personIdentificationGuid);
        Task<IEnumerable<PersonIdentification>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null);
        Task UpdatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null);
        Task SoftDeletePersonIdentificationAsync(PersonIdentification personIdentification, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonIdentificationAsync(Guid personIdentificationGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonIdentification>> SearchPersonIdentificationAsync(
            string? srIdentificationType,
            string? srIdentificationTypeSearchType,
            string? identificationValue,
            string? identificationValueSearchType,
            Guid personGuid, Guid personIdentificationGuid,
            IDbTransaction? transaction = null);
    }
}
