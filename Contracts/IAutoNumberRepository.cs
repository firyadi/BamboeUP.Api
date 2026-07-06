using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IAutoNumberRepository
    {
        Task<AutoNumber> GetAutoNumberAsync(Guid autoNumberGuid, bool trackChanges);
        Task<IEnumerable<AutoNumber>> GetAllAutoNumbersAsync(bool trackChanges);

        Task<IEnumerable<AutoNumber>> SearchAutoNumberAsync(
            string? prefik,
            string? prefikSearchType,
            string? separatorAfterPrefik,
            string? separatorAfterPrefikSearchType,
            string? separatorAfterDept,
            string? separatorAfterDeptSearchType,
            string? separatorAfterYear,
            string? separatorAfterYearSearchType,
            string? separatorAfterMonth,
            string? separatorAfterMonthSearchType,
            string? separatorAfterDay,
            string? separatorAfterDaySearchType,
            string? numberGroupSeparator,
            string? numberGroupSeparatorSearchType,
            string? numberFormat,
            string? numberFormatSearchType,
            
            IDbTransaction? transaction = null);

        Task CreateAutoNumberAsync(AutoNumber autoNumber, IDbTransaction? transaction = null);
        Task UpdateAutoNumberAsync(AutoNumber autoNumber, IDbTransaction? transaction = null);
        Task DeleteAutoNumberAsync(Guid autoNumberGuid, IDbTransaction? transaction = null);
        Task SoftDeleteAutoNumberAsync(AutoNumber autoNumber, long deletedBy, IDbTransaction? transaction = null);

        // Detail helpers (only if entity has a parent)
        
    }
}
