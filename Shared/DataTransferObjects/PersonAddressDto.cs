using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record PersonAddressDto
    {
        public long PersonAddressId { get; set; }
        public Guid PersonAddressGuid { get; init; }
        public long PersonId { get; set; }
        public long SrAddressType { get; set; }
        public string Address { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
public string? CityName { get; set; }
public string? CountryName { get; set; }
public string? PersonName { get; set; }
public string? ProvinceName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record PersonAddressForCreationDto
    {
        public long PersonId { get; set; }
        public long SrAddressType { get; set; }
        public string Address { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record PersonAddressForUpdateDto
    {
        public Guid PersonAddressGuid { get; set; }
        public long PersonId { get; set; }
        public long SrAddressType { get; set; }
        public string Address { get; set; } = string.Empty;
        public long CountryId { get; set; }
        public long ProvinceId { get; set; }
        public long CityId { get; set; }
        public long UpdatedById { get; set; }
    }

    public record PersonAddressForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class PersonAddressSearchDto
    {
        public string? SrAddressType { get; set; }
        public SearchType SrAddressTypeSearchType { get; set; } = SearchType.Contains;

        public string? Address { get; set; }
        public SearchType AddressSearchType { get; set; } = SearchType.Contains;

        public string? CountryId { get; set; }
        public SearchType CountryIdSearchType { get; set; } = SearchType.Contains;

        public string? ProvinceId { get; set; }
        public SearchType ProvinceIdSearchType { get; set; } = SearchType.Contains;

        public string? CityId { get; set; }
        public SearchType CityIdSearchType { get; set; } = SearchType.Contains;
    }
}
