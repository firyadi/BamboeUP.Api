using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IStandardReferenceScopeService
    {
        Task<IEnumerable<StandardReferenceScopeDto>> GetAllStandardReferenceScopesAsync(bool trackChanges);
        Task<StandardReferenceScopeDto> GetStandardReferenceScopeByGuidAsync(Guid standardReferenceScopeGuid, bool trackChanges);
        Task<StandardReferenceScopeDto> CreateStandardReferenceScopeAsync(StandardReferenceScopeForCreationDto input);
        Task UpdateStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, StandardReferenceScopeForUpdateDto input, bool trackChanges);
        Task DeleteStandardReferenceScopeAsync(Guid standardReferenceScopeGuid, StandardReferenceScopeForDeleteDto input, bool trackChanges);
        Task DeleteStandardReferenceScopeByAdminAsync(Guid standardReferenceScopeGuid, bool trackChanges);

        Task<IEnumerable<StandardReferenceScopeDto>> SearchStandardReferenceScopeAsync(
            Guid companyGuid, Guid companyOfficeGuid,
            Guid standardReferenceGuid, Guid standardReferenceScopeGuid
        );

        Task<IEnumerable<StandardReferenceScopeDto>> GetAllByStandardReferenceGuidAsync(Guid standardReferenceGuid);
        Task<StandardReferenceScopeDto> GetByStandardReferenceGuidAndScopeGuidAsync(Guid standardReferenceGuid, Guid standardReferenceScopeGuid);
    }
}
