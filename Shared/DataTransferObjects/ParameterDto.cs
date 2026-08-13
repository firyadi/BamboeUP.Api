using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record ParameterDto
    {
        public long ParameterId { get; set; }
        public Guid ParameterGuid { get; init; }
        public string Parametername { get; set; } = string.Empty;
        public string Parametervalue { get; set; } = string.Empty;

        public IEnumerable<ParameterScopeDto>? Parameterscopes { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }

        // Compatibility properties for Blazor App (casing alignment)
        public string ParameterName
        {
            get => Parametername;
            set => Parametername = value;
        }

        public string ParameterValue
        {
            get => Parametervalue;
            set => Parametervalue = value;
        }

        public string ParameterType { get; set; } = string.Empty;
        public string? StatusName { get; set; }
    }

    public record ParameterForCreationDto
    {
        public string Parametername { get; set; } = string.Empty;
        public string Parametervalue { get; set; } = string.Empty;

        public IEnumerable<ParameterScopeForCreationDto>? Parameterscopes { get; set; }
        public long CreatedById { get; set; } = 0;

        // Compatibility properties for Blazor App (casing alignment)
        public string ParameterName
        {
            get => Parametername;
            set => Parametername = value;
        }

        public string ParameterValue
        {
            get => Parametervalue;
            set => Parametervalue = value;
        }

        public string ParameterType { get; set; } = string.Empty;
        public int StatusId { get; set; } = 1;
    }

    public record ParameterForUpdateDto
    {
        public string Parametername { get; set; } = string.Empty;
        public string Parametervalue { get; set; } = string.Empty;

        public IEnumerable<ParameterScopeForUpdateDto>? Parameterscopes { get; set; }
        public long UpdatedById { get; set; }

        // Compatibility properties for Blazor App (casing alignment)
        public string ParameterName
        {
            get => Parametername;
            set => Parametername = value;
        }

        public string ParameterValue
        {
            get => Parametervalue;
            set => Parametervalue = value;
        }

        public string ParameterType { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
    }

    public record ParameterForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class ParameterSearchDto
    {
        public string? Parametername { get; set; }
        public SearchType ParameternameSearchType { get; set; } = SearchType.Contains;

        public string? Parametervalue { get; set; }
        public SearchType ParametervalueSearchType { get; set; } = SearchType.Contains;

        // Compatibility properties for Blazor App (casing alignment)
        public string? ParameterName
        {
            get => Parametername;
            set => Parametername = value;
        }

        public SearchType ParameterNameSearchType
        {
            get => ParameternameSearchType;
            set => ParameternameSearchType = value;
        }

        public string? ParameterValue
        {
            get => Parametervalue;
            set => Parametervalue = value;
        }

        public SearchType ParameterValueSearchType
        {
            get => ParametervalueSearchType;
            set => ParametervalueSearchType = value;
        }

        public string? ParameterType { get; set; }
        public SearchType ParameterTypeSearchType { get; set; } = SearchType.Contains;
    }
}
