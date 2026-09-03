using Shared.DataTransferObjects;

namespace Contracts;

public interface ISharedParameterCatalogRepository
{
    Task<IReadOnlyList<SharedParameterCatalogFieldDto>> GetLibraryAsync();
    Task<SharedParameterCatalogFieldDto?> GetFieldByGuidAsync(Guid guid);
    Task<Guid> CreateFieldAsync(SharedParameterCatalogFieldForUpsertDto input);
    Task UpdateFieldAsync(Guid guid, SharedParameterCatalogFieldForUpsertDto input);
    Task SoftDeleteFieldAsync(Guid guid, long deletedById);

    Task<IReadOnlyList<SharedParameterProfileDto>> GetProfilesAsync();
    Task<SharedParameterProfileDto?> GetProfileByGuidAsync(Guid guid);
    Task<Guid> CreateProfileAsync(SharedParameterProfileForUpsertDto input);
    Task UpdateProfileAsync(Guid guid, SharedParameterProfileForUpsertDto input);
    Task SoftDeleteProfileAsync(Guid guid, long deletedById);

    Task<IReadOnlyList<SharedParameterProfileFieldDto>> GetMembershipsAsync(string? profileToken = null);
    Task<SharedParameterProfileFieldDto?> GetMembershipByGuidAsync(Guid guid);
    Task<Guid> CreateMembershipAsync(SharedParameterProfileFieldForUpsertDto input);
    Task UpdateMembershipAsync(Guid guid, SharedParameterProfileFieldForUpsertDto input);
    Task SoftDeleteMembershipAsync(Guid guid, long deletedById);

    Task<SharedParameterCatalogSnapshotDto> GetSnapshotAsync();
}
