using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IWorkNoticeRepository
    {
        Task<WorkNotice?> GetWorkNoticeAsync(Guid workNoticeGuid, bool trackChanges);
        Task<IEnumerable<WorkNotice>> GetAllWorkNoticesAsync(bool trackChanges);

        Task CreateWorkNoticeAsync(WorkNotice workNotice, IDbTransaction? transaction = null);
        Task UpdateWorkNoticeAsync(WorkNotice workNotice, IDbTransaction? transaction = null);
        Task DeleteWorkNoticeAsync(Guid workNoticeGuid, IDbTransaction? transaction = null);
        Task SoftDeleteWorkNoticeAsync(WorkNotice workNotice, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<WorkNotice>> SearchWorkNoticeAsync(
            string? workNoticeCode,
            string? workNoticeCodeSearchType,
            string? workNoticeName,
            string? workNoticeNameSearchType,
            string? workNoticeDate,
            string? workNoticeDateSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? note,
            string? noteSearchType,

            IDbTransaction? transaction = null);
    }
}
