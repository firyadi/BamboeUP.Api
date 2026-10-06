using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IWorkNoticeService
    {
        Task<IEnumerable<WorkNoticeDto>> GetAllWorkNoticesAsync(bool trackChanges);
        Task<WorkNoticeDto?> GetWorkNoticeByGuidAsync(Guid workNoticeGuid, bool trackChanges);
        Task<WorkNoticeDto> CreateWorkNoticeAsync(WorkNoticeForCreationDto input);
        Task UpdateWorkNoticeAsync(Guid workNoticeGuid, WorkNoticeForUpdateDto input, bool trackChanges);
        Task DeleteWorkNoticeAsync(Guid workNoticeGuid, WorkNoticeForDeleteDto input, bool trackChanges);
        Task DeleteWorkNoticeByAdminAsync(Guid workNoticeGuid, bool trackChanges);

        Task<IEnumerable<WorkNoticeDto>> SearchWorkNoticeAsync(
            string? workNoticeCode, string? workNoticeCodeSearchType, string? workNoticeName, string? workNoticeNameSearchType, string? workNoticeDate, string? workNoticeDateSearchType, string? isEditAble, string? isEditAbleSearchType, string? note, string? noteSearchType

        );
    }
}
