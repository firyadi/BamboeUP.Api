using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("Company", Schema = "app")]
    public partial class Company
    {
        [Column("CompanyId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CompanyId { get; set; }

        [Key]
        [Column("CompanyGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CompanyGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long LeaderId { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public string? InitialName { get; set; }

        public string? TaxCompulsionNo { get; set; }

        public string? RegistrationNo { get; set; }

        public long? ParentCompanyId { get; set; }

        public string? DefaultCurrency { get; set; }

        // IMPORTANT: app.Company.CompanyLogo is varbinary(max). Keep this as
        // byte[] so Dapper and report branding can consume binary image data.
        // Public DTOs expose Base64 string through Mapster configuration.
        public byte[]? CompanyLogo { get; set; }

public string? ParentCompanyName { get; set; }


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
