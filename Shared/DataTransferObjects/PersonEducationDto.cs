using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record PersonEducationDto
    {
        public long PersonEducationId { get; set; }
        public Guid PersonEducationGuid { get; init; }
        public long PersonId { get; set; }
        public long SrEducationLevel { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
public string? PersonName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record PersonEducationForCreationDto
    {
        public long PersonId { get; set; }
        public long SrEducationLevel { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public long CreatedById { get; set; } = 0;
    }

    public partial record PersonEducationForUpdateDto
    {
        public Guid PersonEducationGuid { get; set; }
        public long PersonId { get; set; }
        public long SrEducationLevel { get; set; }
        public string InstitutionName { get; set; } = string.Empty;
        public long UpdatedById { get; set; }
    }

    public partial record PersonEducationForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class PersonEducationSearchDto
    {
        public string? SrEducationLevel { get; set; }
        public SearchType SrEducationLevelSearchType { get; set; } = SearchType.Contains;

        public string? InstitutionName { get; set; }
        public SearchType InstitutionNameSearchType { get; set; } = SearchType.Contains;
    }
}
