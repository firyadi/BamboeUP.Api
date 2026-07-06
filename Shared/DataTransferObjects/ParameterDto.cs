using System;
using System.Collections.Generic;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public record ParameterDto
    {
        public long ParameterId { get; set; }
        public Guid ParameterGuid { get; init; }
        public string Parametername { get; set; }
        public string Parametervalue { get; set; }

        public IEnumerable<ParameterscopeDto>? Parameterscopes { get; set; }
        public int StatusId { get; set; }
        public byte[] RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record ParameterForCreationDto
    {
        public string Parametername { get; set; }
        public string Parametervalue { get; set; }

        public IEnumerable<ParameterscopeForCreationDto>? Parameterscopes { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public record ParameterForUpdateDto
    {
        public string Parametername { get; set; }
        public string Parametervalue { get; set; }

        public IEnumerable<ParameterscopeForUpdateDto>? Parameterscopes { get; set; }
        public long UpdatedById { get; set; }
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
        

    }
}
