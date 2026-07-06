namespace Shared.DataTransferObjects.AuditTrail;

public class AuditLogDto
{
    public long AuditLogID { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string AuditActionType { get; set; } = string.Empty;
    public string PrimaryKeyData { get; set; } = string.Empty;
    public string ActionByUserID { get; set; } = string.Empty;
    public DateTime LogDateTime { get; set; }
    public List<AuditLogChangeDto> Changes { get; set; } = [];
}
