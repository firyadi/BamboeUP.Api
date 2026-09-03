using Shared.DataTransferObjects;
using Shared.Helpers;

namespace Service.Contracts.Shell;

public interface ISharedParameterCatalogService
{
    Task<SharedParameterCatalogSnapshotDto> GetSnapshotAsync();
    Task<IReadOnlyList<SharedParameterCatalogFieldDto>> GetLibraryAsync();
    Task<IReadOnlyList<SharedParameterProfileDto>> GetProfilesAsync();
    Task<IReadOnlyList<CatalogField>> ApplyProfileAsync(string token, string? audience = null);
    Task<IReadOnlyList<SharedParameterProfileFieldDto>> GetMembershipsAsync(string? profileToken = null);

    Task<SharedParameterCatalogFieldDto> CreateFieldAsync(SharedParameterCatalogFieldForUpsertDto input);
    Task UpdateFieldAsync(Guid guid, SharedParameterCatalogFieldForUpsertDto input);
    Task DeleteFieldAsync(Guid guid, long deletedById);

    Task<SharedParameterProfileDto> CreateProfileAsync(SharedParameterProfileForUpsertDto input);
    Task UpdateProfileAsync(Guid guid, SharedParameterProfileForUpsertDto input);
    Task DeleteProfileAsync(Guid guid, long deletedById);

    Task<SharedParameterProfileFieldDto> CreateMembershipAsync(SharedParameterProfileFieldForUpsertDto input);
    Task UpdateMembershipAsync(Guid guid, SharedParameterProfileFieldForUpsertDto input);
    Task DeleteMembershipAsync(Guid guid, long deletedById);

    Task HydrateSharedCacheAsync();
}
