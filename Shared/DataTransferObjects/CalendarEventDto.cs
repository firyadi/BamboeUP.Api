using System.Text.Json.Serialization;

namespace Shared.DataTransferObjects;

/// <summary>Generic calendar event projection for TemplateUiCalendar (UI-08).</summary>
public sealed class CalendarEventDto
{
    public Guid EventGuid { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public bool IsAllDay { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ResourceId { get; set; }
    public long? PersonId { get; set; }
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;

    [JsonExtensionData]
    public Dictionary<string, object?>? Metadata { get; set; }
}

public sealed class CalendarEventsQueryDto
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
}

/// <summary>Snapshot written by UiCalendarGenerator.</summary>
public sealed class CalendarMappingSnapshotDto
{
    public string CalendarCode { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string TimeMode { get; set; } = "DateOnly";
    public string DefaultView { get; set; } = "Month";
    public string[] EnabledViews { get; set; } = ["Month", "Agenda"];
    public bool AllowCreate { get; set; } = true;
    public bool AllowEdit { get; set; } = true;
    public bool AllowDelete { get; set; } = true;
    public bool AllowDragDrop { get; set; }
    public bool AllowResize { get; set; }
    public bool EnableRecurrence { get; set; }
    public bool EnableReminder { get; set; }
    public bool EnableResource { get; set; }
    public bool EnableConflictDetection { get; set; }
    public string TitleField { get; set; } = string.Empty;
    public string StartField { get; set; } = string.Empty;
    public string? EndField { get; set; }
    public string? DescriptionField { get; set; }
    public string? CategoryField { get; set; }
    public string? StatusField { get; set; }
    public string? LocationField { get; set; }
    public string Route { get; set; } = string.Empty;
    public string ApiEventsPath { get; set; } = string.Empty;
}
