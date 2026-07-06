using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IUserCompanyScopeRepository
    {
        Task<UserCompanyScope?> GetAsync(Guid userCompanyScopeGuid, bool trackChanges);
        Task<IEnumerable<UserCompanyScope>> GetAllAsync(bool trackChanges);
        Task<IEnumerable<UserCompanyScope>> GetAllByUserIdAsync(long userId);
        Task<UserCompanyScope?> GetByUserAndGuidAsync(long userId, Guid userCompanyScopeGuid);
        Task CreateAsync(UserCompanyScope entity, IDbTransaction? transaction = null);
        Task UpdateAsync(UserCompanyScope entity, IDbTransaction? transaction = null);
        Task SoftDeleteAsync(UserCompanyScope entity, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteAsync(Guid userCompanyScopeGuid, IDbTransaction? transaction = null);
    }
}
