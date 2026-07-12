using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;

namespace Service.Modules
{
    public partial class BankService : IBankService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public BankService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
        }

        public async Task<IEnumerable<BankDto>> GetAllBanksAsync(bool trackChanges)
        {
            var entities = await _repository.Bank.GetAllBanksAsync(trackChanges);
            return entities.Adapt<IEnumerable<BankDto>>();
        }

        public async Task<BankDto?> GetBankByGuidAsync(Guid bankGuid, bool trackChanges)
        {
            var entity = await _repository.Bank.GetBankAsync(bankGuid, trackChanges);
            return entity.Adapt<BankDto>();
        }

        public async Task<BankDto> CreateBankAsync(BankForCreationDto input)
        {
            var model = input.Adapt<Bank>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.Bank.CreateBankAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Bank",
                RootEntityKey = model.BankGuid.ToString(),
                RootDisplayName = model.BankName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Bank",
                        EntityKey = model.BankGuid.ToString(),
                        EntityDisplayName = model.BankName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<BankDto>();
        }

        public async Task UpdateBankAsync(Guid bankGuid, BankForUpdateDto input, bool trackChanges)
        {
            var oldBank = await _repository.Bank.GetBankAsync(bankGuid, false);

            var model = input.Adapt<Bank>();
            model.BankGuid = bankGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Bank.UpdateBankAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Bank",
                RootEntityKey = model.BankGuid.ToString(),
                RootDisplayName = model.BankName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Bank",
                        EntityKey = model.BankGuid.ToString(),
                        EntityDisplayName = model.BankName,
                        ActionType = "UPDATE",
                        OldEntity = oldBank,
                        NewEntity = model
                    }
                }
            });
        }

        // Soft delete (pass full entity for audit)
        public async Task DeleteBankAsync(Guid bankGuid, BankForDeleteDto input, bool trackChanges)
        {
            var oldBank = await _repository.Bank.GetBankAsync(bankGuid, false);
            var model = new Bank
            {
                BankGuid = bankGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.Bank.SoftDeleteBankAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Bank",
                RootEntityKey = bankGuid.ToString(),
                RootDisplayName = oldBank?.BankName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Bank",
                        EntityKey = bankGuid.ToString(),
                        EntityDisplayName = oldBank?.BankName,
                        ActionType = "DELETE",
                        OldEntity = oldBank,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteBankByAdminAsync(Guid bankGuid, bool trackChanges)
        {
            await _repository.Bank.DeleteBankAsync(bankGuid);
        }

        public async Task<IEnumerable<BankDto>> SearchBankAsync(
            string? bankName, string? bankNameSearchType, string? bankInitial, string? bankInitialSearchType)
        {
            var data = await _repository.Bank.SearchBankAsync(
                bankName, bankNameSearchType, bankInitial, bankInitialSearchType);
            return data.Adapt<IEnumerable<BankDto>>();
        }
    }
}