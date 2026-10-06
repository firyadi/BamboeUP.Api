using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record WorkTeamCapabilityDto
    {
        public long WorkTeamCapabilityId { get; set; }
        public Guid WorkTeamCapabilityGuid { get; init; }
        public long WorkTeamId { get; set; }
        public long WorkCapabilityId { get; set; }
        public bool IsViewAble { get; set; }
        public bool? IsAddAble { get; set; }
        public bool? IsEditAble { get; set; }
        public bool? IsApproveAble { get; set; }
        public bool? IsExportAble { get; set; }
public string? WorkCapabilityName { get; set; }
public string? WorkTeamName { get; set; }

        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record WorkTeamCapabilityForCreationDto
    {
        public long WorkTeamId { get; set; }
        public long WorkCapabilityId { get; set; }
        public bool IsViewAble { get; set; }
        public bool? IsAddAble { get; set; }
        public bool? IsEditAble { get; set; }
        public bool? IsApproveAble { get; set; }
        public bool? IsExportAble { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record WorkTeamCapabilityForUpdateDto
    {
        public Guid WorkTeamCapabilityGuid { get; set; }
        public long WorkTeamId { get; set; }
        public long WorkCapabilityId { get; set; }
        public bool IsViewAble { get; set; }
        public bool? IsAddAble { get; set; }
        public bool? IsEditAble { get; set; }
        public bool? IsApproveAble { get; set; }
        public bool? IsExportAble { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record WorkTeamCapabilityForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class WorkTeamCapabilitySearchDto
    {
        public string? IsViewAble { get; set; }
        public SearchType IsViewAbleSearchType { get; set; } = SearchType.Contains;

        public string? IsAddAble { get; set; }
        public SearchType IsAddAbleSearchType { get; set; } = SearchType.Contains;

        public string? IsEditAble { get; set; }
        public SearchType IsEditAbleSearchType { get; set; } = SearchType.Contains;

        public string? IsApproveAble { get; set; }
        public SearchType IsApproveAbleSearchType { get; set; } = SearchType.Contains;

        public string? IsExportAble { get; set; }
        public SearchType IsExportAbleSearchType { get; set; } = SearchType.Contains;
    }
}
