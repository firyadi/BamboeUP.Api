using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IUserGroupScopeRepository
    {
        Task<UserGroupScope?> GetAsync(Guid userGroupScopeGuid, bool trackChanges);
        Task<IEnumerable<UserGroupScope>> GetAllByUserGroupGuidAsync(Guid userGroupGuid, bool trackChanges);
        Task<IEnumerable<UserGroupScope>> GetAllByUserIdAsync(long userId);
        Task<IEnumerable<UserGroupScope>> GetAllByCompanyIdAsync(long companyId);
        Task<UserGroupScope?> GetByUserGroupAndGuidAsync(Guid userGroupGuid, Guid userGroupScopeGuid);
        Task CreateAsync(UserGroupScope entity, IDbTransaction? transaction = null);
        Task SoftDeleteAsync(UserGroupScope entity, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteAsync(Guid userGroupScopeGuid, IDbTransaction? transaction = null);

        /// <summary>
        /// Secara atomik dalam 1 transaksi:
        /// 1) Unset semua scope milik userId (IsDefault = 0)
        /// 2) Set scope dengan GUID ini sebagai default (IsDefault = 1)
        /// </summary>
        Task<UserGroupScope> SetDefaultAsync(Guid userGroupScopeGuid, long userId, long updatedBy, IDbTransaction? transaction = null);
    }
}
