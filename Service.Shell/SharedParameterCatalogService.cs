using Contracts;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Shared.Helpers;

namespace Service.Shell;

public class SharedParameterCatalogService : ISharedParameterCatalogService
{
    private readonly ISharedParameterCatalogRepository _repository;

    public SharedParameterCatalogService(ISharedParameterCatalogRepository repository)
    {
        _repository = repository;
    }

    public Task<SharedParameterCatalogSnapshotDto> GetSnapshotAsync() => _repository.GetSnapshotAsync();

    public Task<IReadOnlyList<SharedParameterCatalogFieldDto>> GetLibraryAsync() => _repository.GetLibraryAsync();

    public Task<IReadOnlyList<SharedParameterProfileDto>> GetProfilesAsync() => _repository.GetProfilesAsync();

    public Task<IReadOnlyList<SharedParameterProfileFieldDto>> GetMembershipsAsync(string? profileToken = null)
        => _repository.GetMembershipsAsync(profileToken);

    public async Task<IReadOnlyList<CatalogField>> ApplyProfileAsync(string token, string? audience = null)
    {
        await HydrateSharedCacheAsync();
        var aud = ParseAudience(audience);
        return SharedParameterCatalog.ApplyProfile(token, aud);
    }

    public async Task<SharedParameterCatalogFieldDto> CreateFieldAsync(SharedParameterCatalogFieldForUpsertDto input)
    {
        NormalizeField(input);
        var guid = await _repository.CreateFieldAsync(input);
        await HydrateSharedCacheAsync();
        return (await _repository.GetFieldByGuidAsync(guid))!;
    }

    public async Task UpdateFieldAsync(Guid guid, SharedParameterCatalogFieldForUpsertDto input)
    {
        NormalizeField(input);
        await _repository.UpdateFieldAsync(guid, input);
        await HydrateSharedCacheAsync();
    }

    public async Task DeleteFieldAsync(Guid guid, long deletedById)
    {
        await _repository.SoftDeleteFieldAsync(guid, deletedById);
        await HydrateSharedCacheAsync();
    }

    public async Task<SharedParameterProfileDto> CreateProfileAsync(SharedParameterProfileForUpsertDto input)
    {
        input.ProfileToken = input.ProfileToken.Trim().ToLowerInvariant();
        var guid = await _repository.CreateProfileAsync(input);
        await HydrateSharedCacheAsync();
        return (await _repository.GetProfileByGuidAsync(guid))!;
    }

    public async Task UpdateProfileAsync(Guid guid, SharedParameterProfileForUpsertDto input)
    {
        input.ProfileToken = input.ProfileToken.Trim().ToLowerInvariant();
        await _repository.UpdateProfileAsync(guid, input);
        await HydrateSharedCacheAsync();
    }

    public async Task DeleteProfileAsync(Guid guid, long deletedById)
    {
        await _repository.SoftDeleteProfileAsync(guid, deletedById);
        await HydrateSharedCacheAsync();
    }

    public async Task<SharedParameterProfileFieldDto> CreateMembershipAsync(SharedParameterProfileFieldForUpsertDto input)
    {
        NormalizeMembership(input);
        var guid = await _repository.CreateMembershipAsync(input);
        await HydrateSharedCacheAsync();
        return (await _repository.GetMembershipByGuidAsync(guid))!;
    }

    public async Task UpdateMembershipAsync(Guid guid, SharedParameterProfileFieldForUpsertDto input)
    {
        NormalizeMembership(input);
        await _repository.UpdateMembershipAsync(guid, input);
        await HydrateSharedCacheAsync();
    }

    public async Task DeleteMembershipAsync(Guid guid, long deletedById)
    {
        await _repository.SoftDeleteMembershipAsync(guid, deletedById);
        await HydrateSharedCacheAsync();
    }

    public async Task HydrateSharedCacheAsync()
    {
        var snap = await _repository.GetSnapshotAsync();
        ApplySnapshotToCache(snap);
    }

    public static void ApplySnapshotToCache(SharedParameterCatalogSnapshotDto snap)
    {
        var library = snap.Library.Select(MapField).ToList();
        var profiles = snap.Profiles.Select(p => (p.ProfileToken, p.ProfileLabel)).ToList();
        var memberships = snap.Memberships.Select(m => new CatalogProfileMembership
        {
            SharedParameterProfileFieldId = m.SharedParameterProfileFieldId,
            SharedParameterProfileFieldGuid = m.SharedParameterProfileFieldGuid,
            ProfileToken = m.ProfileToken,
            CatalogCode = m.CatalogCode,
            SharedParameterCatalogId = m.SharedParameterCatalogId,
            Audience = m.Audience,
            SortOrder = m.SortOrder,
            OverrideLabel = m.OverrideLabel,
            OverrideDataType = m.OverrideDataType,
            OverrideControlType = m.OverrideControlType,
            OverrideIsRequired = m.OverrideIsRequired,
            OverrideLookupSource = m.OverrideLookupSource
        }).ToList();

        SharedParameterCatalog.ReplaceFrom(library, profiles, memberships);
    }

    private static CatalogField MapField(SharedParameterCatalogFieldDto f) => new()
    {
        SharedParameterCatalogId = f.SharedParameterCatalogId,
        SharedParameterCatalogGuid = f.SharedParameterCatalogGuid,
        Code = f.Code,
        Label = f.Label,
        DataType = f.DataType,
        CanonicalControlType = f.CanonicalControlType,
        ProcessControlType = f.ProcessControlType,
        ReportControlType = f.ReportControlType,
        IsRequired = f.IsRequired,
        DefaultValue = f.DefaultValue,
        LookupSource = f.LookupSource,
        GroupName = f.GroupName,
        SortOrder = f.SortOrder,
        IsSystemField = f.IsSystemField
    };

    private static CatalogAudience ParseAudience(string? audience)
    {
        if (string.IsNullOrWhiteSpace(audience))
            return CatalogAudience.Process;
        return Enum.TryParse<CatalogAudience>(audience.Trim(), ignoreCase: true, out var a)
            ? a
            : CatalogAudience.Process;
    }

    private static void NormalizeField(SharedParameterCatalogFieldForUpsertDto input)
    {
        input.Code = input.Code.Trim();
        input.Label = input.Label.Trim();
        input.DataType = input.DataType.Trim();
        input.CanonicalControlType = input.CanonicalControlType.Trim();
        input.ProcessControlType = input.ProcessControlType.Trim();
        input.ReportControlType = input.ReportControlType.Trim();
        if (string.IsNullOrWhiteSpace(input.ReportControlType))
            input.ReportControlType = input.CanonicalControlType;
    }

    private static void NormalizeMembership(SharedParameterProfileFieldForUpsertDto input)
    {
        input.Audience = input.Audience.Trim();
        if (!SharedParameterCatalog.Audiences.Contains(input.Audience, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Audience must be one of: {string.Join(", ", SharedParameterCatalog.Audiences)}");
    }
}
