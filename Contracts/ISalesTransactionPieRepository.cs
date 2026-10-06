using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ISalesTransactionPieRepository
    {
        Task<SalesTransactionPieResultDto> QueryAsync(SalesTransactionPieQueryDto query);
    }
}