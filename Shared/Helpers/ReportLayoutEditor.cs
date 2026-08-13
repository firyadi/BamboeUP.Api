using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Helpers;

/// <summary>Build/parse ReportDefinition.LayoutJson for admin UI (mirrors SchemaStudio layout contract).</summary>
public static class ReportLayoutEditor
{
    public static string BuildJson(string headerProfile, string layoutPreset, string marginPreset)
    {
        var doc = new LayoutDocument
        {
            Paper = ResolvePaper(layoutPreset),
            Orientation = ResolveOrientation(layoutPreset),
            MarginPreset = NormalizeMargin(marginPreset),
            Header = BuildHeader(headerProfile)
        };

        return JsonSerializer.Serialize(doc, JsonOptions);
    }

    public static LayoutSelections Parse(string? layoutJson)
    {
        if (string.IsNullOrWhiteSpace(layoutJson))
            return new LayoutSelections();

        try
        {
            var doc = JsonSerializer.Deserialize<LayoutDocument>(layoutJson, JsonOptions) ?? new LayoutDocument();
            return new LayoutSelections
            {
                HeaderProfile = doc.Header?.Profile?.ToLowerInvariant() switch
                {
                    "standard" => "standard",
                    "letterhead" => "letterhead",
                    _ => "minimal"
                },
                LayoutPreset = ResolveLayoutPreset(doc.Paper, doc.Orientation),
                MarginPreset = doc.MarginPreset?.ToLowerInvariant() switch
                {
                    "narrow" => "narrow",
                    "wide" => "wide",
                    _ => "normal"
                }
            };
        }
        catch (JsonException)
        {
            return new LayoutSelections();
        }
    }

    public static string Describe(string? layoutJson)
    {
        var s = Parse(layoutJson);
        return $"{HeaderLabel(s.HeaderProfile)} · {LayoutLabel(s.LayoutPreset)} · margin {s.MarginPreset}";
    }

    public static string HeaderLabel(string profile) => profile switch
    {
        "standard" => "Standard (logo + company)",
        "letterhead" => "Letterhead",
        _ => "Minimal"
    };

    public static string LayoutLabel(string preset) => preset switch
    {
        "a4-landscape" => "A4 Landscape",
        "a5-portrait" => "A5 Portrait",
        _ => "A4 Portrait"
    };

    private static LayoutHeader BuildHeader(string headerProfile) => headerProfile switch
    {
        "standard" => new LayoutHeader
        {
            Profile = "Standard",
            ShowCompanyLogo = true,
            ShowCompanyName = true
        },
        "letterhead" => new LayoutHeader
        {
            Profile = "Letterhead",
            ShowCompanyLogo = true,
            ShowCompanyName = true,
            ShowOfficeName = true,
            ShowPrintedAt = true
        },
        _ => new LayoutHeader { Profile = "Minimal" }
    };

    private static string ResolvePaper(string layoutPreset) => layoutPreset switch
    {
        "a5-portrait" => "A5",
        _ => "A4"
    };

    private static string ResolveOrientation(string layoutPreset) =>
        layoutPreset == "a4-landscape" ? "Landscape" : "Portrait";

    private static string ResolveLayoutPreset(string? paper, string? orientation)
    {
        if (string.Equals(paper, "A5", StringComparison.OrdinalIgnoreCase))
            return "a5-portrait";
        if (string.Equals(orientation, "Landscape", StringComparison.OrdinalIgnoreCase))
            return "a4-landscape";
        return "a4-portrait";
    }

    private static string NormalizeMargin(string marginPreset) => marginPreset switch
    {
        "narrow" => "Narrow",
        "wide" => "Wide",
        _ => "Normal"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class LayoutDocument
    {
        public string Paper { get; set; } = "A4";
        public string Orientation { get; set; } = "Portrait";
        public string MarginPreset { get; set; } = "Normal";
        public LayoutHeader Header { get; set; } = new();
    }

    private sealed class LayoutHeader
    {
        public string Profile { get; set; } = "Minimal";
        public bool ShowCompanyLogo { get; set; }
        public bool ShowCompanyName { get; set; }
        public bool ShowOfficeName { get; set; }
        public bool ShowPrintedAt { get; set; }
    }
}

public sealed class LayoutSelections
{
    public string HeaderProfile { get; set; } = "minimal";
    public string LayoutPreset { get; set; } = "a4-portrait";
    public string MarginPreset { get; set; } = "normal";
}
