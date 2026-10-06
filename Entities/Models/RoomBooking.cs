using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("RoomBooking", Schema = "app")]
    public partial class RoomBooking
    {
        [Column("RoomBookingId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RoomBookingId { get; set; }

        [Key]
        [Column("RoomBookingGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RoomBookingGuid { get; set; } = Guid.NewGuid();

        [Required]
        public long BookedByUserId { get; set; }

        [Required]
        public string RoomBookingCode { get; set; } = string.Empty;

        public long MeetingRoomId { get; set; }

        [Required]
        public string Subject { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string? Note { get; set; }

public string? MeetingRoomName { get; set; }
        public string? BookedbyUserName { get; set; }


        [Column("StatusId")]
        public long StatusId { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [Column("CreatedById")]
        public long CreatedById { get; set; } = 0;

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("UpdatedById")]
        public long UpdatedById { get; set; } = 0;

        public DateTime? UpdatedTime { get; set; }

        [Column("DeletedById")]
        public long DeletedById { get; set; } = 0;

        public DateTime? DeletedTime { get; set; }
    }
}
