using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ISalesTransactionPieService
    {
        Task<SalesTransactionPieResultDto> QueryAsync(SalesTransactionPieQueryDto query);
    }
}