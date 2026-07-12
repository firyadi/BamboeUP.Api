using Entities.Models;
using Shared.RequestFeatures;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAutoNumberLogRepository
    {
        /// <summary>Insert satu log record dalam suatu transaction.</summary>
        Task CreateAutoNumberLogAsync(AutoNumberLog log, IDbTransaction? transaction = null);

        /// <summary>Ambil semua log berdasarkan TemplateId.</summary>
        Task<IEnumerable<AutoNumberLog>> GetLogsByTemplateIdAsync(long templateId, bool trackChanges);

        /// <summary>Ambil log berdasarkan Guid.</summary>
        Task<AutoNumberLog?> GetAutoNumberLogByGuidAsync(Guid logGuid, bool trackChanges);

        /// <summary>Ambil semua log (opsional batasi dengan pagination jika diperlukan nanti).</summary>
        Task<IEnumerable<AutoNumberLog>> GetAllLogsAsync(bool trackChanges);

        /// <summary>Ambil log dengan filter pencarian.</summary>
        Task<IEnumerable<AutoNumberLogView>> GetLogsAsync(AutoNumberLogParameters parameters, bool trackChanges);
    }
}
