using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IReportDefinitionService
    {
        Task<IEnumerable<ReportDefinitionDto>> GetAllAsync();
        Task<ReportDefinitionDto> GetByGuidAsync(Guid reportDefinitionGuid);
        Task<IEnumerable<ReportDefinitionDto>> SearchAsync(ReportDefinitionSearchDto criteria);
        Task<ReportDefinitionDto> CreateAsync(ReportDefinitionForCreationDto input);
        Task UpdateAsync(Guid reportDefinitionGuid, ReportDefinitionForUpdateDto input);
        Task DeleteAsync(Guid reportDefinitionGuid, ReportDefinitionForDeleteDto input);
        Task<IReadOnlyList<ReportParameterDefinitionDto>> GetParametersAsync(Guid reportDefinitionGuid);
        Task ReplaceParametersAsync(Guid reportDefinitionGuid, ReportParameterBatchReplaceDto input);
    }
}
