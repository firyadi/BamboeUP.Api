using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public sealed class SharedParameterCatalogFieldDto
{
    public long SharedParameterCatalogId { get; set; }
    public Guid SharedParameterCatalogGuid { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string CanonicalControlType { get; set; } = "TextBox";
    public string ProcessControlType { get; set; } = "Text";
    public string ReportControlType { get; set; } = "TextBox";
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? LookupSource { get; set; }
    public string? GroupName { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystemField { get; set; }
}

public sealed class SharedParameterCatalogFieldForUpsertDto
{
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Label { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string DataType { get; set; } = "string";

    [Required, MaxLength(30)]
    public string CanonicalControlType { get; set; } = "TextBox";

    [Required, MaxLength(30)]
    public string ProcessControlType { get; set; } = "Text";

    [Required, MaxLength(30)]
    public string ReportControlType { get; set; } = "TextBox";

    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? LookupSource { get; set; }
    public string? GroupName { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystemField { get; set; }
    public long CreatedById { get; set; } = 1;
    public long? UpdatedById { get; set; }
}

public sealed class SharedParameterProfileDto
{
    public long SharedParameterProfileId { get; set; }
    public Guid SharedParameterProfileGuid { get; set; }
    public string ProfileToken { get; set; } = string.Empty;
    public string ProfileLabel { get; set; } = string.Empty;
}

public sealed class SharedParameterProfileForUpsertDto
{
    [Required, MaxLength(30)]
    public string ProfileToken { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string ProfileLabel { get; set; } = string.Empty;

    public long CreatedById { get; set; } = 1;
    public long? UpdatedById { get; set; }
}

public sealed class SharedParameterProfileFieldDto
{
    public long SharedParameterProfileFieldId { get; set; }
    public Guid SharedParameterProfileFieldGuid { get; set; }
    public long SharedParameterProfileId { get; set; }
    public string ProfileToken { get; set; } = string.Empty;
    public long SharedParameterCatalogId { get; set; }
    public string CatalogCode { get; set; } = string.Empty;
    public string Audience { get; set; } = "Process";
    public int SortOrder { get; set; }
    public string? OverrideLabel { get; set; }
    public string? OverrideDataType { get; set; }
    public string? OverrideControlType { get; set; }
    public bool? OverrideIsRequired { get; set; }
    public string? OverrideLookupSource { get; set; }
}

public sealed class SharedParameterProfileFieldForUpsertDto
{
    public long SharedParameterProfileId { get; set; }
    public long SharedParameterCatalogId { get; set; }

    [Required, MaxLength(20)]
    public string Audience { get; set; } = "Process";

    public int SortOrder { get; set; }
    public string? OverrideLabel { get; set; }
    public string? OverrideDataType { get; set; }
    public string? OverrideControlType { get; set; }
    public bool? OverrideIsRequired { get; set; }
    public string? OverrideLookupSource { get; set; }
    public long CreatedById { get; set; } = 1;
    public long? UpdatedById { get; set; }
}

public sealed class SharedParameterCatalogSnapshotDto
{
    public IReadOnlyList<SharedParameterCatalogFieldDto> Library { get; set; } = [];
    public IReadOnlyList<SharedParameterProfileDto> Profiles { get; set; } = [];
    public IReadOnlyList<SharedParameterProfileFieldDto> Memberships { get; set; } = [];
}
