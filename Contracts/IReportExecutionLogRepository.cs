using Shared.DataTransferObjects;
using System.Data;

namespace Contracts
{
    public interface IReportExecutionLogRepository
    {
        Task<long> InsertAsync(ReportExecutionLogInsert row, IDbTransaction? transaction = null);

        Task UpdateAsync(ReportExecutionLogUpdate row, IDbTransaction? transaction = null);

        Task<ReportExecutionLogDto?> GetByPrintIdAsync(string reportPrintId, bool includeFullPrintId, IDbTransaction? transaction = null);

        Task<PagedResult<ReportExecutionLogDto>> GetPagedAsync(
            Guid? userGuid,
            string? companyId,
            string? officeId,
            bool scoped,
            ReportExecutionLogQueryDto query,
            IDbTransaction? transaction = null);
    }
}
