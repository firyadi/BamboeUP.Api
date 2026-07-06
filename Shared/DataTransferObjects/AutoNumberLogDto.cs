using System;

namespace Shared.DataTransferObjects
{
    /// <summary>
    /// DTO Response — hasil generate number.
    /// </summary>
    public class GenerateNumberResultDto
    {
        /// <summary>Nomor dokumen yang dihasilkan, contoh: INV/2026/05/0001</summary>
        public string GeneratedNumber { get; set; }

        /// <summary>Nilai counter saat ini setelah increment.</summary>
        public int CounterValue { get; set; }

        /// <summary>Guid log record untuk referensi/audit.</summary>
        public Guid AutoNumberLogGuid { get; set; }

        /// <summary>Id log record untuk referensi/audit.</summary>
        public long AutoNumberLogId { get; set; }
    }

    /// <summary>
    /// DTO untuk query AutoNumberLog.
    /// </summary>
    public class AutoNumberLogDto
    {
        public long AutoNumberLogId { get; set; }
        public Guid AutoNumberLogGuid { get; set; }
        public long AutoNumberTemplateId { get; set; }
        public string GeneratedNumber { get; set; }
        public int CounterValue { get; set; }
        public string Status { get; set; }
        public string ReferenceId { get; set; }
        
        // Additional names for UI
        public string TemplateName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyOfficeName { get; set; }
        public string OrganizationUnitName { get; set; }
        public string CreatedByName { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public int? DayNo { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
