using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record AutoNumberCounterDto
    {
        public long AutoNumberCounterId { get; set; }
        public Guid AutoNumberCounterGuid { get; init; }
        public long AutoNumberTemplateId { get; set; }
        public string? AutoNumberTemplateName { get; set; }

        // Scope
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int? OrganizationUnitId { get; set; }

        // Time Dimension
        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public int? DayNo { get; set; }

        // Counter
        public int LastNumber { get; set; }

        // Audit
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record AutoNumberCounterForCreationDto
    {
        public long AutoNumberTemplateId { get; set; }

        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int? OrganizationUnitId { get; set; }

        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public int? DayNo { get; set; }

        public int LastNumber { get; set; } = 0;

        public long CreatedById { get; set; } = 0;
    }

    public record AutoNumberCounterForUpdateDto
    {
        public int LastNumber { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record AutoNumberCounterForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class AutoNumberCounterSearchDto
    {
        public long? AutoNumberTemplateId { get; set; }

        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
        public int? OrganizationUnitId { get; set; }

        public int? YearNo { get; set; }
        public int? MonthNo { get; set; }
        public int? DayNo { get; set; }
    }
}
