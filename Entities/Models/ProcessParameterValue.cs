using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

[Table("ProcessParameterValue", Schema = "core")]
public class ProcessParameterValue
{
    [Column("ProcessParameterValueId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessParameterValueId { get; set; }

    public long ProcessExecutionId { get; set; }

    [Required]
    public string ParameterCode { get; set; } = string.Empty;

    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public string? ValueJson { get; set; }
    public byte[]? ValueBlob { get; set; }
}
