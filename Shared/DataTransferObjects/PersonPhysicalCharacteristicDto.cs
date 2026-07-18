using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonPhysicalCharacteristicDto
    {
        public long PersonPhysicalCharacteristicId { get; set; }
        public Guid PersonPhysicalCharacteristicGuid { get; init; }
        public long PersonId { get; set; }
        public long SrPhysicalCharacteristic { get; set; }
        public string PhysicalValue { get; set; } = string.Empty;
        public long? SrMeasurementUnit { get; set; }
        public DateTime RecordedDate { get; set; }
        public string? Remarks { get; set; }
public string? PersonName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PersonPhysicalCharacteristicForCreationDto
    {
        public long PersonId { get; set; }
        public long SrPhysicalCharacteristic { get; set; }
        public string PhysicalValue { get; set; } = string.Empty;
        public long? SrMeasurementUnit { get; set; }
        public DateTime RecordedDate { get; set; }
        public string? Remarks { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonPhysicalCharacteristicForUpdateDto
    {
        public Guid PersonPhysicalCharacteristicGuid { get; set; }
        public long PersonId { get; set; }
        public long SrPhysicalCharacteristic { get; set; }
        public string PhysicalValue { get; set; } = string.Empty;
        public long? SrMeasurementUnit { get; set; }
        public DateTime RecordedDate { get; set; }
        public string? Remarks { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record PersonPhysicalCharacteristicForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonPhysicalCharacteristicSearchDto
    {
        public string? SrPhysicalCharacteristic { get; set; }
        public SearchType SrPhysicalCharacteristicSearchType { get; set; } = SearchType.Contains;

        public string? PhysicalValue { get; set; }
        public SearchType PhysicalValueSearchType { get; set; } = SearchType.Contains;

        public string? SrMeasurementUnit { get; set; }
        public SearchType SrMeasurementUnitSearchType { get; set; } = SearchType.Contains;

        public string? RecordedDate { get; set; }
        public SearchType RecordedDateSearchType { get; set; } = SearchType.Contains;

        public string? Remarks { get; set; }
        public SearchType RemarksSearchType { get; set; } = SearchType.Contains;
    }
}
