using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessExecution", Schema = "core")]
public class ProcessExecution
{
    [Column("ProcessExecutionId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessExecutionId { get; set; }

    [Key]
    [Column("ProcessExecutionGuid")]
    public Guid ProcessExecutionGuid { get; set; } = Guid.NewGuid();

    public long ProcessDefinitionId { get; set; }

    [Required]
    public string ExecutionNo { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Draft";

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long StartedById { get; set; }
    public decimal ProgressPercent { get; set; }
    public string? CurrentStepCode { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public long? CompanyId { get; set; }
    public long? CompanyOfficeId { get; set; }
    public long? ParentExecutionId { get; set; }
    public DateTime? CancellationRequestedAt { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public long CreatedById { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
