using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessResultDetail", Schema = "core")]
public class ProcessResultDetail
{
    [Column("ProcessResultDetailId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessResultDetailId { get; set; }

    public long ProcessExecutionId { get; set; }

    [Required]
    public string RecordKey { get; set; } = string.Empty;

    public string? RecordLabel { get; set; }

    [Required]
    public string Severity { get; set; } = "Error";

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? ReferenceId { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
}
