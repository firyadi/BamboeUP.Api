using System;
using System.Text.Json.Serialization;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record ParameterscopeDto
    {
        public long ParameterscopeId { get; set; }

        // Use alias so Blazor App can deserialize as ParameterScopeGuid
        [JsonPropertyName("parameterScopeGuid")]
        public Guid ParameterscopeGuid { get; init; }
        public long ParameterId { get; set; }
        public Guid ParameterGuid { get; set; }
        public string? ParameterName { get; set; }
        public long? CompanyId { get; set; }
        public Guid? CompanyGuid { get; set; }
        public long? CompanyOfficeId { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyOfficeName { get; set; }

        [JsonPropertyName("overrideValue")]
        public string Overridevalue { get; set; } = string.Empty;

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }

        // Compatibility properties for Blazor App (casing alignment)
        [JsonIgnore]
        public Guid ParameterScopeGuid
        {
            get => ParameterscopeGuid;
            init => ParameterscopeGuid = value;
        }

        [JsonIgnore]
        public string OverrideValue
        {
            get => Overridevalue;
            set => Overridevalue = value;
        }
    }

    public record ParameterscopeForCreationDto
    {
        public Guid ParameterGuid { get; set; }
        public long ParameterId { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }

        [JsonPropertyName("overrideValue")]
        public string Overridevalue { get; set; } = string.Empty;

        public long CreatedById { get; set; } = 0;

        [JsonIgnore]
        public string OverrideValue
        {
            get => Overridevalue;
            set => Overridevalue = value;
        }

        public int StatusId { get; set; } = 1;
    }

    public record ParameterscopeForUpdateDto
    {
        // Accept both camelCase (parameterScopeGuid) and legacy (parameterScopeGuid from UI)
        [JsonPropertyName("parameterScopeGuid")]
        public Guid ParameterscopeGuid { get; set; }
        public Guid ParameterGuid { get; set; }
        public long ParameterId { get; set; }
        public Guid? CompanyGuid { get; set; }
        public Guid? CompanyOfficeGuid { get; set; }

        [JsonPropertyName("overrideValue")]
        public string Overridevalue { get; set; } = string.Empty;

        public long UpdatedById { get; set; }

        [JsonIgnore]
        public Guid ParameterScopeGuid
        {
            get => ParameterscopeGuid;
            set => ParameterscopeGuid = value;
        }

        [JsonIgnore]
        public string OverrideValue
        {
            get => Overridevalue;
            set => Overridevalue = value;
        }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record ParameterscopeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class ParameterscopeSearchDto
    {
        public string? Overridevalue { get; set; }
        public SearchType OverridevalueSearchType { get; set; } = SearchType.Contains;

        [JsonIgnore]
        public string? OverrideValue
        {
            get => Overridevalue;
            set => Overridevalue = value;
        }

        [JsonIgnore]
        public SearchType OverrideValueSearchType
        {
            get => OverridevalueSearchType;
            set => OverridevalueSearchType = value;
        }
    }

    // Compatibility aliases for Blazor App (with uppercase 'S')
    public record ParameterScopeDto : ParameterscopeDto;
    public record ParameterScopeForCreationDto : ParameterscopeForCreationDto;
    public record ParameterScopeForUpdateDto : ParameterscopeForUpdateDto;
    public record ParameterScopeForDeleteDto : ParameterscopeForDeleteDto;
    public class ParameterScopeSearchDto : ParameterscopeSearchDto;
}
