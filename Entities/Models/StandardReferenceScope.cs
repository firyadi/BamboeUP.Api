using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("StandardReferenceScope", Schema = "app")]
    public class StandardReferenceScope
    {
        [Column("StandardReferenceScopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StandardReferenceScopeId { get; set; }

        [Key]
        [Column("StandardReferenceScopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StandardReferenceScopeGuid { get; set; } = Guid.NewGuid();

        [Column("StandardReferenceId")]
        public long StandardReferenceId { get; set; }

        [Column("StandardReferenceGuid")]
        public Guid StandardReferenceGuid { get; set; }

        [Column("CompanyId")]
        public long? CompanyId { get; set; }

        [Column("CompanyGuid")]
        public Guid? CompanyGuid { get; set; }

        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        [Column("CompanyOfficeGuid")]
        public Guid? CompanyOfficeGuid { get; set; }

        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }

        // Joined properties for easier mapping/display
        [NotMapped]
        public string? StandardReferenceName { get; set; }

        [NotMapped]
        public string? CompanyName { get; set; }

        [NotMapped]
        public string? CompanyOfficeName { get; set; }
    }
}
