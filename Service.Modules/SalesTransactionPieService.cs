using BamboeUp.Audit.Contracts;
using Contracts;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class SalesTransactionPieService : ISalesTransactionPieService
    {
        private readonly IRepositoryManager _repository;

        public SalesTransactionPieService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext)
        {
            _repository = repository;
            _ = logger;
            _ = transactionManager;
            _ = audit;
            _ = userContext;
        }

        public Task<SalesTransactionPieResultDto> QueryAsync(SalesTransactionPieQueryDto query)
            => _repository.SalesTransactionPie.QueryAsync(query);
    }
}