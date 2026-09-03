using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record CompanyOfficeDto
    {
        public long CompanyOfficeId { get; set; }
        public Guid CompanyOfficeGuid { get; init; }
        public long CompanyId { get; set; }
        public string CompanyOfficeName { get; set; } = string.Empty;
        public long SrAddressType { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public string PostalCodeId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
public string? CityName { get; set; }
public string? CompanyName { get; set; }
public string? CountryName { get; set; }
public string? PostalCodeName { get; set; }
public string? AddressTypeName { get; set; }
public string? StateName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record CompanyOfficeForCreationDto
    {
        public long CompanyId { get; set; }
        public string CompanyOfficeName { get; set; } = string.Empty;
        public long SrAddressType { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public string PostalCodeId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public long CreatedById { get; set; } = 0;
    }

    public partial record CompanyOfficeForUpdateDto
    {
        public Guid CompanyOfficeGuid { get; set; }
        public long CompanyId { get; set; }
        public string CompanyOfficeName { get; set; } = string.Empty;
        public long SrAddressType { get; set; }
        public long CountryId { get; set; }
        public long StateId { get; set; }
        public long CityId { get; set; }
        public string PostalCodeId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public partial record CompanyOfficeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class CompanyOfficeSearchDto
    {
        public string? CompanyOfficeName { get; set; }
        public SearchType CompanyOfficeNameSearchType { get; set; } = SearchType.Contains;

        public string? Address { get; set; }
        public SearchType AddressSearchType { get; set; } = SearchType.Contains;
    }
}
