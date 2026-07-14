using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Shell
{
    public partial class ParameterService : IParameterService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public ParameterService(
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

        public async Task<IEnumerable<ParameterDto>> GetAllParametersAsync(bool trackChanges)
        {
            var entities = await _repository.Parameter.GetAllParametersAsync(trackChanges);
            return entities.Adapt<IEnumerable<ParameterDto>>();
        }

        public async Task<ParameterDto> GetParameterByGuidAsync(Guid parameterGuid, bool trackChanges)
        {
            var entity = await _repository.Parameter.GetParameterAsync(parameterGuid, trackChanges);
            if (entity is null)
                throw new KeyNotFoundException($"Parameter with GUID {parameterGuid} not found.");
            var dto = entity.Adapt<ParameterDto>();

            var scopes = await _repository.ParameterScope.GetAllByParameterGuidAsync(parameterGuid);
            dto.Parameterscopes = scopes.Adapt<IEnumerable<ParameterscopeDto>>();

            return dto;
        }

        public async Task<ParameterDto> CreateParameterAsync(ParameterForCreationDto input)
        {
            var model = input.Adapt<Parameter>();
            model.StatusId = 1;
            await _repository.Parameter.CreateParameterAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "Parameter",
                    EntityKey = model.ParameterGuid.ToString(),
                    EntityDisplayName = model.Parametername,
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = model
                }
            };

            // Dapper doesn't automatically insert children, so we must manually insert nested Details
            if (input.Parameterscopes != null && input.Parameterscopes.Any())
            {
                foreach (var detailDto in input.Parameterscopes)
                {
                    var detail = detailDto.Adapt<Parameterscope>();
                    detail.ParameterId = model.ParameterId;
                    detail.ParameterGuid = model.ParameterGuid;
                    detail.StatusId = 1;

                    // Resolve CompanyId from CompanyGuid
                    if (detail.CompanyGuid.HasValue && detail.CompanyGuid.Value != Guid.Empty)
                    {
                        var company = await _repository.Company.GetCompanyAsync(detail.CompanyGuid.Value, false);
                        if (company != null)
                        {
                            detail.CompanyId = company.CompanyId;
                        }
                    }

                    // Resolve CompanyOfficeId from CompanyOfficeGuid
                    if (detail.CompanyOfficeGuid.HasValue && detail.CompanyOfficeGuid.Value != Guid.Empty)
                    {
                        var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(detail.CompanyOfficeGuid.Value, false);
                        if (office != null)
                        {
                            detail.CompanyOfficeId = office.CompanyOfficeId;
                        }
                    }

                    await _repository.ParameterScope.CreateParameterscopeAsync(detail);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = detail.ParameterscopeGuid.ToString(),
                        EntityDisplayName = detail.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = model.ParameterGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = detail
                    });
                }
            }

            // Single audit session for all entities
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Parameter",
                RootEntityKey = model.ParameterGuid.ToString(),
                RootDisplayName = model.Parametername,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = entries
            });

            return model.Adapt<ParameterDto>();
        }

        public async Task UpdateParameterAsync(Guid parameterGuid, ParameterForUpdateDto input, bool trackChanges)
        {
            // 1. Fetch old data for audit diff
            var oldParameter = await _repository.Parameter.GetParameterAsync(parameterGuid, false);
            if (oldParameter is null)
                throw new KeyNotFoundException($"Parameter with GUID {parameterGuid} not found.");
            var oldParameterscopes = (await _repository.ParameterScope.GetAllByParameterGuidAsync(parameterGuid)).ToList();

            // 2. Update Header
            var model = input.Adapt<Parameter>();
            model.ParameterGuid = parameterGuid;
            model.ParameterId = oldParameter.ParameterId;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Parameter.UpdateParameterAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "Parameter",
                    EntityKey = model.ParameterGuid.ToString(),
                    EntityDisplayName = model.Parametername,
                    ActionType = "UPDATE",
                    OldEntity = oldParameter,
                    NewEntity = model
                }
            };

            // 3. Child alignment/diff logic
            var newParameterscopes = input.Parameterscopes?.ToList() ?? new List<ParameterscopeForUpdateDto>();
            var oldParameterscopeDict = oldParameterscopes.ToDictionary(o => o.ParameterscopeGuid);

            // Process incoming details (create or update)
            foreach (var detailDto in newParameterscopes)
            {
                var detail = detailDto.Adapt<Parameterscope>();
                detail.ParameterGuid = parameterGuid;
                detail.StatusId = 2;
                detail.UpdatedTime = DateTime.UtcNow;

                // Resolve CompanyId from CompanyGuid
                if (detail.CompanyGuid.HasValue && detail.CompanyGuid.Value != Guid.Empty)
                {
                    var company = await _repository.Company.GetCompanyAsync(detail.CompanyGuid.Value, false);
                    if (company != null)
                    {
                        detail.CompanyId = company.CompanyId;
                    }
                }

                // Resolve CompanyOfficeId from CompanyOfficeGuid
                if (detail.CompanyOfficeGuid.HasValue && detail.CompanyOfficeGuid.Value != Guid.Empty)
                {
                    var office = await _repository.CompanyOffice.GetCompanyOfficeAsync(detail.CompanyOfficeGuid.Value, false);
                    if (office != null)
                    {
                        detail.CompanyOfficeId = office.CompanyOfficeId;
                    }
                }

                if (detailDto.ParameterscopeGuid != Guid.Empty && oldParameterscopeDict.ContainsKey(detailDto.ParameterscopeGuid))
                {
                    // Existing detail → UPDATE
                    detail.ParameterscopeGuid = detailDto.ParameterscopeGuid;
                    detail.ParameterId = model.ParameterId;
                    detail.UpdatedById = input.UpdatedById;
                    await _repository.ParameterScope.UpdateParameterscopeAsync(detail);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = detail.ParameterscopeGuid.ToString(),
                        EntityDisplayName = detail.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = parameterGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldParameterscopeDict[detail.ParameterscopeGuid],
                        NewEntity = detail
                    });
                }
                else
                {
                    // New detail → CREATE
                    detail.ParameterscopeGuid = Guid.NewGuid();
                    detail.ParameterId = model.ParameterId;
                    detail.CreatedById = input.UpdatedById;
                    detail.StatusId = 1;
                    await _repository.ParameterScope.CreateParameterscopeAsync(detail);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = detail.ParameterscopeGuid.ToString(),
                        EntityDisplayName = detail.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = parameterGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = detail
                    });
                }
            }

            // Details removed from UI → soft delete
            foreach (var oldParameterscope in oldParameterscopes)
            {
                if (!newParameterscopes.Any(o => o.ParameterscopeGuid == oldParameterscope.ParameterscopeGuid))
                {
                    await _repository.ParameterScope.SoftDeleteParameterscopeAsync(
                        new Parameterscope { ParameterscopeGuid = oldParameterscope.ParameterscopeGuid },
                        input.UpdatedById);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = oldParameterscope.ParameterscopeGuid.ToString(),
                        EntityDisplayName = oldParameterscope.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = parameterGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldParameterscope,
                        NewEntity = null
                    });
                }
            }

            // 4. Single audit session for all entities
            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Parameter",
                RootEntityKey = model.ParameterGuid.ToString(),
                RootDisplayName = model.Parametername,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                Entries = entries
            });
        }

        public async Task DeleteParameterAsync(Guid parameterGuid, ParameterForDeleteDto input, bool trackChanges)
        {
            var oldParameter = await _repository.Parameter.GetParameterAsync(parameterGuid, false);
            var model = new Parameter { ParameterGuid = parameterGuid };
            await _repository.Parameter.SoftDeleteParameterAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Parameter",
                RootEntityKey = parameterGuid.ToString(),
                RootDisplayName = oldParameter?.Parametername,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "Parameter",
                        EntityKey = parameterGuid.ToString(),
                        EntityDisplayName = oldParameter?.Parametername,
                        ActionType = "DELETE",
                        OldEntity = oldParameter,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteParameterByAdminAsync(Guid parameterGuid, bool trackChanges)
        {
            await _repository.Parameter.DeleteParameterAsync(parameterGuid);
        }

        public async Task<IEnumerable<ParameterDto>> SearchParameterAsync(
            string? parametername, string? parameternameSearchType,
            string? parametervalue, string? parametervalueSearchType)
        {
            var data = await _repository.Parameter.SearchParameterAsync(
                parametername, parameternameSearchType, parametervalue, parametervalueSearchType);
            return data.Adapt<IEnumerable<ParameterDto>>();
        }
    }
}
