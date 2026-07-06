using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Shell
{
    public interface IAutoNumberLogService
    {
        Task<IEnumerable<AutoNumberLogDto>> GetAllLogsAsync(bool trackChanges);
        Task<IEnumerable<AutoNumberLogDto>> GetLogsAsync(AutoNumberLogParameters parameters, bool trackChanges);
        Task<AutoNumberLogDto> GetLogByGuidAsync(Guid guid, bool trackChanges);
    }
}
