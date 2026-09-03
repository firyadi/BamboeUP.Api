using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    /// <summary>
    /// Shell-required fields stripped by Header-Detail regenerate (TargetLayer=Modules).
    /// Keep in Custom so Shell controllers/services keep compiling.
    /// </summary>
    public partial record CompanyOfficeDto
    {
        public Guid CompanyGuid { get; set; }
    }

    public partial record CompanyOfficeForCreationDto
    {
        public Guid CompanyGuid { get; set; }
    }

    public partial record CompanyOfficeForUpdateDto
    {
        public Guid CompanyGuid { get; set; }
    }

    public partial class CompanyOfficeSearchDto
    {
        public long? SrAddressType { get; set; }
        public long? CountryId { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public string? PostalCodeId { get; set; }
        public SearchType PostalCodeIdSearchType { get; set; } = SearchType.Contains;
    }
}
