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
    public partial class OrganizationUnitScopeService : IOrganizationUnitScopeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public OrganizationUnitScopeService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public OrganizationUnitScopeService(
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

        public async Task<IEnumerable<OrganizationUnitScopeDto>> GetAllOrganizationUnitScopesAsync(bool trackChanges)
        {
            var entities = await _repository.OrganizationUnitScope.GetAllByOrganizationUnitGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<OrganizationUnitScopeDto>>();
        }

        public async Task<OrganizationUnitScopeDto> GetOrganizationUnitScopeByGuidAsync(Guid organizationUnitScopeGuid, bool trackChanges)
        {
            var entity = await _repository.OrganizationUnitScope.GetOrganizationUnitScopeAsync(organizationUnitScopeGuid, trackChanges);
            return entity.Adapt<OrganizationUnitScopeDto>();
        }

        public async Task<OrganizationUnitScopeDto> CreateOrganizationUnitScopeAsync(Guid organizationUnitGuid, OrganizationUnitScopeForCreationDto input)
        {
            var model = input.Adapt<OrganizationUnitScope>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.OrganizationUnitScope.CreateOrganizationUnitScopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "OrganizationUnitScope",
                RootEntityKey = model.OrganizationUnitScopeGuid.ToString(),
                RootDisplayName = model.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "OrganizationUnitScope",
                        EntityKey = model.OrganizationUnitScopeGuid.ToString(),
                        EntityDisplayName = model.ScopeType,
                        ParentTableName = "OrganizationUnit",
                        ParentEntityKey = organizationUnitGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<OrganizationUnitScopeDto>();
        }

        public async Task UpdateOrganizationUnitScopeAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid, OrganizationUnitScopeForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.OrganizationUnitScope.GetOrganizationUnitScopeAsync(organizationUnitScopeGuid, false);

            var model = input.Adapt<OrganizationUnitScope>();
            model.OrganizationUnitScopeGuid = organizationUnitScopeGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.OrganizationUnitScope.UpdateOrganizationUnitScopeAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "OrganizationUnitScope",
                RootEntityKey = model.OrganizationUnitScopeGuid.ToString(),
                RootDisplayName = model.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "OrganizationUnitScope",
                        EntityKey = model.OrganizationUnitScopeGuid.ToString(),
                        EntityDisplayName = model.ScopeType,
                        ParentTableName = "OrganizationUnit",
                        ParentEntityKey = organizationUnitGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteOrganizationUnitScopeAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid, OrganizationUnitScopeForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.OrganizationUnitScope.GetOrganizationUnitScopeAsync(organizationUnitScopeGuid, false);
            var model = new OrganizationUnitScope
            {
                OrganizationUnitScopeGuid = organizationUnitScopeGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.OrganizationUnitScope.SoftDeleteOrganizationUnitScopeAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "OrganizationUnitScope",
                RootEntityKey = organizationUnitScopeGuid.ToString(),
                RootDisplayName = oldDetail?.ScopeType,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "OrganizationUnitScope",
                        EntityKey = organizationUnitScopeGuid.ToString(),
                        EntityDisplayName = oldDetail?.ScopeType,
                        ParentTableName = "OrganizationUnit",
                        ParentEntityKey = organizationUnitGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteOrganizationUnitScopeByAdminAsync(Guid organizationUnitScopeGuid, bool trackChanges)
        {
            await _repository.OrganizationUnitScope.DeleteOrganizationUnitScopeAsync(organizationUnitScopeGuid);
        }

        public async Task<IEnumerable<OrganizationUnitScopeDto>> SearchOrganizationUnitScopeAsync(
            string? scopeType, string? scopeTypeSearchType,
            Guid organizationUnitGuid, Guid organizationUnitScopeGuid)
        {
            var data = await _repository.OrganizationUnitScope.SearchOrganizationUnitScopeAsync(
                scopeType, scopeTypeSearchType, 
                organizationUnitGuid, organizationUnitScopeGuid);
            return data.Adapt<IEnumerable<OrganizationUnitScopeDto>>();
        }

        public async Task<IEnumerable<OrganizationUnitScopeDto>> GetAllByOrganizationUnitGuidAsync(Guid organizationUnitGuid)
        {
            var result = await _repository.OrganizationUnitScope.GetAllByOrganizationUnitGuidAsync(organizationUnitGuid);
            return result.Adapt<IEnumerable<OrganizationUnitScopeDto>>();
        }

        public async Task<OrganizationUnitScopeDto> GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(Guid organizationUnitGuid, Guid organizationUnitScopeGuid)
        {
            var result = await _repository.OrganizationUnitScope.GetByOrganizationUnitGuidAndOrganizationUnitScopeGuidAsync(organizationUnitGuid, organizationUnitScopeGuid);
            return result.Adapt<OrganizationUnitScopeDto>();
        }
    }
}
