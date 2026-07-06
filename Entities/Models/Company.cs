using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Company", Schema = "app")]
    public class Company
    {
        [Column("CompanyId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CompanyId { get; set; }

        [Key]
        [Column("CompanyGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyGuid { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [MaxLength(20)]
        public string InitialName { get; set; }

        [MaxLength(30)]
        public string TaxCompulsionNo { get; set; }

        [MaxLength(30)]
        public string RegistrationNo { get; set; }

        [Column("ParentCompanyId")]
        public long ParentCompanyId { get; set; } = 0;

        [MaxLength(3)]
        public string DefaultCurrency { get; set; }

        public byte[] CompanyLogo { get; set; }

        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; }  = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
