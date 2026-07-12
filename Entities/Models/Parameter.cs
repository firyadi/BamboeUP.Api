using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Parameter", Schema = "app")]
    public class Parameter
    {
        [Column("ParameterId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ParameterId { get; set; }

        [Key]
        [Column("ParameterGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ParameterGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Parametername { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Parametervalue { get; set; } = string.Empty;





        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
