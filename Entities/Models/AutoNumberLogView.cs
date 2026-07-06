using System;

namespace Entities.Models
{
    public class AutoNumberLogView : AutoNumberLog
    {
        public string TemplateName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyOfficeName { get; set; }
        public string OrganizationUnitName { get; set; }
        public string CreatedByName { get; set; }
    }
}
