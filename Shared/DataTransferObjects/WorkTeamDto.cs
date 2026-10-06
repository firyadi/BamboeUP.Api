using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record WorkTeamDto
    {
        public long WorkTeamId { get; set; }
        public Guid WorkTeamGuid { get; init; }
        public string WorkTeamCode { get; set; } = string.Empty;
        public string WorkTeamName { get; set; } = string.Empty;
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }


        // ##HeaderDetailCollections##
        public IEnumerable<WorkTeamCapabilityDto>? WorkTeamCapabilities { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record WorkTeamForCreationDto
    {
        public string WorkTeamCode { get; set; } = string.Empty;
        public string WorkTeamName { get; set; } = string.Empty;
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }
        // ##HeaderDetailCreateCollections##
        public IEnumerable<WorkTeamCapabilityForCreationDto>? WorkTeamCapabilities { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record WorkTeamForUpdateDto
    {
        public string WorkTeamCode { get; set; } = string.Empty;
        public string WorkTeamName { get; set; } = string.Empty;
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }
        // ##HeaderDetailUpdateCollections##
        public IEnumerable<WorkTeamCapabilityForUpdateDto>? WorkTeamCapabilities { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record WorkTeamForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class WorkTeamSearchDto
    {
        public string? WorkTeamCode { get; set; }
        public SearchType WorkTeamCodeSearchType { get; set; } = SearchType.Contains;

        public string? WorkTeamName { get; set; }
        public SearchType WorkTeamNameSearchType { get; set; } = SearchType.Contains;

        public string? IsEditAble { get; set; }
        public SearchType IsEditAbleSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;

    }
}
