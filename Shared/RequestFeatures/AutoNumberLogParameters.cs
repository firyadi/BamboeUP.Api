using System;

namespace Shared.RequestFeatures
{
    public class AutoNumberLogParameters
    {
        public long? TemplateId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
    }
}
