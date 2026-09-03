using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessResult", Schema = "core")]
public class ProcessResult
{
    [Column("ProcessResultId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessResultId { get; set; }

    public long ProcessExecutionId { get; set; }

    [Required]
    public string ResultStatus { get; set; } = "Success";

    public string? ResultMessage { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int WarningRecords { get; set; }
    public int FailedRecords { get; set; }
    public string? ResultReference { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
