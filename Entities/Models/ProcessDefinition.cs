using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessDefinition", Schema = "core")]
public class ProcessDefinition
{
    [Column("ProcessDefinitionId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessDefinitionId { get; set; }

    [Key]
    [Column("ProcessDefinitionGuid")]
    public Guid ProcessDefinitionGuid { get; set; } = Guid.NewGuid();

    [Required]
    public string ProcessCode { get; set; } = string.Empty;

    [Required]
    public string ProcessName { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Version { get; set; } = 1;
    public byte ExecutionMode { get; set; } = 2;
    public bool RequiresApproval { get; set; }
    public bool AllowRetry { get; set; } = true;
    public bool AllowCancel { get; set; } = true;
    public bool AllowSchedule { get; set; }
    public bool IsActive { get; set; } = true;
    public int StatusId { get; set; } = 1;

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public long CreatedById { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public long? DeletedById { get; set; }
    public DateTime? DeletedTime { get; set; }
}
