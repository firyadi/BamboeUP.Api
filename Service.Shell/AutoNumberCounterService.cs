using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell;
    public class AutoNumberCounterService : IAutoNumberCounterService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public AutoNumberCounterService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<AutoNumberCounterDto>> GetAllAutoNumberCountersAsync(bool trackChanges)
        {
            var entities = await _repository.AutoNumberCounter.GetAllAutoNumberCountersAsync(trackChanges);
            return entities.Adapt<IEnumerable<AutoNumberCounterDto>>();
        }

        public async Task<AutoNumberCounterDto> GetAutoNumberCounterByGuidAsync(Guid autoNumberCounterGuid, bool trackChanges)
        {
            var entity = await _repository.AutoNumberCounter.GetAutoNumberCounterAsync(autoNumberCounterGuid, trackChanges)
                ?? throw new KeyNotFoundException($"AutoNumberCounter with Guid '{autoNumberCounterGuid}' not found.");
            return entity.Adapt<AutoNumberCounterDto>();
        }

        public async Task<AutoNumberCounterDto> CreateAutoNumberCounterAsync(AutoNumberCounterForCreationDto input)
        {
            var model = input.Adapt<AutoNumberCounter>();
            model.StatusId = 1;
            await _repository.AutoNumberCounter.CreateAutoNumberCounterAsync(model);
            return model.Adapt<AutoNumberCounterDto>();
        }

        public async Task UpdateAutoNumberCounterAsync(Guid autoNumberCounterGuid, AutoNumberCounterForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<AutoNumberCounter>();
            model.AutoNumberCounterGuid = autoNumberCounterGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AutoNumberCounter.UpdateAutoNumberCounterAsync(model);
        }

        public async Task DeleteAutoNumberCounterAsync(Guid autoNumberCounterGuid, AutoNumberCounterForDeleteDto input, bool trackChanges)
        {
            var model = new AutoNumberCounter { AutoNumberCounterGuid = autoNumberCounterGuid };
            await _repository.AutoNumberCounter.SoftDeleteAutoNumberCounterAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteAutoNumberCounterByAdminAsync(Guid autoNumberCounterGuid, bool trackChanges)
        {
            await _repository.AutoNumberCounter.DeleteAutoNumberCounterAsync(autoNumberCounterGuid);
        }

        public async Task<IEnumerable<AutoNumberCounterDto>> SearchAutoNumberCounterAsync(
            long? autoNumberTemplateId,
            long? companyId,
            long? companyOfficeId,
            int? organizationUnitId,
            int? yearNo,
            int? monthNo,
            int? dayNo)
        {
            var data = await _repository.AutoNumberCounter.SearchAutoNumberCounterAsync(
                autoNumberTemplateId, companyId, companyOfficeId,
                organizationUnitId, yearNo, monthNo, dayNo);
            return data.Adapt<IEnumerable<AutoNumberCounterDto>>();
        }

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        public async Task<IEnumerable<AutoNumberCounterDto>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid)
        {
            var entities = await _repository.AutoNumberCounter.GetAllByAutoNumberTemplateGuidAsync(autoNumberTemplateGuid, trackChanges: false);
            return entities.Adapt<IEnumerable<AutoNumberCounterDto>>();
        }
    }