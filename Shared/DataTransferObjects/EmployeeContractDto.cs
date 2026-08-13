using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeContractDto
    {
        public long EmployeeContractId { get; set; }
        public Guid EmployeeContractGuid { get; init; }
        public long EmployeeId { get; set; }
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
        public Guid? FileStorageGuid { get; set; }
public string? EmployeeAssignmentName { get; set; }
public string? ContractStatusName { get; set; }
public string? ContractTypeName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeContractForCreationDto
    {
        public long EmployeeId { get; set; }
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
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeContractForUpdateDto
    {
        public Guid EmployeeContractGuid { get; set; }
        public long EmployeeId { get; set; }
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
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeContractForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeContractSearchDto
    {
        public string? ContractNo { get; set; }
        public SearchType ContractNoSearchType { get; set; } = SearchType.Contains;

        public string? ContractStartDate { get; set; }
        public SearchType ContractStartDateSearchType { get; set; } = SearchType.Contains;

        public string? ContractEndDate { get; set; }
        public SearchType ContractEndDateSearchType { get; set; } = SearchType.Contains;

        public string? EffectiveDate { get; set; }
        public SearchType EffectiveDateSearchType { get; set; } = SearchType.Contains;

        public string? RenewalSequence { get; set; }
        public SearchType RenewalSequenceSearchType { get; set; } = SearchType.Contains;

        public string? IsCurrent { get; set; }
        public SearchType IsCurrentSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
