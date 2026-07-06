using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record SubDistrictDto
    {
        public long SubDistrictId { get; set; }
        public Guid SubDistrictGuid { get; init; }
        public string SubDistrictName { get; set; }
        public long DistrictId { get; set; }
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

    public record SubDistrictForCreationDto
    {
        public string SubDistrictName { get; set; }
        public long CreatedById { get; set; }
    }

    public record SubDistrictForUpdateDto
    {
        public string SubDistrictName { get; set; }
        public long UpdatedById { get; set; }
    }

    public record SubDistrictForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class SubDistrictSearchDto
    {
        public string? SubDistrictName { get; set; }
        public SearchType SubDistrictNameSearchType { get; set; } = SearchType.Contains;
    }
}
