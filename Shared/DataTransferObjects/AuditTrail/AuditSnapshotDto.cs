using System;

namespace Shared.DataTransferObjects.AuditTrail
{
    public class AuditSnapshotDto
    {
        public long SnapshotId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string EntityKey { get; set; } = string.Empty;
        public string SnapshotJson { get; set; } = string.Empty;
        public DateTime SnapshotDateTime { get; set; }
    }
}
