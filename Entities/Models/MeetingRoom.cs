using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    [Table("MeetingRoom", Schema = "app")]
    public partial class MeetingRoom
    {
        [Column("MeetingRoomId")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MeetingRoomId { get; set; }

        [Key]
        [Column("MeetingRoomGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MeetingRoomGuid { get; set; } = Guid.NewGuid();

        [Required]
        public string RoomCode { get; set; } = string.Empty;

        [Required]
        public string RoomName { get; set; } = string.Empty;

        public int? Capacity { get; set; }

        public string? Location { get; set; }

        public string? Note { get; set; }



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
