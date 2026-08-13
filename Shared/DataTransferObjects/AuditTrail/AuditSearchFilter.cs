using System;

namespace Shared.DataTransferObjects.AuditTrail
{
    public class AuditSearchFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TableName { get; set; }
        public string? Action { get; set; } // CREATE, UPDATE, DELETE, APPROVE, VOID
        public string? Keyword { get; set; }
        public bool? DeletedOnly { get; set; }
        public long? AuditSessionId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
