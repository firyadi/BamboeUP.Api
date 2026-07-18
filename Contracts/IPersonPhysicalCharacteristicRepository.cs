using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonPhysicalCharacteristicRepository
    {
        Task<PersonPhysicalCharacteristic?> GetPersonPhysicalCharacteristicAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges);
        Task<PersonPhysicalCharacteristic?> GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid);
        Task<IEnumerable<PersonPhysicalCharacteristic>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, IDbTransaction? transaction = null);
        Task UpdatePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, IDbTransaction? transaction = null);
        Task SoftDeletePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonPhysicalCharacteristicAsync(Guid personPhysicalCharacteristicGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonPhysicalCharacteristic>> SearchPersonPhysicalCharacteristicAsync(
            string? srPhysicalCharacteristic,
            string? srPhysicalCharacteristicSearchType,
            string? physicalValue,
            string? physicalValueSearchType,
            string? srMeasurementUnit,
            string? srMeasurementUnitSearchType,
            string? recordedDate,
            string? recordedDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid personGuid, Guid personPhysicalCharacteristicGuid,
            IDbTransaction? transaction = null);
    }
}
