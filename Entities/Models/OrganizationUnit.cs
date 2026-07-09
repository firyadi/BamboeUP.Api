using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("OrganizationUnit", Schema = "app")]
    public class OrganizationUnit
    {
        [Column("OrganizationUnitId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrganizationUnitId { get; set; }

        [Key]
        [Column("OrganizationUnitGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrganizationUnitGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string OrganizationUnitCode { get; set; }

        [Required]
        public string OrganizationUnitName { get; set; }

        public int? ParentOrganizationUnitId { get; set; }

        public long SrOrganizationLevel { get; set; }

        public int LevelDepth { get; set; }

        public string? HierarchyPath { get; set; }

public string? ParentOrganizationUnitName { get; set; }


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
