using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record DistrictDto
    {
        public long DistrictId { get; set; }
        public Guid DistrictGuid { get; init; }
        public string DistrictName { get; set; } = string.Empty;
        public long CityId { get; set; }
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

    public record DistrictForCreationDto
    {
        public string DistrictName { get; set; } = string.Empty;
        public long CreatedById { get; set; }
    }

    public record DistrictForUpdateDto
    {
        public string DistrictName { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public record DistrictForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class DistrictSearchDto
    {
        public string? DistrictName { get; set; }
        public SearchType DistrictNameSearchType { get; set; } = SearchType.Contains;
    }
}
