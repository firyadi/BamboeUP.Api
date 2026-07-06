using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class AutoNumberService : IAutoNumberService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public AutoNumberService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<AutoNumberDto>> GetAllAutoNumbersAsync(bool trackChanges)
        {
            var entities = await _repository.AutoNumber.GetAllAutoNumbersAsync(trackChanges);
            return entities.Adapt<IEnumerable<AutoNumberDto>>();
        }

        public async Task<AutoNumberDto> GetAutoNumberByGuidAsync(Guid autoNumberGuid, bool trackChanges)
        {
            var entity = await _repository.AutoNumber.GetAutoNumberAsync(autoNumberGuid, trackChanges);
            return entity.Adapt<AutoNumberDto>();
        }

        public async Task<AutoNumberDto> CreateAutoNumberAsync(AutoNumberForCreationDto input)
        {
            var model = input.Adapt<AutoNumber>();
            model.StatusId = 1;
            await _repository.AutoNumber.CreateAutoNumberAsync(model);
            return model.Adapt<AutoNumberDto>();
        }

        public async Task UpdateAutoNumberAsync(Guid autoNumberGuid, AutoNumberForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<AutoNumber>();
            model.AutoNumberGuid = autoNumberGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AutoNumber.UpdateAutoNumberAsync(model);
        }

        // Soft delete (pass full entity for audit)
        public async Task DeleteAutoNumberAsync(Guid autoNumberGuid, AutoNumberForDeleteDto input, bool trackChanges)
        {
            var model = new AutoNumber { AutoNumberGuid = autoNumberGuid };
            await _repository.AutoNumber.SoftDeleteAutoNumberAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteAutoNumberByAdminAsync(Guid autoNumberGuid, bool trackChanges)
        {
            await _repository.AutoNumber.DeleteAutoNumberAsync(autoNumberGuid);
        }

        public async Task<IEnumerable<AutoNumberDto>> SearchAutoNumberAsync(
            string? prefik, string? prefikSearchType, string? separatorAfterPrefik, string? separatorAfterPrefikSearchType, string? separatorAfterDept, string? separatorAfterDeptSearchType, string? separatorAfterYear, string? separatorAfterYearSearchType, string? separatorAfterMonth, string? separatorAfterMonthSearchType, string? separatorAfterDay, string? separatorAfterDaySearchType, string? numberGroupSeparator, string? numberGroupSeparatorSearchType, string? numberFormat, string? numberFormatSearchType)
        {
            var data = await _repository.AutoNumber.SearchAutoNumberAsync(
                prefik, prefikSearchType, separatorAfterPrefik, separatorAfterPrefikSearchType, separatorAfterDept, separatorAfterDeptSearchType, separatorAfterYear, separatorAfterYearSearchType, separatorAfterMonth, separatorAfterMonthSearchType, separatorAfterDay, separatorAfterDaySearchType, numberGroupSeparator, numberGroupSeparatorSearchType, numberFormat, numberFormatSearchType);
            return data.Adapt<IEnumerable<AutoNumberDto>>();
        }
    }
}