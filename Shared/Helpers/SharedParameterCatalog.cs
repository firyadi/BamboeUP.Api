namespace Shared.Helpers;

/// <summary>
/// Shared parameter catalog for Report / Process / Pivot authoring.
/// Hydrate from DB/API via <see cref="ReplaceFrom"/>; otherwise BuiltIn fallback (tests / cold start).
/// </summary>
public static class SharedParameterCatalog
{
    private static readonly object Gate = new();
    private static IReadOnlyList<CatalogField> _library = BuiltInLibrary();
    private static IReadOnlyList<(string Token, string Label)> _profiles = BuiltInProfiles();
    private static IReadOnlyList<CatalogProfileMembership> _memberships = BuiltInMemberships();
    private static bool _hydratedFromStore;

    public static bool IsHydratedFromStore
    {
        get { lock (Gate) return _hydratedFromStore; }
    }

    public static IReadOnlyList<(string Token, string Label)> Profiles
    {
        get { lock (Gate) return _profiles; }
    }

    public static IReadOnlyList<string> CanonicalControlTypes { get; } =
    [
        "ReadonlyText", "TextBox", "NumberBox", "CheckBox",
        "DatePicker", "DateTimePicker", "ComboBox", "FileUpload"
    ];

    /// <summary>ProcessParameterDefinition ControlType aliases (DB / seed).</summary>
    public static IReadOnlyList<string> ProcessControlTypes { get; } =
    [
        "Text", "Number", "Boolean", "Date", "Lookup", "Dropdown", "FileUpload", "ReadonlyText"
    ];

    public static IReadOnlyList<string> Audiences { get; } = ["Process", "Report", "Pivot"];

    public static IReadOnlyList<CatalogField> Library
    {
        get { lock (Gate) return _library; }
    }

    public static IReadOnlyList<CatalogProfileMembership> Memberships
    {
        get { lock (Gate) return _memberships; }
    }

    /// <summary>Replace in-memory catalog from DB/API snapshot.</summary>
    public static void ReplaceFrom(
        IEnumerable<CatalogField> library,
        IEnumerable<(string Token, string Label)> profiles,
        IEnumerable<CatalogProfileMembership> memberships)
    {
        lock (Gate)
        {
            _library = library.Select(f => f.Clone()).OrderBy(f => f.SortOrder).ThenBy(f => f.Code).ToList();
            _profiles = profiles.Select(p => (p.Token, p.Label)).ToList();
            _memberships = memberships.Select(m => m.Clone()).ToList();
            _hydratedFromStore = true;
        }
    }

    /// <summary>Reset to BuiltIn (unit tests).</summary>
    public static void ResetToBuiltIn()
    {
        lock (Gate)
        {
            _library = BuiltInLibrary();
            _profiles = BuiltInProfiles();
            _memberships = BuiltInMemberships();
            _hydratedFromStore = false;
        }
    }

    public static IReadOnlyList<CatalogField> ApplyProfile(string? profileToken, CatalogAudience audience = CatalogAudience.Process)
    {
        var token = (profileToken ?? "none").Trim().ToLowerInvariant();
        if (token is "none" or "")
            return [];

        var audienceName = audience.ToString();
        List<CatalogProfileMembership> members;
        List<CatalogField> library;
        lock (Gate)
        {
            members = _memberships
                .Where(m => m.ProfileToken.Equals(token, StringComparison.OrdinalIgnoreCase)
                            && m.Audience.Equals(audienceName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.SortOrder)
                .ToList();
            library = _library.ToList();
        }

        if (members.Count == 0)
            return [];

        var result = new List<CatalogField>();
        foreach (var m in members)
        {
            var baseField = library.FirstOrDefault(f => f.Code.Equals(m.CatalogCode, StringComparison.OrdinalIgnoreCase));
            if (baseField == null)
                continue;
            result.Add(ApplyMembershipOverrides(baseField, m, audience));
        }

        return result;
    }

    public static CatalogField? FindLibrary(string code) =>
        Library.FirstOrDefault(f => f.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static string ToSqlType(string dataType) =>
        (dataType ?? "string").Trim().ToLowerInvariant() switch
        {
            "bigint" or "long" => "bigint",
            "int" => "int",
            "decimal" or "numeric" => "decimal(18,2)",
            "boolean" or "bool" or "bit" => "bit",
            "datetime" or "date" or "datetime2" => "datetime2",
            "uniqueidentifier" or "guid" => "uniqueidentifier",
            _ => "nvarchar(200)"
        };

    public static string ToProcessDataType(string dataType) =>
        (dataType ?? "string").Trim().ToLowerInvariant() switch
        {
            "bigint" or "long" => "bigint",
            "int" => "int",
            "decimal" or "numeric" => "decimal",
            "boolean" or "bool" or "bit" => "boolean",
            "datetime" or "date" or "datetime2" => "datetime",
            _ => "string"
        };

    private static CatalogField ApplyMembershipOverrides(CatalogField baseField, CatalogProfileMembership m, CatalogAudience audience)
    {
        var clone = baseField.Clone();
        if (!string.IsNullOrWhiteSpace(m.OverrideLabel))
            clone.Label = m.OverrideLabel!;
        if (!string.IsNullOrWhiteSpace(m.OverrideDataType))
            clone.DataType = m.OverrideDataType!;
        if (!string.IsNullOrWhiteSpace(m.OverrideControlType))
        {
            if (audience == CatalogAudience.Report)
            {
                clone.ProcessControlType = m.OverrideControlType!;
                // Report path historically used ProcessControlType slot for Lookup on Company
            }
            else
                clone.ProcessControlType = m.OverrideControlType!;
        }

        if (m.OverrideIsRequired.HasValue)
            clone.IsRequired = m.OverrideIsRequired.Value;
        if (m.OverrideLookupSource != null)
            clone.LookupSource = m.OverrideLookupSource;
        clone.SortOrder = m.SortOrder;
        return clone;
    }

    private static List<(string Token, string Label)> BuiltInProfiles() =>
    [
        ("none", "None — no parameters"),
        ("scope", "Scope — Company + Office"),
        ("period", "Period — Scope + Year/Month (Process) / Date range (Report)"),
        ("standard", "Standard — same as Period"),
        ("process", "Process — Period + ValidateOnly")
    ];

    private static List<CatalogField> BuiltInLibrary() =>
    [
        Field("CompanyId", "Company", "bigint", "ReadonlyText", "ReadonlyText", true, null, null, "Scope", 10),
        Field("CompanyOfficeId", "Office", "bigint", "ReadonlyText", "ReadonlyText", false, null, null, "Scope", 20),
        Field("PeriodYear", "Period Year", "int", "NumberBox", "Number", true, null, null, "Period", 30),
        Field("PeriodMonth", "Period Month", "int", "NumberBox", "Number", true, null, null, "Period", 40),
        Field("DateFrom", "Valid From", "datetime", "DatePicker", "Date", true, null, null, "Period", 30),
        Field("DateTo", "Valid To", "datetime", "DatePicker", "Date", true, null, null, "Period", 40),
        Field("ValidateOnly", "Validate Only", "boolean", "CheckBox", "Boolean", false, "false", null, "Options", 90),
        Field("SourceFile", "Source File", "string", "FileUpload", "FileUpload", true, null, null, "Input", 50),
        Field("ImportType", "Import Type", "string", "ComboBox", "Dropdown", true, null, "ImportType", "Input", 45),
        Field("EmployeeId", "Employee", "bigint", "ComboBox", "Lookup", false, null, "Employee", "Scope", 25)
    ];

    private static List<CatalogProfileMembership> BuiltInMemberships()
    {
        var list = new List<CatalogProfileMembership>();

        void Add(string profile, string code, string audience, int order,
            string? oLabel = null, string? oData = null, string? oCtrl = null, bool? oReq = null, string? oLookup = null)
            => list.Add(new CatalogProfileMembership
            {
                ProfileToken = profile,
                CatalogCode = code,
                Audience = audience,
                SortOrder = order,
                OverrideLabel = oLabel,
                OverrideDataType = oData,
                OverrideControlType = oCtrl,
                OverrideIsRequired = oReq,
                OverrideLookupSource = oLookup
            });

        Add("scope", "CompanyId", "Process", 10);
        Add("scope", "CompanyOfficeId", "Process", 20);
        Add("scope", "CompanyId", "Report", 10, "Company", "string", "Lookup", true, "Company");
        Add("scope", "CompanyOfficeId", "Report", 20, "Office", "string", "Lookup", true, "Office");
        Add("scope", "CompanyId", "Pivot", 10);
        Add("scope", "CompanyOfficeId", "Pivot", 20);

        foreach (var p in new[] { "period", "standard" })
        {
            Add(p, "CompanyId", "Process", 10);
            Add(p, "PeriodYear", "Process", 30);
            Add(p, "PeriodMonth", "Process", 40);
            Add(p, "CompanyId", "Report", 10, "Company", "string", "Lookup", true, "Company");
            Add(p, "CompanyOfficeId", "Report", 20, "Office", "string", "Lookup", true, "Office");
            Add(p, "DateFrom", "Report", 30);
            Add(p, "DateTo", "Report", 40);
            Add(p, "CompanyId", "Pivot", 10);
            Add(p, "PeriodYear", "Pivot", 30);
            Add(p, "PeriodMonth", "Pivot", 40);
        }

        Add("process", "CompanyId", "Process", 10);
        Add("process", "PeriodYear", "Process", 30);
        Add("process", "PeriodMonth", "Process", 40);
        Add("process", "ValidateOnly", "Process", 90);
        Add("process", "CompanyId", "Pivot", 10);
        Add("process", "PeriodYear", "Pivot", 30);
        Add("process", "PeriodMonth", "Pivot", 40);
        Add("process", "ValidateOnly", "Pivot", 90);

        return list;
    }

    private static CatalogField Field(
        string code,
        string label,
        string dataType,
        string canonical,
        string process,
        bool required,
        string? defaultValue,
        string? lookup,
        string group,
        int order) => new()
    {
        Code = code,
        Label = label,
        DataType = dataType,
        CanonicalControlType = canonical,
        ProcessControlType = process,
        ReportControlType = canonical,
        IsRequired = required,
        DefaultValue = defaultValue,
        LookupSource = lookup,
        GroupName = group,
        SortOrder = order
    };
}

public enum CatalogAudience
{
    Process,
    Report,
    Pivot
}

public sealed class CatalogField
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

    public CatalogField Clone() => new()
    {
        SharedParameterCatalogId = SharedParameterCatalogId,
        SharedParameterCatalogGuid = SharedParameterCatalogGuid,
        Code = Code,
        Label = Label,
        DataType = DataType,
        CanonicalControlType = CanonicalControlType,
        ProcessControlType = ProcessControlType,
        ReportControlType = ReportControlType,
        IsRequired = IsRequired,
        DefaultValue = DefaultValue,
        LookupSource = LookupSource,
        GroupName = GroupName,
        SortOrder = SortOrder,
        IsSystemField = IsSystemField
    };
}

public sealed class CatalogProfileMembership
{
    public long SharedParameterProfileFieldId { get; set; }
    public Guid SharedParameterProfileFieldGuid { get; set; }
    public string ProfileToken { get; set; } = string.Empty;
    public string CatalogCode { get; set; } = string.Empty;
    public long SharedParameterCatalogId { get; set; }
    public string Audience { get; set; } = "Process";
    public int SortOrder { get; set; }
    public string? OverrideLabel { get; set; }
    public string? OverrideDataType { get; set; }
    public string? OverrideControlType { get; set; }
    public bool? OverrideIsRequired { get; set; }
    public string? OverrideLookupSource { get; set; }

    public CatalogProfileMembership Clone() => new()
    {
        SharedParameterProfileFieldId = SharedParameterProfileFieldId,
        SharedParameterProfileFieldGuid = SharedParameterProfileFieldGuid,
        ProfileToken = ProfileToken,
        CatalogCode = CatalogCode,
        SharedParameterCatalogId = SharedParameterCatalogId,
        Audience = Audience,
        SortOrder = SortOrder,
        OverrideLabel = OverrideLabel,
        OverrideDataType = OverrideDataType,
        OverrideControlType = OverrideControlType,
        OverrideIsRequired = OverrideIsRequired,
        OverrideLookupSource = OverrideLookupSource
    };
}
