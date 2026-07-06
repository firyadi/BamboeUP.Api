using Shared.Settings.Enums;
using System;

namespace Shared.DataTransferObjects
{
    public record ProgramDto
    {
        public long ProgramId { get; set; }
        public Guid ProgramGuid { get; init; }
        public string ProgramCode { get; set; }
        public long? ParentId { get; set; }
        public string ParentProgramName { get; set; }
        public string? IconCode { get; set; }
        public string ProgramName { get; set; }
        public long? TopLevelProgramId { get; set; }
        public byte RootLevel { get; set; }
        public byte RowIndex { get; set; }
        public string? Note { get; set; }
        public bool? IsParentProgram { get; set; }
        public bool? IsProgram { get; set; }
        public bool? IsBeginGroup { get; set; }
        public string? ProgramType { get; set; }
        public bool? IsProgramAddAble { get; set; }
        public bool? IsProgramEditAble { get; set; }
        public bool? IsProgramDeleteAble { get; set; }
        public bool? IsProgramViewAble { get; set; }
        public bool? IsProgramApprovalAble { get; set; }
        public bool? IsProgramUnApprovalAble { get; set; }
        public bool? IsProgramVoidAble { get; set; }
        public bool? IsProgramUnVoidAble { get; set; }
        public bool? IsProgramDirectVoid { get; set; }
        public bool? IsProgramPrintAble { get; set; }
        public bool? IsMenuAddVisible { get; set; }
        public bool? IsMenuHomeVisible { get; set; }
        public bool? IsVisible { get; set; }
        public string? NavigateUrl { get; set; }
        public string? HelpLinkId { get; set; }
        public string? AssemblyName { get; set; }
        public string? AssemblyClassName { get; set; }
        public string? StoreProcedureName { get; set; }
        public string? AccessKey { get; set; }
        public bool? IsActive { get; set; }
        public int StatusId { get; set; }
        public byte[]? RowVersion { get; set; }
        public long CreatedById { get; set; }
        public DateTime CreatedTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public long? DeletedById { get; set; }
        public DateTime? DeletedTime { get; set; }
    }

    public record ProgramForCreationDto
    {
        public string ProgramCode { get; set; }
        public long? ParentId { get; set; }
        public string? IconCode { get; set; }
        public string ProgramName { get; set; }
        public long? TopLevelProgramId { get; set; }
        public byte RootLevel { get; set; }
        public byte RowIndex { get; set; }
        public string? Note { get; set; }
        public bool? IsParentProgram { get; set; }
        public bool? IsProgram { get; set; }
        public bool? IsBeginGroup { get; set; }
        public string? ProgramType { get; set; }
        public bool? IsProgramAddAble { get; set; }
        public bool? IsProgramEditAble { get; set; }
        public bool? IsProgramDeleteAble { get; set; }
        public bool? IsProgramViewAble { get; set; }
        public bool? IsProgramApprovalAble { get; set; }
        public bool? IsProgramUnApprovalAble { get; set; }
        public bool? IsProgramVoidAble { get; set; }
        public bool? IsProgramUnVoidAble { get; set; }
        public bool? IsProgramDirectVoid { get; set; }
        public bool? IsProgramPrintAble { get; set; }
        public bool? IsMenuAddVisible { get; set; }
        public bool? IsMenuHomeVisible { get; set; }
        public bool? IsVisible { get; set; }
        public string? NavigateUrl { get; set; }
        public string? HelpLinkId { get; set; }
        public string? AssemblyName { get; set; }
        public string? AssemblyClassName { get; set; }
        public string? StoreProcedureName { get; set; }
        public string? AccessKey { get; set; }
        public bool? IsActive { get; set; }
        public long CreatedById { get; set; }
    }

    public record ProgramForUpdateDto
    {
        public string ProgramCode { get; set; }
        public long? ParentId { get; set; }
        public string? IconCode { get; set; }
        public string ProgramName { get; set; }
        public long? TopLevelProgramId { get; set; }
        public byte RootLevel { get; set; }
        public byte RowIndex { get; set; }
        public string? Note { get; set; }
        public bool? IsParentProgram { get; set; }
        public bool? IsProgram { get; set; }
        public bool? IsBeginGroup { get; set; }
        public string? ProgramType { get; set; }
        public bool? IsProgramAddAble { get; set; }
        public bool? IsProgramEditAble { get; set; }
        public bool? IsProgramDeleteAble { get; set; }
        public bool? IsProgramViewAble { get; set; }
        public bool? IsProgramApprovalAble { get; set; }
        public bool? IsProgramUnApprovalAble { get; set; }
        public bool? IsProgramVoidAble { get; set; }
        public bool? IsProgramUnVoidAble { get; set; }
        public bool? IsProgramDirectVoid { get; set; }
        public bool? IsProgramPrintAble { get; set; }
        public bool? IsMenuAddVisible { get; set; }
        public bool? IsMenuHomeVisible { get; set; }
        public bool? IsVisible { get; set; }
        public string? NavigateUrl { get; set; }
        public string? HelpLinkId { get; set; }
        public string? AssemblyName { get; set; }
        public string? AssemblyClassName { get; set; }
        public string? StoreProcedureName { get; set; }
        public string? AccessKey { get; set; }
        public bool? IsActive { get; set; }
        public long UpdatedById { get; set; }
    }

    public record ProgramForDeleteDto
    {
        public long DeletedById { get; set; }
    }

    public class ProgramSearchDto
    {
        public string? ProgramName { get; set; }
        public SearchType ProgramNameSearchType { get; set; } = SearchType.Contains;

        public string? ProgramCode { get; set; }
        public SearchType ProgramCodeSearchType { get; set; } = SearchType.Contains;
    }
}
