using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonEmergencyContact", Schema = "app")]
    public partial class PersonEmergencyContact
    {
        [Column("PersonEmergencyContactId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonEmergencyContactId { get; set; }

        [Key]
        [Column("PersonEmergencyContactGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonEmergencyContactGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        [Required]
        public string ContactName { get; set; } = string.Empty;

        public long SrRelationship { get; set; }

        public string? Phone { get; set; }

        public bool IsPrimary { get; set; }



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
