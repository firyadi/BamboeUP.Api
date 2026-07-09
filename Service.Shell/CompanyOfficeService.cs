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
        private readonly IUserScopeService _userScope;

        public CompanyOfficeService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext,
            IUserScopeService userScope)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
            _userScope = userScope;
        }

        public async Task<IEnumerable<CompanyOfficeDto>> GetAllCompanyOfficesAsync(bool trackChanges)
        {
            return await _userScope.GetAllAccessibleOfficesAsync();
        }

        public async Task<CompanyOfficeDto> GetCompanyOfficeByGuidAsync(Guid companyOfficeGuid, bool trackChanges)
        {
            var entity = await _repository.CompanyOffice.GetCompanyOfficeAsync(companyOfficeGuid, trackChanges);
            await _userScope.EnsureCanAccessOfficeAsync(entity.CompanyId, entity.CompanyOfficeId);
            return entity.Adapt<CompanyOfficeDto>();
        }

        public async Task<CompanyOfficeDto> CreateCompanyOfficeAsync(CompanyOfficeForCreationDto input)
        {
            await _userScope.EnsureCanAccessCompanyAsync(input.CompanyId);
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
            await _userScope.EnsureCanAccessCompanyAsync(oldOffice.CompanyId);

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
            await _userScope.EnsureCanAccessCompanyAsync(oldOffice.CompanyId);
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

            if (companyGuid != Guid.Empty)
            {
                var company = await _repository.Company.GetCompanyAsync(companyGuid, false);
                await _userScope.EnsureCanAccessCompanyAsync(company.CompanyId);
                return data.Adapt<IEnumerable<CompanyOfficeDto>>();
            }

            var allAccessible = await _userScope.GetAllAccessibleOfficesAsync();
            var allowedGuids = allAccessible.Select(o => o.CompanyOfficeGuid).ToHashSet();
            return data.Where(o => allowedGuids.Contains(o.CompanyOfficeGuid)).Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        // Header-detail: all offices under company (no UserGroupScope office filter).
        public async Task<IEnumerable<CompanyOfficeDto>> GetAllByCompanyGuidAsync(Guid companyGuid)
        {
            var company = await _repository.Company.GetCompanyAsync(companyGuid, false);
            await _userScope.EnsureCanAccessCompanyAsync(company.CompanyId);
            var result = await _repository.CompanyOffice.GetAllByCompanyGuidAsync(companyGuid);
            return result.Adapt<IEnumerable<CompanyOfficeDto>>();
        }

        public async Task<CompanyOfficeDto> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid)
        {
            var company = await _repository.Company.GetCompanyAsync(companyGuid, false);
            await _userScope.EnsureCanAccessCompanyAsync(company.CompanyId);
            var result = await _repository.CompanyOffice.GetByCompanyGuidAndCompanyOfficeGuidAsync(companyGuid, companyOfficeGuid);
            return result.Adapt<CompanyOfficeDto>();
        }
    }
}

