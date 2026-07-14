using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Shell
{
    public partial class ParameterscopeService : IParameterscopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public ParameterscopeService(
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

        public async Task<IEnumerable<ParameterscopeDto>> GetAllCompanyOfficesAsync(bool trackChanges)
        {
            var entities = await _repository.ParameterScope.GetAllByParameterGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<ParameterscopeDto>>();
        }

        public async Task<ParameterscopeDto> GetParameterscopeByGuidAsync(Guid parameterscopeGuid, bool trackChanges)
        {
            var entity = await _repository.ParameterScope.GetParameterscopeAsync(parameterscopeGuid, trackChanges)
                ?? throw new KeyNotFoundException($"Parameterscope with Guid '{parameterscopeGuid}' not found.");
            return entity.Adapt<ParameterscopeDto>();
        }

        public async Task<ParameterscopeDto> CreateParameterscopeAsync(ParameterscopeForCreationDto input)
        {
            var model = input.Adapt<Parameterscope>();
            model.StatusId = 1;
            await _repository.ParameterScope.CreateParameterscopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "ParameterScope",
                RootEntityKey = model.ParameterscopeGuid.ToString(),
                RootDisplayName = model.Overridevalue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = model.ParameterscopeGuid.ToString(),
                        EntityDisplayName = model.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = model.ParameterGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<ParameterscopeDto>();
        }

        public async Task UpdateParameterscopeAsync(Guid parameterscopeGuid, ParameterscopeForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.ParameterScope.GetParameterscopeAsync(parameterscopeGuid, false);

            var model = input.Adapt<Parameterscope>();
            model.ParameterscopeGuid = parameterscopeGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.ParameterScope.UpdateParameterscopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "ParameterScope",
                RootEntityKey = model.ParameterscopeGuid.ToString(),
                RootDisplayName = model.Overridevalue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = model.ParameterscopeGuid.ToString(),
                        EntityDisplayName = model.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = model.ParameterGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteParameterscopeAsync(Guid parameterscopeGuid, ParameterscopeForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.ParameterScope.GetParameterscopeAsync(parameterscopeGuid, false);
            var model = new Parameterscope { ParameterscopeGuid = parameterscopeGuid };
            await _repository.ParameterScope.SoftDeleteParameterscopeAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "ParameterScope",
                RootEntityKey = parameterscopeGuid.ToString(),
                RootDisplayName = oldDetail?.Overridevalue,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "ParameterScope",
                        EntityKey = parameterscopeGuid.ToString(),
                        EntityDisplayName = oldDetail?.Overridevalue,
                        ParentTableName = "Parameter",
                        ParentEntityKey = oldDetail?.ParameterGuid.ToString() ?? string.Empty,
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteParameterscopeByAdminAsync(Guid parameterscopeGuid, bool trackChanges)
        {
            await _repository.ParameterScope.DeleteParameterscopeAsync(parameterscopeGuid);
        }

        public async Task<IEnumerable<ParameterscopeDto>> SearchParameterscopeAsync(
            string? overridevalue, string? overridevalueSearchType,
            Guid parameterGuid, Guid parameterscopeGuid)
        {
            var data = await _repository.ParameterScope.SearchParameterscopeAsync(
                overridevalue, overridevalueSearchType, parameterGuid, parameterscopeGuid);
            return data.Adapt<IEnumerable<ParameterscopeDto>>();
        }

        public async Task<IEnumerable<ParameterscopeDto>> GetAllByParameterGuidAsync(Guid parameterGuid)
        {
            var result = await _repository.ParameterScope.GetAllByParameterGuidAsync(parameterGuid);
            return result.Adapt<IEnumerable<ParameterscopeDto>>();
        }

        public async Task<ParameterscopeDto> GetByParameterGuidAndParameterscopeGuidAsync(Guid parameterGuid, Guid parameterscopeGuid)
        {
            var result = await _repository.ParameterScope.GetByParameterGuidAndParameterscopeGuidAsync(parameterGuid, parameterscopeGuid)
                ?? throw new KeyNotFoundException($"Parameterscope with Guid '{parameterscopeGuid}' for Parameter Guid '{parameterGuid}' not found.");
            return result.Adapt<ParameterscopeDto>();
        }
        public async Task<string?> GetEffectiveParameterValueAsync(string parameterName, long? companyId, long? officeId)
        {
            return await _repository.ParameterScope.GetEffectiveParameterValueAsync(parameterName, companyId, officeId);
        }
    }
}
