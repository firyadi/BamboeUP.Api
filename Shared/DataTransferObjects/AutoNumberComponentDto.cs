using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record AutoNumberComponentDto
    {
        public long AutoNumberComponentId { get; set; }
        public Guid AutoNumberComponentGuid { get; init; }
        public long AutoNumberTemplateId { get; set; }
        public string? AutoNumberTemplateName { get; set; }

        public short SeqNo { get; set; }
        public string ComponentType { get; set; } = string.Empty;
        public string StaticValue { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public short? CounterLength { get; set; }
        public bool IsResetKey { get; set; }

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

    public record AutoNumberComponentForCreationDto
    {
        public long AutoNumberTemplateId { get; set; }
        public short SeqNo { get; set; }
        public string ComponentType { get; set; } = string.Empty;
        public string StaticValue { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public short? CounterLength { get; set; }
        public bool IsResetKey { get; set; } = false;

        public long CreatedById { get; set; } = 0;
    }

    public record AutoNumberComponentForUpdateDto
    {
        public long AutoNumberTemplateId { get; set; }
        public short SeqNo { get; set; }
        public string ComponentType { get; set; } = string.Empty;
        public string StaticValue { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public short? CounterLength { get; set; }
        public bool IsResetKey { get; set; }

        public long? UpdatedById { get; set; }
    }

    public record AutoNumberComponentForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class AutoNumberComponentSearchDto
    {
        public string? ComponentType { get; set; }
        public SearchType ComponentTypeSearchType { get; set; } = SearchType.Contains;

        public string? StaticValue { get; set; }
        public SearchType StaticValueSearchType { get; set; } = SearchType.Contains;

        public string? Format { get; set; }
        public SearchType FormatSearchType { get; set; } = SearchType.Contains;

        public long? AutoNumberTemplateId { get; set; }
    }
}
