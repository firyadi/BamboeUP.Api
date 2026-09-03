using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessMessage", Schema = "core")]
public class ProcessMessage
{
    [Column("ProcessMessageId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessMessageId { get; set; }

    public long ProcessExecutionId { get; set; }
    public string? StepCode { get; set; }

    [Required]
    public string Severity { get; set; } = "Information";

    [Required]
    public string Source { get; set; } = "Business";

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? DetailJson { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
