using System;
using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial record WorkNoticeDto
    {
        public long WorkNoticeId { get; set; }
        public Guid WorkNoticeGuid { get; init; }
        
        public string WorkNoticeCode { get; set; } = string.Empty;
        public string WorkNoticeName { get; set; } = string.Empty;
        public DateTime WorkNoticeDate { get; set; }
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }


        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public partial record WorkNoticeForCreationDto
    {
        public string WorkNoticeCode { get; set; } = string.Empty;
        public string WorkNoticeName { get; set; } = string.Empty;
        public DateTime WorkNoticeDate { get; set; }
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }
        public long CreatedById { get; set; } = 0;
    }

    public partial record WorkNoticeForUpdateDto
    {
        public string WorkNoticeCode { get; set; } = string.Empty;
        public string WorkNoticeName { get; set; } = string.Empty;
        public DateTime WorkNoticeDate { get; set; }
        public bool IsEditAble { get; set; }
        public string? Note { get; set; }
        public long UpdatedById { get; set; }
    }

    public partial record WorkNoticeForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public partial class WorkNoticeSearchDto
    {
        public string? WorkNoticeCode { get; set; }
        public SearchType WorkNoticeCodeSearchType { get; set; } = SearchType.Contains;

        public string? WorkNoticeName { get; set; }
        public SearchType WorkNoticeNameSearchType { get; set; } = SearchType.Contains;

        public string? WorkNoticeDate { get; set; }
        public SearchType WorkNoticeDateSearchType { get; set; } = SearchType.Contains;

        public string? IsEditAble { get; set; }
        public SearchType IsEditAbleSearchType { get; set; } = SearchType.Contains;

        public string? Note { get; set; }
        public SearchType NoteSearchType { get; set; } = SearchType.Contains;

    }
}
