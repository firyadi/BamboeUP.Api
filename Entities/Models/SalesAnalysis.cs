using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("SalesAnalysis", Schema = "app")]
    public partial class SalesAnalysis
    {
        [Column("SalesAnalysisId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SalesAnalysisId { get; set; }

        [Key]
        [Column("SalesAnalysisGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SalesAnalysisGuid { get; set; } = Guid.NewGuid();

        public DateTime SalesDate { get; set; }

        [Required]
        public string Salesperson { get; set; } = string.Empty;

        [Required]
        public string Area { get; set; } = string.Empty;

        [Required]
        public string ProductType { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal SalesAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal NetSales { get; set; }

        public decimal CostAmount { get; set; }

        public decimal ProfitAmount { get; set; }

        public string? SalesAnalysisDescription { get; set; }



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
