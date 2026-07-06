using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell;
    public class AutoNumberTemplateService : IAutoNumberTemplateService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public AutoNumberTemplateService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<AutoNumberTemplateDto>> GetAllAutoNumberTemplatesAsync(bool trackChanges)
        {
            var entities = await _repository.AutoNumberTemplate.GetAllAutoNumberTemplatesAsync(trackChanges);
            return entities.Adapt<IEnumerable<AutoNumberTemplateDto>>();
        }

        public async Task<AutoNumberTemplateDto> GetAutoNumberTemplateByGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges)
        {
            var entity = await _repository.AutoNumberTemplate.GetAutoNumberTemplateAsync(autoNumberTemplateGuid, trackChanges);
            return entity.Adapt<AutoNumberTemplateDto>();
        }

        public async Task<AutoNumberTemplateDto> CreateAutoNumberTemplateAsync(AutoNumberTemplateForCreationDto input)
        {
            var model = input.Adapt<AutoNumberTemplate>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.Now; // Always set server-side
            await _repository.AutoNumberTemplate.CreateAutoNumberTemplateAsync(model);
            return model.Adapt<AutoNumberTemplateDto>();
        }

        public async Task UpdateAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, AutoNumberTemplateForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<AutoNumberTemplate>();
            model.AutoNumberTemplateGuid = autoNumberTemplateGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.AutoNumberTemplate.UpdateAutoNumberTemplateAsync(model);
        }

        public async Task DeleteAutoNumberTemplateAsync(Guid autoNumberTemplateGuid, AutoNumberTemplateForDeleteDto input, bool trackChanges)
        {
            var model = new AutoNumberTemplate { AutoNumberTemplateGuid = autoNumberTemplateGuid };
            await _repository.AutoNumberTemplate.SoftDeleteAutoNumberTemplateAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteAutoNumberTemplateByAdminAsync(Guid autoNumberTemplateGuid, bool trackChanges)
        {
            await _repository.AutoNumberTemplate.DeleteAutoNumberTemplateAsync(autoNumberTemplateGuid);
        }

        public async Task<IEnumerable<AutoNumberTemplateDto>> SearchAutoNumberTemplateAsync(
            string? templateName, string? templateNameSearchType,
            string? description, string? descriptionSearchType,
            DateTime? effectiveDateFrom, DateTime? effectiveDateTo,
            string? templateScopeType,
            long? companyId,
            long? companyOfficeId)
        {
            var data = await _repository.AutoNumberTemplate.SearchAutoNumberTemplateAsync(
                templateName, templateNameSearchType,
                description, descriptionSearchType,
                effectiveDateFrom, effectiveDateTo,
                templateScopeType, companyId, companyOfficeId);
            return data.Adapt<IEnumerable<AutoNumberTemplateDto>>();
        }
    }