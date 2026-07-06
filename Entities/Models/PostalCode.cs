using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PostalCode", Schema = "core")]
    public class PostalCode
    {
        [Column("PostalCodeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PostalCodeId { get; set; }

        [Key]
        [Column("PostalCodeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostalCodeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long SubDistrictId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("PostalCode", TypeName = "varchar(10)")]
        public string PostalCodeValue { get; set; }

        [Required]
        public int StatusId { get; set; }  = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }  = DateTime.UtcNow;

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
