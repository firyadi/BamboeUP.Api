using System;

namespace Entities.Models
{
    public class AutoNumberLogView : AutoNumberLog
    {
        public string TemplateName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyOfficeName { get; set; } = string.Empty;
        public string OrganizationUnitName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
    }
}
