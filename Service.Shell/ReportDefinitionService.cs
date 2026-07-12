using Contracts;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class ReportDefinitionService : IReportDefinitionService
    {
        private readonly IRepositoryManager _repository;

        public ReportDefinitionService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<ReportDefinitionDto>> GetAllAsync()
            => _repository.ReportDefinition.GetAllAsync();

        public async Task<ReportDefinitionDto> GetByGuidAsync(Guid reportDefinitionGuid)
        {
            var row = await _repository.ReportDefinition.GetByGuidAsync(reportDefinitionGuid);
            if (row == null)
                throw new KeyNotFoundException($"ReportDefinition {reportDefinitionGuid} not found.");
            return row;
        }

        public Task<IEnumerable<ReportDefinitionDto>> SearchAsync(ReportDefinitionSearchDto criteria)
            => _repository.ReportDefinition.SearchAsync(criteria);

        public async Task<ReportDefinitionDto> CreateAsync(ReportDefinitionForCreationDto input)
        {
            var guid = await _repository.ReportDefinition.CreateAsync(input);
            return await GetByGuidAsync(guid);
        }

        public Task UpdateAsync(Guid reportDefinitionGuid, ReportDefinitionForUpdateDto input)
            => _repository.ReportDefinition.UpdateAsync(reportDefinitionGuid, input);

        public Task DeleteAsync(Guid reportDefinitionGuid, ReportDefinitionForDeleteDto input)
            => _repository.ReportDefinition.SoftDeleteAsync(reportDefinitionGuid, input.DeletedById);
    }
}
