using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonPhysicalCharacteristicService
    {
        Task<IEnumerable<PersonPhysicalCharacteristicDto>> GetAllPersonPhysicalCharacteristicsAsync(bool trackChanges);
        Task<PersonPhysicalCharacteristicDto?> GetPersonPhysicalCharacteristicByGuidAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges);
        Task<PersonPhysicalCharacteristicDto> CreatePersonPhysicalCharacteristicAsync(Guid personGuid, PersonPhysicalCharacteristicForCreationDto input);
        Task UpdatePersonPhysicalCharacteristicAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid, PersonPhysicalCharacteristicForUpdateDto input, bool trackChanges);
        Task DeletePersonPhysicalCharacteristicAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid, PersonPhysicalCharacteristicForDeleteDto input, bool trackChanges);
        Task DeletePersonPhysicalCharacteristicByAdminAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges);

        Task<IEnumerable<PersonPhysicalCharacteristicDto>> SearchPersonPhysicalCharacteristicAsync(
            string? srPhysicalCharacteristic, string? srPhysicalCharacteristicSearchType, string? physicalValue, string? physicalValueSearchType, string? srMeasurementUnit, string? srMeasurementUnitSearchType, string? recordedDate, string? recordedDateSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personPhysicalCharacteristicGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonPhysicalCharacteristicDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonPhysicalCharacteristicDto?> GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid);
    }
}
