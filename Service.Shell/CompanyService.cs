using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public partial class CompanyService : ICompanyService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

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
            var entities = await _repository.Company.GetAllCompaniesAsync(trackChanges);
            return entities.Adapt<IEnumerable<CompanyDto>>();
        }

        public async Task<CompanyDto> GetCompanyByGuidAsync(Guid companyGuid, bool trackChanges)
        {
            var entity = await _repository.Company.GetCompanyAsync(companyGuid, trackChanges);
            return entity.Adapt<CompanyDto>();
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto input)
        {
            var model = input.Adapt<Company>();
            model.StatusId = 1;
            await _repository.Company.CreateCompanyAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "Company",
                    EntityKey = model.CompanyGuid.ToString(),
                    EntityDisplayName = model.CompanyName,
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = model
                }
            };

            // Dapper doesn't automatically insert children, so we must manually insert nested Offices
            if (input.Offices != null && input.Offices.Any())
            {
                foreach (var officeDto in input.Offices)
                {
                    var office = officeDto.Adapt<CompanyOffice>();
                    office.CompanyId = model.CompanyId;
                    office.CompanyGuid = model.CompanyGuid;
                    office.StatusId = 1;
                    await _repository.CompanyOffice.CreateCompanyOfficeAsync(office);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = office.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = office.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = model.CompanyGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = office
                    });
                }
            }

            // Single audit session for all entities
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Company",
                RootEntityKey = model.CompanyGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = entries
            });

            return model.Adapt<CompanyDto>();
        }

        public async Task UpdateCompanyAsync(Guid companyGuid, CompanyForUpdateDto input, bool trackChanges)
        {
            // Fetch old data for audit diff
            var oldCompany = await _repository.Company.GetCompanyAsync(companyGuid, false);
            var oldOffices = (await _repository.CompanyOffice.GetAllByCompanyGuidAsync(companyGuid)).ToList();

            // Update Company header
            var model = input.Adapt<Company>();
            model.CompanyGuid = companyGuid;
            model.CompanyId = oldCompany.CompanyId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Company.UpdateCompanyAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "Company",
                    EntityKey = model.CompanyGuid.ToString(),
                    EntityDisplayName = model.CompanyName,
                    ActionType = "UPDATE",
                    OldEntity = oldCompany,
                    NewEntity = model
                }
            };

            // Office diff logic
            var newOffices = input.Offices?.ToList() ?? new List<CompanyOfficeForUpdateDto>();
            var oldOfficeDict = oldOffices.ToDictionary(o => o.CompanyOfficeGuid);

            // Process incoming offices (create or update)
            foreach (var officeDto in newOffices)
            {
                var office = officeDto.Adapt<CompanyOffice>();
                office.CompanyGuid = companyGuid;
                office.StatusId = 2;
                office.UpdatedTime = DateTime.UtcNow;

                if (officeDto.CompanyOfficeGuid != Guid.Empty && oldOfficeDict.ContainsKey(officeDto.CompanyOfficeGuid))
                {
                    // Existing office → UPDATE
                    office.CompanyOfficeGuid = officeDto.CompanyOfficeGuid;
                    office.CompanyId = model.CompanyId;
                    office.UpdatedById = input.UpdatedById;
                    await _repository.CompanyOffice.UpdateCompanyOfficeAsync(office);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = office.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = office.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = companyGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldOfficeDict[office.CompanyOfficeGuid],
                        NewEntity = office
                    });
                }
                else
                {
                    // New office → CREATE
                    office.CompanyOfficeGuid = Guid.NewGuid();
                    office.CompanyId = model.CompanyId;
                    office.CreatedById = input.UpdatedById;
                    office.StatusId = 1;
                    await _repository.CompanyOffice.CreateCompanyOfficeAsync(office);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = office.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = office.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = companyGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = office
                    });
                }
            }

            // Offices removed from UI → soft delete
            foreach (var oldOffice in oldOffices)
            {
                if (!newOffices.Any(o => o.CompanyOfficeGuid == oldOffice.CompanyOfficeGuid))
                {
                    await _repository.CompanyOffice.SoftDeleteCompanyOfficeAsync(
                        new CompanyOffice { CompanyOfficeGuid = oldOffice.CompanyOfficeGuid },
                        input.UpdatedById);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "CompanyOffice",
                        EntityKey = oldOffice.CompanyOfficeGuid.ToString(),
                        EntityDisplayName = oldOffice.CompanyOfficeName,
                        ParentTableName = "Company",
                        ParentEntityKey = companyGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldOffice,
                        NewEntity = null
                    });
                }
            }

            // Single audit session for all entities
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Company",
                RootEntityKey = model.CompanyGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                Entries = entries
            });
        }

        // Soft delete (pass full entity for audit)
        public async Task DeleteCompanyAsync(Guid companyGuid, CompanyForDeleteDto input, bool trackChanges)
        {
            var oldCompany = await _repository.Company.GetCompanyAsync(companyGuid, false);
            var model = new Company { CompanyGuid = companyGuid };
            await _repository.Company.SoftDeleteCompanyAsync(model, input.DeletedById ?? 0);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Company",
                RootEntityKey = companyGuid.ToString(),
                RootDisplayName = oldCompany?.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : (input.DeletedById ?? 0).ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Company",
                        EntityKey = companyGuid.ToString(),
                        EntityDisplayName = oldCompany?.CompanyName,
                        ActionType = "DELETE",
                        OldEntity = oldCompany,
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
            string? companyName, string? companyNameSearchType, string? initialName, string? initialNameSearchType, string? taxCompulsionNo, string? taxCompulsionNoSearchType, string? registrationNo, string? registrationNoSearchType, string? defaultCurrency, string? defaultCurrencySearchType)
        {
            var data = await _repository.Company.SearchCompanyAsync(
                companyName, companyNameSearchType, initialName, initialNameSearchType, taxCompulsionNo, taxCompulsionNoSearchType, registrationNo, registrationNoSearchType, defaultCurrency, defaultCurrencySearchType);
            return data.Adapt<IEnumerable<CompanyDto>>();
        }
    }
}
