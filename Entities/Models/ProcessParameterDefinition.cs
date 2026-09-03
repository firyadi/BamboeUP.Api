using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessParameterDefinition", Schema = "core")]
public class ProcessParameterDefinition
{
    [Column("ProcessParameterDefinitionId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessParameterDefinitionId { get; set; }

    [Key]
    [Column("ProcessParameterDefinitionGuid")]
    public Guid ProcessParameterDefinitionGuid { get; set; } = Guid.NewGuid();

    public long ProcessDefinitionId { get; set; }

    [Required]
    public string ParameterCode { get; set; } = string.Empty;

    [Required]
    public string ParameterName { get; set; } = string.Empty;

    [Required]
    public string DataType { get; set; } = "string";

    [Required]
    public string ControlType { get; set; } = "Text";

    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRuleJson { get; set; }
    public string? LookupSource { get; set; }
    public int DisplayOrder { get; set; }
    public string? GroupName { get; set; }
    public bool IsVisible { get; set; } = true;
    public int StatusId { get; set; } = 1;
    public long CreatedById { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public long? UpdatedById { get; set; }
    public DateTime? UpdatedTime { get; set; }
}
