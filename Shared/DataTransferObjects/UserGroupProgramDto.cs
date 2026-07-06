using Shared.Settings.Enums;
using System;

namespace Shared.DataTransferObjects
{
    public record UserGroupProgramDto
    {
        public long UserGroupProgramId { get; set; }
        public Guid UserGroupProgramGuid { get; init; }
        public long UserGroupId { get; set; }
        public long ProgramsId { get; set; }
        public bool IsUserGroupViewAble { get; set; }
        public bool? IsUserGroupAddAble { get; set; }
        public bool? IsUserGroupEditAble { get; set; }
        public bool? IsUserGroupDeleteAble { get; set; }
        public bool? IsUserGroupApprovalAble { get; set; }
        public bool? IsUserGroupUnApprovalAble { get; set; }
        public bool? IsUserGroupVoidAble { get; set; }
        public bool? IsUserGroupUnVoidAble { get; set; }
        public bool? IsUserGroupExportAble { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record UserGroupProgramForCreationDto
    {
        public long UserGroupId { get; set; }
        public long ProgramsId { get; set; }
        public bool IsUserGroupViewAble { get; set; } = true;
        public bool? IsUserGroupAddAble { get; set; }
        public bool? IsUserGroupEditAble { get; set; }
        public bool? IsUserGroupDeleteAble { get; set; }
        public bool? IsUserGroupApprovalAble { get; set; }
        public bool? IsUserGroupUnApprovalAble { get; set; }
        public bool? IsUserGroupVoidAble { get; set; }
        public bool? IsUserGroupUnVoidAble { get; set; }
        public bool? IsUserGroupExportAble { get; set; }
        public long CreatedById { get; set; }
    }

    public record UserGroupProgramForUpdateDto
    {
        public long UserGroupId { get; set; }
        public long ProgramsId { get; set; }
        public bool IsUserGroupViewAble { get; set; } = true;
        public bool? IsUserGroupAddAble { get; set; }
        public bool? IsUserGroupEditAble { get; set; }
        public bool? IsUserGroupDeleteAble { get; set; }
        public bool? IsUserGroupApprovalAble { get; set; }
        public bool? IsUserGroupUnApprovalAble { get; set; }
        public bool? IsUserGroupVoidAble { get; set; }
        public bool? IsUserGroupUnVoidAble { get; set; }
        public bool? IsUserGroupExportAble { get; set; }
        public long? UpdatedById { get; set; }
    }

    public record UserGroupProgramForDeleteDto
    {
        public long? DeletedById { get; set; }
    }

    public class UserGroupProgramSearchDto
    {
    }
}
