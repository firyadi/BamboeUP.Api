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
    public partial class OrganizationUnitService : IOrganizationUnitService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public OrganizationUnitService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public OrganizationUnitService(
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

        public async Task<IEnumerable<OrganizationUnitDto>> GetAllOrganizationUnitsAsync(bool trackChanges)
        {
            var entities = await _repository.OrganizationUnit.GetAllOrganizationUnitsAsync(trackChanges);
            return entities.Adapt<IEnumerable<OrganizationUnitDto>>();
        }

        public async Task<OrganizationUnitDto> GetOrganizationUnitByGuidAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            var entity = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, trackChanges);
            return entity.Adapt<OrganizationUnitDto>();
        }

        public async Task<OrganizationUnitDto> CreateOrganizationUnitAsync(OrganizationUnitForCreationDto input)
        {
            var model = input.Adapt<OrganizationUnit>();
            model.ParentOrganizationUnitId = input.ParentOrganizationUnitId is null or 0 ? null : input.ParentOrganizationUnitId;
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.OrganizationUnit.CreateOrganizationUnitAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "OrganizationUnit",
                    EntityKey = model.OrganizationUnitGuid.ToString(),
                    EntityDisplayName = model.OrganizationUnitCode,
                    ActionType = "CREATE",
                    OldEntity = null,
                    NewEntity = model
                }
            };

            // Dapper doesn't automatically insert children, so we must manually insert nested Details
            if (input.OrganizationUnitScopes != null && input.OrganizationUnitScopes.Any())
            {
                foreach (var detailDto in input.OrganizationUnitScopes)
                {
                    var detail = detailDto.Adapt<OrganizationUnitScope>();
                    detail.OrganizationUnitId = model.OrganizationUnitId;
                    detail.StatusId = 1;
                    detail.CreatedTime = DateTime.UtcNow;
                    await _repository.OrganizationUnitScope.CreateOrganizationUnitScopeAsync(detail);

                    entries.Add(new AuditLogEntry
                    {
                        TableName = "OrganizationUnitScope",
                        EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                        EntityDisplayName = detail.ScopeType,
                        ParentTableName = "OrganizationUnit",
                        ParentEntityKey = model.OrganizationUnitGuid.ToString(),
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
                RootTableName = "OrganizationUnit",
                RootEntityKey = model.OrganizationUnitGuid.ToString(),
                RootDisplayName = model.OrganizationUnitCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = entries
            });

            return model.Adapt<OrganizationUnitDto>();
        }

        public async Task UpdateOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForUpdateDto input, bool trackChanges)
        {
            var oldOrganizationUnit = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, false)
                ?? throw new KeyNotFoundException($"OrganizationUnit '{organizationUnitGuid}' was not found.");

            var model = input.Adapt<OrganizationUnit>();
            model.OrganizationUnitGuid = organizationUnitGuid;
            model.OrganizationUnitId = oldOrganizationUnit.OrganizationUnitId;
            model.ParentOrganizationUnitId = input.ParentOrganizationUnitId is null or 0 ? null : input.ParentOrganizationUnitId;
            model.StatusId = 2;
            model.UpdatedById = input.UpdatedById;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.OrganizationUnit.UpdateOrganizationUnitAsync(model);

            var entries = new List<AuditLogEntry>
            {
                new()
                {
                    TableName = "OrganizationUnit",
                    EntityKey = model.OrganizationUnitGuid.ToString(),
                    EntityDisplayName = model.OrganizationUnitCode,
                    ActionType = "UPDATE",
                    OldEntity = oldOrganizationUnit,
                    NewEntity = model
                }
            };

            if (input.OrganizationUnitScopes != null)
            {
                var oldOrganizationUnitScopes = (await _repository.OrganizationUnitScope.GetAllByOrganizationUnitGuidAsync(organizationUnitGuid)).ToList();
                var newOrganizationUnitScopes = input.OrganizationUnitScopes.ToList();
                var oldOrganizationUnitScopeDict = oldOrganizationUnitScopes.ToDictionary(o => o.OrganizationUnitScopeGuid);

                foreach (var detailDto in newOrganizationUnitScopes)
                {
                    var detail = detailDto.Adapt<OrganizationUnitScope>();
                    detail.OrganizationUnitId = model.OrganizationUnitId;
                    detail.StatusId = 2;
                    detail.UpdatedTime = DateTime.UtcNow;

                    if (detailDto.OrganizationUnitScopeGuid != Guid.Empty && oldOrganizationUnitScopeDict.ContainsKey(detailDto.OrganizationUnitScopeGuid))
                    {
                        detail.OrganizationUnitScopeGuid = detailDto.OrganizationUnitScopeGuid;
                        detail.UpdatedById = input.UpdatedById;
                        await _repository.OrganizationUnitScope.UpdateOrganizationUnitScopeAsync(detail);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "OrganizationUnitScope",
                            EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                            EntityDisplayName = detail.ScopeType,
                            ParentTableName = "OrganizationUnit",
                            ParentEntityKey = organizationUnitGuid.ToString(),
                            ActionType = "UPDATE",
                            OldEntity = oldOrganizationUnitScopeDict[detailDto.OrganizationUnitScopeGuid],
                            NewEntity = detail
                        });
                    }
                    else
                    {
                        detail.OrganizationUnitScopeGuid = Guid.NewGuid();
                        detail.CreatedById = input.UpdatedById;
                        detail.StatusId = 1;
                        detail.CreatedTime = DateTime.UtcNow;
                        await _repository.OrganizationUnitScope.CreateOrganizationUnitScopeAsync(detail);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "OrganizationUnitScope",
                            EntityKey = detail.OrganizationUnitScopeGuid.ToString(),
                            EntityDisplayName = detail.ScopeType,
                            ParentTableName = "OrganizationUnit",
                            ParentEntityKey = organizationUnitGuid.ToString(),
                            ActionType = "CREATE",
                            OldEntity = null,
                            NewEntity = detail
                        });
                    }
                }

                foreach (var oldOrganizationUnitScope in oldOrganizationUnitScopes)
                {
                    if (!newOrganizationUnitScopes.Any(o => o.OrganizationUnitScopeGuid == oldOrganizationUnitScope.OrganizationUnitScopeGuid))
                    {
                        await _repository.OrganizationUnitScope.SoftDeleteOrganizationUnitScopeAsync(
                            new OrganizationUnitScope
                            {
                                OrganizationUnitScopeGuid = oldOrganizationUnitScope.OrganizationUnitScopeGuid,
                                DeletedById = input.UpdatedById,
                                DeletedTime = DateTime.UtcNow
                            },
                            input.UpdatedById);

                        entries.Add(new AuditLogEntry
                        {
                            TableName = "OrganizationUnitScope",
                            EntityKey = oldOrganizationUnitScope.OrganizationUnitScopeGuid.ToString(),
                            EntityDisplayName = oldOrganizationUnitScope.ScopeType,
                            ParentTableName = "OrganizationUnit",
                            ParentEntityKey = organizationUnitGuid.ToString(),
                            ActionType = "DELETE",
                            OldEntity = oldOrganizationUnitScope,
                            NewEntity = null
                        });
                    }
                }
            }

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "OrganizationUnit",
                RootEntityKey = model.OrganizationUnitGuid.ToString(),
                RootDisplayName = model.OrganizationUnitCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.UpdatedById.ToString(),
                Entries = entries
            });
        }

        public async Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, OrganizationUnitForDeleteDto input, bool trackChanges)
        {
            var oldOrganizationUnit = await _repository.OrganizationUnit.GetOrganizationUnitAsync(organizationUnitGuid, false);
            var model = new OrganizationUnit
            {
                OrganizationUnitGuid = organizationUnitGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.OrganizationUnit.SoftDeleteOrganizationUnitAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "OrganizationUnit",
                RootEntityKey = organizationUnitGuid.ToString(),
                RootDisplayName = oldOrganizationUnit?.OrganizationUnitCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "OrganizationUnit",
                        EntityKey = organizationUnitGuid.ToString(),
                        EntityDisplayName = oldOrganizationUnit?.OrganizationUnitCode,
                        ActionType = "DELETE",
                        OldEntity = oldOrganizationUnit,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteOrganizationUnitByAdminAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            await _repository.OrganizationUnit.DeleteOrganizationUnitAsync(organizationUnitGuid);
        }

        public async Task<IEnumerable<OrganizationUnitDto>> SearchOrganizationUnitAsync(
            string? organizationUnitCode, string? organizationUnitCodeSearchType,
            string? organizationUnitName, string? organizationUnitNameSearchType
, string? parentOrganizationUnitName, string? parentOrganizationUnitNameSearchType

        )
        {
            var data = await _repository.OrganizationUnit.SearchOrganizationUnitAsync(
                organizationUnitCode, organizationUnitCodeSearchType, organizationUnitName, organizationUnitNameSearchType
, parentOrganizationUnitName, parentOrganizationUnitNameSearchType

            );
            return data.Adapt<IEnumerable<OrganizationUnitDto>>();
        }
    }
}
