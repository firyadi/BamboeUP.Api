using Contracts;
using Dapper;
using Shared.DataTransferObjects;

namespace Repository;

public class SharedParameterCatalogRepository : ISharedParameterCatalogRepository
{
    private readonly RepositoryContext _context;

    private const string FieldSelect = @"
        c.SharedParameterCatalogId, c.SharedParameterCatalogGuid, c.Code, c.Label, c.DataType,
        c.CanonicalControlType, c.ProcessControlType, c.ReportControlType, c.IsRequired, c.DefaultValue,
        c.LookupSource, c.GroupName, c.SortOrder, c.IsSystemField";

    private const string ProfileSelect = @"
        p.SharedParameterProfileId, p.SharedParameterProfileGuid, p.ProfileToken, p.ProfileLabel";

    private const string MemberSelect = @"
        m.SharedParameterProfileFieldId, m.SharedParameterProfileFieldGuid,
        m.SharedParameterProfileId, p.ProfileToken,
        m.SharedParameterCatalogId, c.Code AS CatalogCode,
        m.Audience, m.SortOrder,
        m.OverrideLabel, m.OverrideDataType, m.OverrideControlType, m.OverrideIsRequired, m.OverrideLookupSource";

    public SharedParameterCatalogRepository(RepositoryContext context) => _context = context;

    public async Task<IReadOnlyList<SharedParameterCatalogFieldDto>> GetLibraryAsync()
    {
        using var connection = _context.CreateConnection();
        var rows = await connection.QueryAsync<SharedParameterCatalogFieldDto>($@"
            SELECT {FieldSelect}
            FROM [core].[SharedParameterCatalog] c
            WHERE c.StatusId > 0 AND c.DeletedTime IS NULL
            ORDER BY c.SortOrder, c.Code");
        return rows.ToList();
    }

    public async Task<SharedParameterCatalogFieldDto?> GetFieldByGuidAsync(Guid guid)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SharedParameterCatalogFieldDto>($@"
            SELECT {FieldSelect}
            FROM [core].[SharedParameterCatalog] c
            WHERE c.SharedParameterCatalogGuid = @guid AND c.StatusId > 0 AND c.DeletedTime IS NULL",
            new { guid });
    }

    public async Task<Guid> CreateFieldAsync(SharedParameterCatalogFieldForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var guid = Guid.NewGuid();
        await connection.ExecuteAsync(@"
            INSERT INTO [core].[SharedParameterCatalog]
            (SharedParameterCatalogGuid, Code, Label, DataType, CanonicalControlType, ProcessControlType, ReportControlType,
             IsRequired, DefaultValue, LookupSource, GroupName, SortOrder, IsSystemField, StatusId, CreatedById, CreatedTime)
            VALUES
            (@Guid, @Code, @Label, @DataType, @CanonicalControlType, @ProcessControlType, @ReportControlType,
             @IsRequired, @DefaultValue, @LookupSource, @GroupName, @SortOrder, @IsSystemField, 1, @CreatedById, SYSDATETIME())",
            new
            {
                Guid = guid,
                input.Code,
                input.Label,
                input.DataType,
                input.CanonicalControlType,
                input.ProcessControlType,
                input.ReportControlType,
                input.IsRequired,
                input.DefaultValue,
                input.LookupSource,
                input.GroupName,
                input.SortOrder,
                input.IsSystemField,
                input.CreatedById
            });
        return guid;
    }

    public async Task UpdateFieldAsync(Guid guid, SharedParameterCatalogFieldForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterCatalog]
            SET Code = @Code,
                Label = @Label,
                DataType = @DataType,
                CanonicalControlType = @CanonicalControlType,
                ProcessControlType = @ProcessControlType,
                ReportControlType = @ReportControlType,
                IsRequired = @IsRequired,
                DefaultValue = @DefaultValue,
                LookupSource = @LookupSource,
                GroupName = @GroupName,
                SortOrder = @SortOrder,
                IsSystemField = @IsSystemField,
                UpdatedById = @UpdatedById,
                UpdatedTime = SYSDATETIME()
            WHERE SharedParameterCatalogGuid = @Guid AND DeletedTime IS NULL",
            new
            {
                Guid = guid,
                input.Code,
                input.Label,
                input.DataType,
                input.CanonicalControlType,
                input.ProcessControlType,
                input.ReportControlType,
                input.IsRequired,
                input.DefaultValue,
                input.LookupSource,
                input.GroupName,
                input.SortOrder,
                input.IsSystemField,
                UpdatedById = input.UpdatedById ?? input.CreatedById
            });
        if (n == 0)
            throw new KeyNotFoundException($"Catalog field {guid} not found.");
    }

    public async Task SoftDeleteFieldAsync(Guid guid, long deletedById)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterCatalog]
            SET StatusId = 0, DeletedById = @deletedById, DeletedTime = SYSDATETIME()
            WHERE SharedParameterCatalogGuid = @guid AND DeletedTime IS NULL AND IsSystemField = 0",
            new { guid, deletedById });
        if (n == 0)
            throw new InvalidOperationException("Field not found or is a system field (cannot delete).");
    }

    public async Task<IReadOnlyList<SharedParameterProfileDto>> GetProfilesAsync()
    {
        using var connection = _context.CreateConnection();
        var rows = await connection.QueryAsync<SharedParameterProfileDto>($@"
            SELECT {ProfileSelect}
            FROM [core].[SharedParameterProfile] p
            WHERE p.StatusId > 0 AND p.DeletedTime IS NULL
            ORDER BY
                CASE p.ProfileToken
                    WHEN 'none' THEN 0 WHEN 'scope' THEN 1 WHEN 'period' THEN 2
                    WHEN 'standard' THEN 3 WHEN 'process' THEN 4 ELSE 99 END,
                p.ProfileToken");
        return rows.ToList();
    }

    public async Task<SharedParameterProfileDto?> GetProfileByGuidAsync(Guid guid)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SharedParameterProfileDto>($@"
            SELECT {ProfileSelect}
            FROM [core].[SharedParameterProfile] p
            WHERE p.SharedParameterProfileGuid = @guid AND p.StatusId > 0 AND p.DeletedTime IS NULL",
            new { guid });
    }

    public async Task<Guid> CreateProfileAsync(SharedParameterProfileForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var guid = Guid.NewGuid();
        await connection.ExecuteAsync(@"
            INSERT INTO [core].[SharedParameterProfile]
            (SharedParameterProfileGuid, ProfileToken, ProfileLabel, StatusId, CreatedById, CreatedTime)
            VALUES (@Guid, @ProfileToken, @ProfileLabel, 1, @CreatedById, SYSDATETIME())",
            new { Guid = guid, input.ProfileToken, input.ProfileLabel, input.CreatedById });
        return guid;
    }

    public async Task UpdateProfileAsync(Guid guid, SharedParameterProfileForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterProfile]
            SET ProfileToken = @ProfileToken,
                ProfileLabel = @ProfileLabel,
                UpdatedById = @UpdatedById,
                UpdatedTime = SYSDATETIME()
            WHERE SharedParameterProfileGuid = @Guid AND DeletedTime IS NULL",
            new
            {
                Guid = guid,
                input.ProfileToken,
                input.ProfileLabel,
                UpdatedById = input.UpdatedById ?? input.CreatedById
            });
        if (n == 0)
            throw new KeyNotFoundException($"Profile {guid} not found.");
    }

    public async Task SoftDeleteProfileAsync(Guid guid, long deletedById)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterProfile]
            SET StatusId = 0, DeletedById = @deletedById, DeletedTime = SYSDATETIME()
            WHERE SharedParameterProfileGuid = @guid AND DeletedTime IS NULL
              AND ProfileToken NOT IN ('none','scope','period','standard','process')",
            new { guid, deletedById });
        if (n == 0)
            throw new InvalidOperationException("Profile not found or is a built-in profile (cannot delete).");
    }

    public async Task<IReadOnlyList<SharedParameterProfileFieldDto>> GetMembershipsAsync(string? profileToken = null)
    {
        using var connection = _context.CreateConnection();
        var sql = $@"
            SELECT {MemberSelect}
            FROM [core].[SharedParameterProfileField] m
            INNER JOIN [core].[SharedParameterProfile] p ON p.SharedParameterProfileId = m.SharedParameterProfileId
            INNER JOIN [core].[SharedParameterCatalog] c ON c.SharedParameterCatalogId = m.SharedParameterCatalogId
            WHERE m.StatusId > 0 AND m.DeletedTime IS NULL
              AND p.DeletedTime IS NULL AND c.DeletedTime IS NULL
              AND (@ProfileToken IS NULL OR p.ProfileToken = @ProfileToken)
            ORDER BY p.ProfileToken, m.Audience, m.SortOrder, c.Code";
        var rows = await connection.QueryAsync<SharedParameterProfileFieldDto>(
            sql, new { ProfileToken = string.IsNullOrWhiteSpace(profileToken) ? null : profileToken.Trim() });
        return rows.ToList();
    }

    public async Task<SharedParameterProfileFieldDto?> GetMembershipByGuidAsync(Guid guid)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SharedParameterProfileFieldDto>($@"
            SELECT {MemberSelect}
            FROM [core].[SharedParameterProfileField] m
            INNER JOIN [core].[SharedParameterProfile] p ON p.SharedParameterProfileId = m.SharedParameterProfileId
            INNER JOIN [core].[SharedParameterCatalog] c ON c.SharedParameterCatalogId = m.SharedParameterCatalogId
            WHERE m.SharedParameterProfileFieldGuid = @guid AND m.StatusId > 0 AND m.DeletedTime IS NULL",
            new { guid });
    }

    public async Task<Guid> CreateMembershipAsync(SharedParameterProfileFieldForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var guid = Guid.NewGuid();
        await connection.ExecuteAsync(@"
            INSERT INTO [core].[SharedParameterProfileField]
            (SharedParameterProfileFieldGuid, SharedParameterProfileId, SharedParameterCatalogId, Audience, SortOrder,
             OverrideLabel, OverrideDataType, OverrideControlType, OverrideIsRequired, OverrideLookupSource,
             StatusId, CreatedById, CreatedTime)
            VALUES
            (@Guid, @SharedParameterProfileId, @SharedParameterCatalogId, @Audience, @SortOrder,
             @OverrideLabel, @OverrideDataType, @OverrideControlType, @OverrideIsRequired, @OverrideLookupSource,
             1, @CreatedById, SYSDATETIME())",
            new
            {
                Guid = guid,
                input.SharedParameterProfileId,
                input.SharedParameterCatalogId,
                Audience = input.Audience.Trim(),
                input.SortOrder,
                input.OverrideLabel,
                input.OverrideDataType,
                input.OverrideControlType,
                input.OverrideIsRequired,
                input.OverrideLookupSource,
                input.CreatedById
            });
        return guid;
    }

    public async Task UpdateMembershipAsync(Guid guid, SharedParameterProfileFieldForUpsertDto input)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterProfileField]
            SET SharedParameterProfileId = @SharedParameterProfileId,
                SharedParameterCatalogId = @SharedParameterCatalogId,
                Audience = @Audience,
                SortOrder = @SortOrder,
                OverrideLabel = @OverrideLabel,
                OverrideDataType = @OverrideDataType,
                OverrideControlType = @OverrideControlType,
                OverrideIsRequired = @OverrideIsRequired,
                OverrideLookupSource = @OverrideLookupSource,
                UpdatedById = @UpdatedById,
                UpdatedTime = SYSDATETIME()
            WHERE SharedParameterProfileFieldGuid = @Guid AND DeletedTime IS NULL",
            new
            {
                Guid = guid,
                input.SharedParameterProfileId,
                input.SharedParameterCatalogId,
                Audience = input.Audience.Trim(),
                input.SortOrder,
                input.OverrideLabel,
                input.OverrideDataType,
                input.OverrideControlType,
                input.OverrideIsRequired,
                input.OverrideLookupSource,
                UpdatedById = input.UpdatedById ?? input.CreatedById
            });
        if (n == 0)
            throw new KeyNotFoundException($"Membership {guid} not found.");
    }

    public async Task SoftDeleteMembershipAsync(Guid guid, long deletedById)
    {
        using var connection = _context.CreateConnection();
        var n = await connection.ExecuteAsync(@"
            UPDATE [core].[SharedParameterProfileField]
            SET StatusId = 0, DeletedById = @deletedById, DeletedTime = SYSDATETIME()
            WHERE SharedParameterProfileFieldGuid = @guid AND DeletedTime IS NULL",
            new { guid, deletedById });
        if (n == 0)
            throw new KeyNotFoundException($"Membership {guid} not found.");
    }

    public async Task<SharedParameterCatalogSnapshotDto> GetSnapshotAsync()
    {
        var library = await GetLibraryAsync();
        var profiles = await GetProfilesAsync();
        var memberships = await GetMembershipsAsync();
        return new SharedParameterCatalogSnapshotDto
        {
            Library = library,
            Profiles = profiles,
            Memberships = memberships
        };
    }
}
