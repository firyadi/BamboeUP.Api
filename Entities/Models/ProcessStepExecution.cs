using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessStepExecution", Schema = "core")]
public class ProcessStepExecution
{
    [Column("ProcessStepExecutionId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessStepExecutionId { get; set; }

    public long ProcessExecutionId { get; set; }

    [Required]
    public string StepCode { get; set; } = string.Empty;

    [Required]
    public string StepName { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Pending";

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal ProgressPercent { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int WarningCount { get; set; }
    public int FailedCount { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}
