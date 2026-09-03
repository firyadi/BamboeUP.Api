using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record MeetingRoomDto
    {
        public long MeetingRoomId { get; set; }
        public Guid MeetingRoomGuid { get; init; }
        
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record MeetingRoomForCreationDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record MeetingRoomForUpdateDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int? Capacity { get; set; }
        public string? Location { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record MeetingRoomForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class MeetingRoomSearchDto
    {
        public string? RoomCode { get; set; }
        public SearchType RoomCodeSearchType { get; set; } = SearchType.Contains;

        public string? RoomName { get; set; }
        public SearchType RoomNameSearchType { get; set; } = SearchType.Contains;

        public string? Capacity { get; set; }
        public SearchType CapacitySearchType { get; set; } = SearchType.Contains;

        public string? Location { get; set; }
        public SearchType LocationSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;

    }
}
