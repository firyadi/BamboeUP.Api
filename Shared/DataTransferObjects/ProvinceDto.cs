using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record ProvinceDto
    {
        public long ProvinceId { get; set; }
        public Guid ProvinceGuid { get; init; }
        public string ProvinceName { get; set; } = string.Empty;
        public long CountryId { get; set; }
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

    public record ProvinceForCreationDto
    {
        public string ProvinceName { get; set; } = string.Empty;
        public long CreatedById { get; set; }
    }

    public record ProvinceForUpdateDto
    {
        public string ProvinceName { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public record ProvinceForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class ProvinceSearchDto
    {
        public string? ProvinceName { get; set; }
        public SearchType ProvinceNameSearchType { get; set; } = SearchType.Contains;
    }
}
