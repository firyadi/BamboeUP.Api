using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("UserGroupScope", Schema = "core")]
    public class UserGroupScope
    {
        [Column("UserGroupScopeId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserGroupScopeId { get; set; }

        [Key]
        [Column("UserGroupScopeGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserGroupScopeGuid { get; set; } = Guid.NewGuid();

        [Required]
        [Column("UserId")]
        public long UserId { get; set; }

        [Required]
        [Column("UserGuid")]
        public Guid UserGuid { get; set; }

        [Required]
        [Column("UserGroupId")]
        public long UserGroupId { get; set; }

        [Required]
        [Column("UserGroupGuid")]
        public Guid UserGroupGuid { get; set; }

        [Required]
        [Column("CompanyId")]
        public long CompanyId { get; set; }

        [Required]
        [Column("CompanyGuid")]
        public Guid CompanyGuid { get; set; }

        /// <summary>
        /// NULL = role berlaku untuk semua office di company.
        /// NOT NULL = role hanya berlaku untuk 1 office spesifik.
        /// </summary>
        [Column("CompanyOfficeId")]
        public long? CompanyOfficeId { get; set; }

        [Column("CompanyOfficeGuid")]
        public Guid? CompanyOfficeGuid { get; set; }

        /// <summary>
        /// Menandakan scope ini adalah scope default/utama user saat login.
        /// Hanya boleh ada 1 IsDefault = true per UserId.
        /// </summary>
        [Column("IsDefault")]
        public bool IsDefault { get; set; } = false;

        [Column("StatusId")]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; }

        [Column("CreatedTime")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long? UpdatedById { get; set; }

        [Column("UpdatedTime")]
        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long? DeletedById { get; set; }

        [Column("DeletedTime")]
        public DateTime? DeletedTime { get; set; }

        // ─── Joined / computed (tidak disimpan ke DB) ───────────────────────
        [NotMapped]
        public string? FullName { get; set; }          // joined from core.Users

        [NotMapped]
        public string? UserName { get; set; }          // joined from core.Users

        [NotMapped]
        public string? UserGroupName { get; set; }     // joined from core.UserGroup

        [NotMapped]
        public string? CompanyName { get; set; }       // joined from app.Company

        [NotMapped]
        public string? CompanyOfficeName { get; set; } // joined from app.CompanyOffice
    }
}
