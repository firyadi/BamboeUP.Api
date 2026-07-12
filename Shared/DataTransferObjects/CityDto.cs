using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record CityDto
    {
        public long CityId { get; set; }
        public Guid CityGuid { get; init; }
        public string CityName { get; set; } = string.Empty;
        public long ProvinceId { get; set; }
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

    public record CityForCreationDto
    {
        public string CityName { get; set; } = string.Empty;
        public long CreatedById { get; set; }
    }

    public record CityForUpdateDto
    {
        public string CityName { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public record CityForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class CitySearchDto
    {
        public string? CityName { get; set; }
        public SearchType CityNameSearchType { get; set; } = SearchType.Contains;
    }
}
