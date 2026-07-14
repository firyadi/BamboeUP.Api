using Shared.DataTransferObjects;
using System.Data;

namespace Contracts
{
    public interface IReportDefinitionRepository
    {
        Task<IEnumerable<ReportDefinitionDto>> GetAllAsync(IDbTransaction? transaction = null);

        Task<ReportDefinitionDto?> GetByGuidAsync(Guid reportDefinitionGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<ReportDefinitionDto>> SearchAsync(ReportDefinitionSearchDto criteria, IDbTransaction? transaction = null);

        Task<Guid> CreateAsync(ReportDefinitionForCreationDto input, IDbTransaction? transaction = null);

        Task UpdateAsync(Guid reportDefinitionGuid, ReportDefinitionForUpdateDto input, IDbTransaction? transaction = null);

        Task SoftDeleteAsync(Guid reportDefinitionGuid, long deletedById, IDbTransaction? transaction = null);

        Task ReplaceParametersAsync(
            long reportDefinitionId,
            IReadOnlyList<ReportParameterForUpsertDto> parameters,
            long updatedById,
            IDbTransaction? transaction = null);
    }
}
