using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record CountryDto
    {
        public long CountryId { get; set; }
        public Guid CountryGuid { get; init; }
        public string CountryIso { get; set; }
        public string CountryName { get; set; }
        public string? CountryIso3 { get; set; }
        public int PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record CountryForCreationDto
    {
        public string CountryIso { get; set; }
        public string CountryName { get; set; }
        public string? CountryIso3 { get; set; }
        public int PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public record CountryForUpdateDto
    {
        public string CountryIso { get; set; }
        public string CountryName { get; set; }
        public string? CountryIso3 { get; set; }
        public int PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public long UpdatedById { get; set; }
    }

    public record CountryForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class CountrySearchDto
    {
        public string? CountryName { get; set; }
        public SearchType CountryNameSearchType { get; set; } = SearchType.Contains;

        public string? CountryIso { get; set; }
        public SearchType CountryIsoSearchType { get; set; } = SearchType.Contains;
    }
}
