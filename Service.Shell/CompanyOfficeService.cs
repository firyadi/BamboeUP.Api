// Old-style service implementation (detail/child helpers included)
using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public partial class CompanyOfficeService : ICompanyOfficeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public CompanyOfficeService(
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

        public async Task<IEnumerable<CompanyOfficeDto>> GetAllCompanyOfficesAsync(bool trackChanges)
        {
            var entities = await _repository.CompanyOffice.GetAllCompanyOfficesAsync(trackChanges);
            return entities.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        public async Task<CompanyOfficeDto> GetCompanyOfficeByGuidAsync(Guid companyOfficeGuid, bool trackChanges)
        {
            var entity = await _repository.CompanyOffice.GetCompanyOfficeAsync(companyOfficeGuid, trackChanges);
            return entity.Adapt<CompanyOfficeDto>();
        }

        public async Task<CompanyOfficeDto> CreateCompanyOfficeAsync(CompanyOfficeForCreationDto input)
        {
            var model = input.Adapt<CompanyOffice>();
            model.StatusId = 1;
            await _repository.CompanyOffice.CreateCompanyOfficeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "CompanyOffice",
                RootEntityKey = model.CompanyOfficeGuid.ToString(),
                RootDisplayName = model.CompanyOfficeName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = model.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = model.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = model.CompanyGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<CompanyOfficeDto>();
        }

        public async Task UpdateCompanyOfficeAsync(Guid companyOfficeGuid, CompanyOfficeForUpdateDto input, bool trackChanges)
        {
            var oldOffice = await _repository.CompanyOffice.GetCompanyOfficeAsync(companyOfficeGuid, false);

            var model = input.Adapt<CompanyOffice>();
            model.CompanyOfficeGuid = companyOfficeGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.CompanyOffice.UpdateCompanyOfficeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "CompanyOffice",
                RootEntityKey = model.CompanyOfficeGuid.ToString(),
                RootDisplayName = model.CompanyOfficeName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = model.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = model.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = model.CompanyGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldOffice,
                        NewEntity = model
                    }
                }
            });
        }

        // Soft delete - keep old pattern with full entity to repository
        public async Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, CompanyOfficeForDeleteDto input, bool trackChanges)
        {
            var oldOffice = await _repository.CompanyOffice.GetCompanyOfficeAsync(companyOfficeGuid, false);
            var model = new CompanyOffice { CompanyOfficeGuid = companyOfficeGuid };
            await _repository.CompanyOffice.SoftDeleteCompanyOfficeAsync(model, input.DeletedById ?? 0);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "CompanyOffice",
                RootEntityKey = companyOfficeGuid.ToString(),
                RootDisplayName = oldOffice?.CompanyOfficeName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (input.DeletedById ?? 0).ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = companyOfficeGuid.ToString(),
                        EntityDisplayName = oldOffice?.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = oldOffice?.CompanyGuid.ToString() ?? string.Empty,
                        ActionType = "DELETE",
                        OldEntity = oldOffice,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteCompanyOfficeByAdminAsync(Guid companyOfficeGuid, bool trackChanges)
        {
            await _repository.CompanyOffice.DeleteCompanyOfficeAsync(companyOfficeGuid);
        }

        public async Task<IEnumerable<CompanyOfficeDto>> SearchCompanyOfficeAsync(
            string? companyOfficeName, string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId, 
            long? stateId, 
            long? cityId, 
            string? postalCodeId, string? postalCodeIdSearchType,
            string? address, string? addressSearchType, 
            Guid companyGuid, Guid companyOfficeGuid)
        {
            var data = await _repository.CompanyOffice.SearchCompanyOfficeAsync(
                companyOfficeName, companyOfficeNameSearchType, srAddressType, countryId, stateId, cityId, postalCodeId, postalCodeIdSearchType, address, addressSearchType, companyGuid, companyOfficeGuid);
            return data.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        // Detail (child) helpers
        public async Task<IEnumerable<CompanyOfficeDto>> GetAllByCompanyGuidAsync(Guid companyGuid)
        {
            var result = await _repository.CompanyOffice.GetAllByCompanyGuidAsync(companyGuid);
            return result.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        public async Task<CompanyOfficeDto> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid)
        {
            var result = await _repository.CompanyOffice.GetByCompanyGuidAndCompanyOfficeGuidAsync(companyGuid, companyOfficeGuid);
            return result.Adapt<CompanyOfficeDto>();
        }
    }
}

