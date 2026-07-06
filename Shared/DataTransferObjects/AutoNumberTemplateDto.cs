using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record AutoNumberTemplateDto
    {
        public long AutoNumberTemplateId { get; set; }
        public Guid AutoNumberTemplateGuid { get; init; }
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }

        // Scope
        public string TemplateScopeType { get; set; }
        public long SrFormMappingNumbering { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }

        // Audit
        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record AutoNumberTemplateForCreationDto
    {
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }

        public string TemplateScopeType { get; set; } = "GLOBAL";
        public long SrFormMappingNumbering { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }

        public long CreatedById { get; set; } = 0;
    }

    public record AutoNumberTemplateForUpdateDto
    {
        public string TemplateName { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }

        public string TemplateScopeType { get; set; }
        public long SrFormMappingNumbering { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }

        public long? UpdatedById { get; set; }
    }

    public record AutoNumberTemplateForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class AutoNumberTemplateSearchDto
    {
        public string? TemplateName { get; set; }
        public SearchType TemplateNameSearchType { get; set; } = SearchType.Contains;

        public string? Description { get; set; }
        public SearchType DescriptionSearchType { get; set; } = SearchType.Contains;

        public DateTime? EffectiveDateFrom { get; set; }
        public DateTime? EffectiveDateTo { get; set; }

        public string? TemplateScopeType { get; set; }
        public long? CompanyId { get; set; }
        public long? CompanyOfficeId { get; set; }
    }
}
