namespace Shared.DataTransferObjects.AuditTrail;

public class AuditLogChangeDto
{
    public string ColumnName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
