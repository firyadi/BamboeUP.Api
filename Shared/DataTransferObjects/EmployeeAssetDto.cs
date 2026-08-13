using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeAssetDto
    {
        public long EmployeeAssetId { get; set; }
        public Guid EmployeeAssetGuid { get; init; }
        public long EmployeeId { get; set; }
        public long AssetId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? ConditionWhenAssigned { get; set; }
        public string? ConditionWhenReturned { get; set; }
        public string? Remarks { get; set; }
public string? AssetName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record EmployeeAssetForCreationDto
    {
        public long EmployeeId { get; set; }
        public long AssetId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? ConditionWhenAssigned { get; set; }
        public string? ConditionWhenReturned { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record EmployeeAssetForUpdateDto
    {
        public Guid EmployeeAssetGuid { get; set; }
        public long EmployeeId { get; set; }
        public long AssetId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? ConditionWhenAssigned { get; set; }
        public string? ConditionWhenReturned { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record EmployeeAssetForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class EmployeeAssetSearchDto
    {
        public string? AssignedDate { get; set; }
        public SearchType AssignedDateSearchType { get; set; } = SearchType.Contains;

        public string? ReturnedDate { get; set; }
        public SearchType ReturnedDateSearchType { get; set; } = SearchType.Contains;

        public string? ExpectedReturnDate { get; set; }
        public SearchType ExpectedReturnDateSearchType { get; set; } = SearchType.Contains;

        public string? IsCurrent { get; set; }
        public SearchType IsCurrentSearchType { get; set; } = SearchType.Contains;

        public string? ConditionWhenAssigned { get; set; }
        public SearchType ConditionWhenAssignedSearchType { get; set; } = SearchType.Contains;

        public string? ConditionWhenReturned { get; set; }
        public SearchType ConditionWhenReturnedSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
