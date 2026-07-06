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
        [MaxLength(20)]
        public string OrganizationUnitCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string OrganizationUnitName { get; set; }

        public long? ParentOrganizationUnitId { get; set; }

        public string? ParentOrganizationUnitName { get; set; }

        public long SrOrganizationLevel { get; set; }

        public int LevelDepth { get; set; }

        [MaxLength(500)]
        public string? HierarchyPath { get; set; }

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
