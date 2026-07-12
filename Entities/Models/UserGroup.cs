using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("UserGroup", Schema = "core")]
    public class UserGroup
    {
        [Column("UserGroupId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserGroupId { get; set; }

        [Key]
        [Column("UserGroupGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserGroupGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string UserGroupName { get; set; } = string.Empty;

        public bool IsEditAble { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Required]
        public long CreatedById { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        public long? UpdatedById { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public long? DeletedById { get; set; }

        public DateTime? DeletedTime { get; set; }
    }
}
