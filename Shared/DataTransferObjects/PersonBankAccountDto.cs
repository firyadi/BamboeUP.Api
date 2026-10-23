using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonBankAccountDto
    {
        public long PersonBankAccountId { get; set; }
        public Guid PersonBankAccountGuid { get; init; }
        public long PersonId { get; set; }
        public string? BankBranch { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? SwiftCode { get; set; }
        public string? IBAN { get; set; }
        public long? CurrencyId { get; set; }
        public long SrBankAccountType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPayrollAccount { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
public string? PersonName { get; set; }
public string? BankName { get; set; }
public string? BankAccountTypeName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PersonBankAccountForCreationDto
    {
        public long PersonId { get; set; }
        public string? BankBranch { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? SwiftCode { get; set; }
        public string? IBAN { get; set; }
        public long? CurrencyId { get; set; }
        public long SrBankAccountType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPayrollAccount { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonBankAccountForUpdateDto
    {
        public Guid PersonBankAccountGuid { get; set; }
        public long PersonId { get; set; }
        public string? BankBranch { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string? SwiftCode { get; set; }
        public string? IBAN { get; set; }
        public long? CurrencyId { get; set; }
        public long SrBankAccountType { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPayrollAccount { get; set; }
        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PersonBankAccountForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonBankAccountSearchDto
    {
        public string? BankBranch { get; set; }
        public SearchType BankBranchSearchType { get; set; } = SearchType.Contains;

        public string? AccountNumber { get; set; }
        public SearchType AccountNumberSearchType { get; set; } = SearchType.Contains;

        public string? AccountName { get; set; }
        public SearchType AccountNameSearchType { get; set; } = SearchType.Contains;

        public string? SwiftCode { get; set; }
        public SearchType SwiftCodeSearchType { get; set; } = SearchType.Contains;

        public string? IBAN { get; set; }
        public SearchType IBANSearchType { get; set; } = SearchType.Contains;

        public string? CurrencyId { get; set; }
        public SearchType CurrencyIdSearchType { get; set; } = SearchType.Contains;

        public string? IsPrimary { get; set; }
        public SearchType IsPrimarySearchType { get; set; } = SearchType.Contains;

        public string? IsPayrollAccount { get; set; }
        public SearchType IsPayrollAccountSearchType { get; set; } = SearchType.Contains;

        public string? IsActive { get; set; }
        public SearchType IsActiveSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
