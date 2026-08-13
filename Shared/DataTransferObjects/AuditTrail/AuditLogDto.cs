using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects.AuditTrail
{
    public class AuditLogDto
    {
        public long AuditLogId { get; set; }
        public long AuditSessionId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string EntityKey { get; set; } = string.Empty;
        public string? EntityDisplayName { get; set; }
        public string? ParentTableName { get; set; }
        public string? ParentEntityKey { get; set; }
        public string AuditActionType { get; set; } = string.Empty;
        public string ActionByUserID { get; set; } = string.Empty;
        public DateTime LogDateTime { get; set; }
        public long? SnapshotId { get; set; }
        public List<AuditLogChangeDto> Changes { get; set; } = new();

        // Compatibility mapping for older code
        public string PrimaryKeyData
        {
            get => EntityKey;
            set => EntityKey = value;
        }
    }
}
