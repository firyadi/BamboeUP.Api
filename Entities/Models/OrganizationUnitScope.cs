using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("OrganizationUnitScope", Schema = "app")]
    public class OrganizationUnitScope
    {
        [Column("OrganizationUnitScopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrganizationUnitScopeId { get; set; }

        [Key]
        [Column("OrganizationUnitScopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrganizationUnitScopeGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long OrganizationUnitId { get; set; }

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        [Required]
        public string ScopeType { get; set; }

public string? CompanyName { get; set; }
public string? CompanyOfficeName { get; set; }


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
