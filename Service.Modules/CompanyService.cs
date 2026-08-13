using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public CompanyService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public CompanyService(
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

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
        {
            var entities = await _repository.Company.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<CompanyDto>>();
        }

        public async Task<CompanyDto?> GetCompanyByGuidAsync(Guid companyGuid, bool trackChanges)
        {
            var entity = await _repository.Company.GetCompanyAsync(companyGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<CompanyDto>();
            return dto;
        }

        public async Task<CompanyDto> CreateCompanyAsync(Guid personGuid, CompanyForCreationDto input)
        {
            var model = input.Adapt<Company>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.Company.CreateCompanyAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Company",
                RootEntityKey = model.CompanyGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Company",
                        EntityKey = model.CompanyGuid.ToString(),
                        EntityDisplayName = model.CompanyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<CompanyDto>();
        }

        public async Task UpdateCompanyAsync(Guid personGuid, Guid companyGuid, CompanyForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.Company.GetCompanyAsync(companyGuid, false);

            var model = input.Adapt<Company>();
            model.CompanyGuid = companyGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Company.UpdateCompanyAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Company",
                RootEntityKey = model.CompanyGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Company",
                        EntityKey = model.CompanyGuid.ToString(),
                        EntityDisplayName = model.CompanyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteCompanyAsync(Guid personGuid, Guid companyGuid, CompanyForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.Company.GetCompanyAsync(companyGuid, false);
            var model = new Company
            {
                CompanyGuid = companyGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.Company.SoftDeleteCompanyAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Company",
                RootEntityKey = companyGuid.ToString(),
                RootDisplayName = oldDetail?.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Company",
                        EntityKey = companyGuid.ToString(),
                        EntityDisplayName = oldDetail?.CompanyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteCompanyByAdminAsync(Guid companyGuid, bool trackChanges)
        {
            await _repository.Company.DeleteCompanyAsync(companyGuid);
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompanyAsync(
            string? companyName, string? companyNameSearchType, string? initialName, string? initialNameSearchType, string? taxCompulsionNo, string? taxCompulsionNoSearchType, string? registrationNo, string? registrationNoSearchType, string? defaultCurrency, string? defaultCurrencySearchType, string? companyLogo, string? companyLogoSearchType,
            Guid personGuid, Guid companyGuid)
        {
            var data = await _repository.Company.SearchCompanyAsync(
                companyName, companyNameSearchType, initialName, initialNameSearchType, taxCompulsionNo, taxCompulsionNoSearchType, registrationNo, registrationNoSearchType, defaultCurrency, defaultCurrencySearchType, companyLogo, companyLogoSearchType, 
                personGuid, companyGuid);
            return data.Adapt<IEnumerable<CompanyDto>>();
        }

        public async Task<IEnumerable<CompanyDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.Company.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<CompanyDto>>();
        }

        public async Task<CompanyDto?> GetByPersonGuidAndCompanyGuidAsync(Guid personGuid, Guid companyGuid)
        {
            var result = await _repository.Company.GetByPersonGuidAndCompanyGuidAsync(personGuid, companyGuid);
            if (result == null) return null;
            return result.Adapt<CompanyDto>();
        }
    }
}
