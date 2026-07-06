using Contracts;
using Mapster;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public class AutoNumberLogService : IAutoNumberLogService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;

        public AutoNumberLogService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<AutoNumberLogDto>> GetAllLogsAsync(bool trackChanges)
        {
            var logs = await _repository.AutoNumberLog.GetAllLogsAsync(trackChanges);
            return logs.Adapt<IEnumerable<AutoNumberLogDto>>();
        }

        public async Task<IEnumerable<AutoNumberLogDto>> GetLogsAsync(AutoNumberLogParameters parameters, bool trackChanges)
        {
            var logs = await _repository.AutoNumberLog.GetLogsAsync(parameters, trackChanges);
            return logs.Adapt<IEnumerable<AutoNumberLogDto>>();
        }

        public async Task<AutoNumberLogDto> GetLogByGuidAsync(Guid guid, bool trackChanges)
        {
            var log = await _repository.AutoNumberLog.GetAutoNumberLogByGuidAsync(guid, trackChanges);
            if (log == null)
                throw new KeyNotFoundException($"AutoNumberLog dengan Guid {guid} tidak ditemukan.");

            return log.Adapt<AutoNumberLogDto>();
        }
    }
}
