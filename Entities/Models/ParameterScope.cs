using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("ParameterScope", Schema = "app")]
    public class Parameterscope
    {
        [Column("ParameterscopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ParameterscopeId { get; set; }

        [Key]
        [Column("ParameterscopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ParameterscopeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long ParameterId { get; set; }

        [Required]
        public Guid ParameterGuid { get; set; }

        public string? ParameterName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyOfficeName { get; set; }

        public long? CompanyId { get; set; }

        public Guid? CompanyGuid { get; set; }

        public long? CompanyOfficeId { get; set; }

        public Guid? CompanyOfficeGuid { get; set; }

        [Required]
        [MaxLength(200)]
        public string Overridevalue { get; set; } = string.Empty;

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
