using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record PostalCodeDto
    {
        public long PostalCodeId { get; set; }
        public Guid PostalCodeGuid { get; init; }
        public long SubDistrictId { get; set; }
        public string PostalCodeValue { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record PostalCodeForCreationDto
    {
        public string PostalCodeValue { get; set; } = string.Empty;
        public long CreatedById { get; set; }
    }

    public record PostalCodeForUpdateDto
    {
        public string PostalCodeValue { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public record PostalCodeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class PostalCodeSearchDto
    {
        public string? PostalCodeValue { get; set; }
        public SearchType PostalCodeValueSearchType { get; set; } = SearchType.Contains;
    }
}
