using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("EmployeeContract", Schema = "app")]
    public partial class EmployeeContract
    {
        [Column("EmployeeContractId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EmployeeContractId { get; set; }

        [Key]
        [Column("EmployeeContractGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeContractGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long EmployeeId { get; set; }

        [Required]
        public string ContractNo { get; set; } = string.Empty;

        public long SrContractType { get; set; }

        public long SrContractStatus { get; set; }

        public DateTime ContractStartDate { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int RenewalSequence { get; set; }

        public bool IsCurrent { get; set; }

        public long? FileStorageId { get; set; }

        public string? Remarks { get; set; }

public string? EmployeeAssignmentName { get; set; }
public string? ContractStatusName { get; set; }
public string? ContractTypeName { get; set; }


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
