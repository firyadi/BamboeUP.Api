using System;
using System.Collections.Generic;

namespace Shared.DataTransferObjects.AuditTrail
{
    public class AuditSessionDto
    {
        public long AuditSessionId { get; set; }
        public Guid SessionGuid { get; set; }
        public string SessionType { get; set; } = string.Empty; // C, U, D, A, V
        public string RootTableName { get; set; } = string.Empty;
        public string RootEntityKey { get; set; } = string.Empty;
        public string? RootDisplayName { get; set; }
        public string ActionByUserId { get; set; } = string.Empty;
        public DateTime LogDateTime { get; set; }
        public int AffectedRecords { get; set; }
        public List<AuditLogDto> Logs { get; set; } = new();
    }
}
