using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Bank", Schema = "core")]
    public class Bank
    {
        [Column("BankId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BankId { get; set; }

        [Key]
        [Column("BankGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BankGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string BankName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string BankInitial { get; set; } = string.Empty;

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
