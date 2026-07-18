using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonFamily", Schema = "app")]
    public partial class PersonFamily
    {
        [Column("PersonFamilyId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonFamilyId { get; set; }

        [Key]
        [Column("PersonFamilyGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonFamilyGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public long SrFamilyRelation { get; set; }

        [Required]
        public string FamilyName { get; set; } = string.Empty;

        public DateTime DateBirth { get; set; }

        public long? SrEducationLevel { get; set; }

        public string? Address { get; set; }

        public long? StateId { get; set; }

        public long? CityId { get; set; }

        public string? ZipCode { get; set; }

        public string? Phone { get; set; }

        public long? SrMaritalStatus { get; set; }

        public long? SrGender { get; set; }



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
