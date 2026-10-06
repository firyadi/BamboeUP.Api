using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("PersonBankAccount", Schema = "app")]
    public partial class PersonBankAccount
    {
        [Column("PersonBankAccountId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PersonBankAccountId { get; set; }

        [Key]
        [Column("PersonBankAccountGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PersonBankAccountGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long PersonId { get; set; }

        public string? BankBranch { get; set; }

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public string AccountName { get; set; } = string.Empty;

        public string? SwiftCode { get; set; }

        public string? IBAN { get; set; }

        public long? CurrencyId { get; set; }

        public long SrBankAccountType { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsPayrollAccount { get; set; }

        public bool IsActive { get; set; }

        public string? Remarks { get; set; }

public string? BankName { get; set; }
public string? CurrencyName { get; set; }
public string? BankAccountTypeName { get; set; }


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
