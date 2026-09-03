using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessStepDefinition", Schema = "core")]
public class ProcessStepDefinition
{
    [Column("ProcessStepDefinitionId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessStepDefinitionId { get; set; }

    [Key]
    [Column("ProcessStepDefinitionGuid")]
    public Guid ProcessStepDefinitionGuid { get; set; } = Guid.NewGuid();

    public long ProcessDefinitionId { get; set; }
    public int StepNo { get; set; }

    [Required]
    public string StepCode { get; set; } = string.Empty;

    [Required]
    public string StepName { get; set; } = string.Empty;

    [Required]
    public string StepType { get; set; } = "Custom";

    public bool IsMandatory { get; set; } = true;
    public bool AllowRetry { get; set; } = true;
    public bool AllowSkip { get; set; }
    public int ExecutionOrder { get; set; }
    public string? ConfigurationJson { get; set; }
    public int StatusId { get; set; } = 1;
    public long CreatedById { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
