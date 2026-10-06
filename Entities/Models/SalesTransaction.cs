using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("SalesTransaction", Schema = "app")]
    public partial class SalesTransaction
    {
        [Column("SalesTransactionId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SalesTransactionId { get; set; }

        [Key]
        [Column("SalesTransactionGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SalesTransactionGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string TransactionCode { get; set; } = string.Empty;

        public long CompanyId { get; set; }

        public long? CompanyOfficeId { get; set; }

        public int PeriodYear { get; set; }

        public int PeriodMonth { get; set; }

        public DateTime TransactionDate { get; set; }

        [Required]
        public string RegionCode { get; set; } = string.Empty;

        [Required]
        public string ProductCategory { get; set; } = string.Empty;

        [Required]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string SalesChannel { get; set; } = string.Empty;

        [Required]
        public string CustomerSegment { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal NetAmount { get; set; }

        public decimal CostAmount { get; set; }



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
