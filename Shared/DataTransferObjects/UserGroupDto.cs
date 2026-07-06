using Shared.Settings.Enums;
using System;

namespace Shared.DataTransferObjects
{
    public record UserGroupDto
    {
        public long UserGroupId { get; set; }
        public Guid UserGroupGuid { get; init; }
        public string UserGroupName { get; set; }
        public bool IsEditAble { get; set; }
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

    public record UserGroupForCreationDto
    {
        public string UserGroupName { get; set; }
        public bool IsEditAble { get; set; }
        public long CreatedById { get; set; }
    }

    public record UserGroupForUpdateDto
    {
        public string UserGroupName { get; set; }
        public bool IsEditAble { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record UserGroupForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class UserGroupSearchDto
    {
        public string UserGroupName { get; set; }
        public SearchType UserGroupNameSearchType { get; set; } = SearchType.Contains;
    }
}
