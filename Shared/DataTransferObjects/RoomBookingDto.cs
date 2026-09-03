using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record RoomBookingDto
    {
        public long RoomBookingId { get; set; }
        public Guid RoomBookingGuid { get; init; }
        public long BookedByUserId { get; set; }
        public string RoomBookingCode { get; set; } = string.Empty;
        public long MeetingRoomId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Note { get; set; }
public string? MeetingRoomName { get; set; }
public string? BookedbyUserName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record RoomBookingForCreationDto
    {
        public long BookedByUserId { get; set; }
        public string RoomBookingCode { get; set; } = string.Empty;
        public long MeetingRoomId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record RoomBookingForUpdateDto
    {
        public Guid RoomBookingGuid { get; set; }
        public long BookedByUserId { get; set; }
        public string RoomBookingCode { get; set; } = string.Empty;
        public long MeetingRoomId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record RoomBookingForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class RoomBookingSearchDto
    {
        public string? RoomBookingCode { get; set; }
        public SearchType RoomBookingCodeSearchType { get; set; } = SearchType.Contains;

        public string? Subject { get; set; }
        public SearchType SubjectSearchType { get; set; } = SearchType.Contains;

        public string? StartDateTime { get; set; }
        public SearchType StartDateTimeSearchType { get; set; } = SearchType.Contains;

        public string? EndDateTime { get; set; }
        public SearchType EndDateTimeSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;
    }
}
