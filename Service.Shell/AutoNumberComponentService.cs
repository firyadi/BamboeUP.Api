using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell;
    public class AutoNumberComponentService : IAutoNumberComponentService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public AutoNumberComponentService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<AutoNumberComponentDto>> GetAllAutoNumberComponentsAsync(bool trackChanges)
        {
            var entities = await _repository.AutoNumberComponent.GetAllAutoNumberComponentsAsync(trackChanges);
            return entities.Adapt<IEnumerable<AutoNumberComponentDto>>();
        }

        public async Task<AutoNumberComponentDto> GetAutoNumberComponentByGuidAsync(Guid autoNumberComponentGuid, bool trackChanges)
        {
            var entity = await _repository.AutoNumberComponent.GetAutoNumberComponentAsync(autoNumberComponentGuid, trackChanges);
            return entity.Adapt<AutoNumberComponentDto>();
        }

        public async Task<AutoNumberComponentDto> CreateAutoNumberComponentAsync(AutoNumberComponentForCreationDto input)
        {
            var model = input.Adapt<AutoNumberComponent>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.Now; // Always set server-side
            await _repository.AutoNumberComponent.CreateAutoNumberComponentAsync(model);
            return model.Adapt<AutoNumberComponentDto>();
        }

        public async Task UpdateAutoNumberComponentAsync(Guid autoNumberComponentGuid, AutoNumberComponentForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<AutoNumberComponent>();
            model.AutoNumberComponentGuid = autoNumberComponentGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AutoNumberComponent.UpdateAutoNumberComponentAsync(model);
        }

        public async Task DeleteAutoNumberComponentAsync(Guid autoNumberComponentGuid, AutoNumberComponentForDeleteDto input, bool trackChanges)
        {
            var model = new AutoNumberComponent { AutoNumberComponentGuid = autoNumberComponentGuid };
            await _repository.AutoNumberComponent.SoftDeleteAutoNumberComponentAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteAutoNumberComponentByAdminAsync(Guid autoNumberComponentGuid, bool trackChanges)
        {
            await _repository.AutoNumberComponent.DeleteAutoNumberComponentAsync(autoNumberComponentGuid);
        }

        public async Task<IEnumerable<AutoNumberComponentDto>> SearchAutoNumberComponentAsync(
            string? componentType, string? componentTypeSearchType,
            string? staticValue, string? staticValueSearchType,
            string? format, string? formatSearchType,
            long? autoNumberTemplateId)
        {
            var data = await _repository.AutoNumberComponent.SearchAutoNumberComponentAsync(
                componentType, componentTypeSearchType,
                staticValue, staticValueSearchType,
                format, formatSearchType,
                autoNumberTemplateId);
            return data.Adapt<IEnumerable<AutoNumberComponentDto>>();
        }

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        public async Task<IEnumerable<AutoNumberComponentDto>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid)
        {
            var entities = await _repository.AutoNumberComponent.GetAllByAutoNumberTemplateGuidAsync(autoNumberTemplateGuid, trackChanges: false);
            return entities.Adapt<IEnumerable<AutoNumberComponentDto>>();
        }
    }