using Entities.Models;
using System.Data;

namespace Contracts
{
    public interface IProgramRepository
    {
        Task<Programs> GetProgramAsync(Guid programGuid, bool trackChanges);
        Task<IEnumerable<Programs>> GetAllProgramsAsync(bool trackChanges);
        Task CreateProgramAsync(Programs program, IDbTransaction? transaction = null);
        Task UpdateProgramAsync(Programs program, IDbTransaction? transaction = null);
        Task SoftDeleteProgramAsync(Programs program, long deletedBy, IDbTransaction? transaction = null);
        Task DeleteProgramAsync(Guid programGuid, IDbTransaction? transaction = null);
        Task<IEnumerable<Programs>> GetAllowedProgramsAsync(Guid userGuid, string? companyId, string? officeId, IDbTransaction? transaction = null);
        Task<IEnumerable<Programs>> SearchProgramAsync(
            string? programName, string? programNameSearchType,
            string? programCode, string? programCodeSearchType,
            IDbTransaction? transaction = null);
    }
}
