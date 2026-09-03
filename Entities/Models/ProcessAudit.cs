using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessAudit", Schema = "core")]
public class ProcessAudit
{
    [Column("ProcessAuditId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessAuditId { get; set; }

    public long ProcessExecutionId { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty;

    public string? StepCode { get; set; }
    public long UserId { get; set; }
    public string? ClientIp { get; set; }
    public string? DetailJson { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
