using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ITempContoh1Service
    {
        Task<IEnumerable<TempContoh1Dto>> GetAllTempContoh1sAsync(bool trackChanges);
        Task<TempContoh1Dto?> GetTempContoh1ByGuidAsync(Guid tempContoh1Guid, bool trackChanges);
        Task<TempContoh1Dto> CreateTempContoh1Async(TempContoh1ForCreationDto input);
        Task UpdateTempContoh1Async(Guid tempContoh1Guid, TempContoh1ForUpdateDto input, bool trackChanges);
        Task DeleteTempContoh1Async(Guid tempContoh1Guid, TempContoh1ForDeleteDto input, bool trackChanges);
        Task DeleteTempContoh1ByAdminAsync(Guid tempContoh1Guid, bool trackChanges);

        Task<IEnumerable<TempContoh1Dto>> SearchTempContoh1Async(
            string? personName, string? personNameSearchType,
            string? awardTypeName, string? awardTypeNameSearchType
        );
    }
}
