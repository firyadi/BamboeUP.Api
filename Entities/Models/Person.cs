using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Person", Schema = "app")]
    public partial class Person
    {
        [Column("PersonId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonId { get; set; }

        [Key]
        [Column("PersonGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public string? PreTitle { get; set; }

        public string? PostTitle { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? PersonName { get; set; }

        public string? BirthName { get; set; }

        public string? PlaceofBirth { get; set; }

        public DateTime BirthDate { get; set; }

        public string? NationalIdNo { get; set; }

        public long SrGender { get; set; }

        public long SrReligion { get; set; }

        public long? SrSalutation { get; set; }

        public long? SrBloodType { get; set; }

        public long? SrMaritalStatus { get; set; }

        public string? Photo { get; set; }



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
