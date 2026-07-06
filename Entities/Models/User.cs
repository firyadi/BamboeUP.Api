using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Users", Schema = "core")]
    public class User
    {
        [Column("UserId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long UserId { get; set; }

        [Key]
        [Column("UserGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(128)]
        public string PasswordSalt { get; set; }

        [MaxLength(200)]
        public string FullName { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        public bool IsAdmin { get; set; }

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
