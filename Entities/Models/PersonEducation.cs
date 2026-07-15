using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonEducation", Schema = "app")]
    public class PersonEducation
    {
        [Column("PersonEducationId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonEducationId { get; set; }

        [Key]
        [Column("PersonEducationGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonEducationGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long SrEducationLevel { get; set; }

        [Required]
        public string InstitutionName { get; set; } = string.Empty;



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
