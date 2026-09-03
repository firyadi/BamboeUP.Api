namespace Shared.DataTransferObjects;

/// <summary>Lightweight card for TemplateUiKanban (UI-09).</summary>
public sealed class KanbanCardDto
{
    public Guid CardGuid { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string? StatusName { get; set; }
    public string? Module { get; set; }
    public string? Type { get; set; }
    public string? Owner { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int? ComponentCount { get; set; }
    public int? CompletionPercent { get; set; }
    public int? ErrorCount { get; set; }
    public int? WarningCount { get; set; }
    public string? Framework { get; set; }
    public string? Tags { get; set; }
    public string? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool CanEdit { get; set; } = true;
    public bool CanDelete { get; set; } = true;
}

public sealed class KanbanColumnDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public int? WipLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public bool AllowDragIn { get; set; } = true;
    public bool AllowDragOut { get; set; } = true;
    public bool RequireApproval { get; set; }
    public bool IsTerminal { get; set; }
}

public sealed class KanbanTransitionDto
{
    public string FromCode { get; set; } = string.Empty;
    public List<string> ToCodes { get; set; } = [];
}

public sealed class KanbanCardFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Order { get; set; }
}

public sealed class KanbanBoardDto
{
    public string BoardTitle { get; set; } = string.Empty;
    public List<KanbanColumnDto> Columns { get; set; } = [];
    public List<KanbanCardDto> Cards { get; set; } = [];
    public List<KanbanTransitionDto> Transitions { get; set; } = [];
    public List<KanbanCardFieldDto> CardFields { get; set; } = [];
}

public sealed class KanbanCardMoveDto
{
    public Guid CardGuid { get; set; }
    public string FromStatusCode { get; set; } = string.Empty;
    public string ToStatusCode { get; set; } = string.Empty;
    public int? TargetIndex { get; set; }
    public string? Note { get; set; }
}

public sealed class KanbanMoveConfirmResult
{
    public bool Confirmed { get; set; }
    public string? Note { get; set; }
}

public sealed class KanbanBoardQueryDto
{
    public string? Search { get; set; }
    public string? Module { get; set; }
    public string? Type { get; set; }
    public string? Owner { get; set; }
    public string? StatusCode { get; set; }
}

/// <summary>Snapshot written by UiKanbanGenerator.</summary>
public sealed class KanbanMappingSnapshotDto
{
    public string KanbanCode { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string TitleField { get; set; } = string.Empty;
    public string StatusField { get; set; } = string.Empty;
    public string? CodeField { get; set; }
    public string? OwnerField { get; set; }
    public string? DescriptionField { get; set; }
    public string Route { get; set; } = string.Empty;
    public string ApiBoardPath { get; set; } = string.Empty;
    public string ApiMovePath { get; set; } = string.Empty;
    public string DefaultSort { get; set; } = "ModifiedAtDesc";
    public string Density { get; set; } = "Comfortable";
    public List<KanbanColumnDto> Columns { get; set; } = [];
    public List<KanbanTransitionDto> Transitions { get; set; } = [];
    public List<KanbanCardFieldDto> CardFields { get; set; } = [];
}
