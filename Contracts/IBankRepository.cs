using Entities.Models;
using System.Data;

namespace Contracts
{
    public partial interface IBankRepository
    {
        Task<Bank> GetBankAsync(Guid bankGuid, bool trackChanges);
        Task<IEnumerable<Bank>> GetAllBanksAsync(bool trackChanges);

        Task CreateBankAsync(Bank bank, IDbTransaction? transaction = null);
        Task UpdateBankAsync(Bank bank, IDbTransaction? transaction = null);
        Task DeleteBankAsync(Guid bankGuid, IDbTransaction? transaction = null);
        Task SoftDeleteBankAsync(Bank bank, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Bank>> SearchBankAsync(
            string? bankName, string? bankNameSearchType,
string? bankInitial, string? bankInitialSearchType,
IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        
    }
}
