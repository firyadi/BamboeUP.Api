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
    public partial class PersonWorkExperienceService : IPersonWorkExperienceService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public PersonWorkExperienceService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public PersonWorkExperienceService(
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

        public async Task<IEnumerable<PersonWorkExperienceDto>> GetAllPersonWorkExperiencesAsync(bool trackChanges)
        {
            var entities = await _repository.PersonWorkExperience.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<PersonWorkExperienceDto>>();
        }

        public async Task<PersonWorkExperienceDto?> GetPersonWorkExperienceByGuidAsync(Guid personWorkExperienceGuid, bool trackChanges)
        {
            var entity = await _repository.PersonWorkExperience.GetPersonWorkExperienceAsync(personWorkExperienceGuid, trackChanges);
            if (entity == null) return null;
            return entity.Adapt<PersonWorkExperienceDto>();
        }

        public async Task<PersonWorkExperienceDto> CreatePersonWorkExperienceAsync(Guid personGuid, PersonWorkExperienceForCreationDto input)
        {
            var model = input.Adapt<PersonWorkExperience>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.PersonWorkExperience.CreatePersonWorkExperienceAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "PersonWorkExperience",
                RootEntityKey = model.PersonWorkExperienceGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonWorkExperience",
                        EntityKey = model.PersonWorkExperienceGuid.ToString(),
                        EntityDisplayName = model.CompanyName,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<PersonWorkExperienceDto>();
        }

        public async Task UpdatePersonWorkExperienceAsync(Guid personGuid, Guid personWorkExperienceGuid, PersonWorkExperienceForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonWorkExperience.GetPersonWorkExperienceAsync(personWorkExperienceGuid, false);

            var model = input.Adapt<PersonWorkExperience>();
            model.PersonWorkExperienceGuid = personWorkExperienceGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.PersonWorkExperience.UpdatePersonWorkExperienceAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "PersonWorkExperience",
                RootEntityKey = model.PersonWorkExperienceGuid.ToString(),
                RootDisplayName = model.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonWorkExperience",
                        EntityKey = model.PersonWorkExperienceGuid.ToString(),
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

        public async Task DeletePersonWorkExperienceAsync(Guid personGuid, Guid personWorkExperienceGuid, PersonWorkExperienceForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.PersonWorkExperience.GetPersonWorkExperienceAsync(personWorkExperienceGuid, false);
            var model = new PersonWorkExperience
            {
                PersonWorkExperienceGuid = personWorkExperienceGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.PersonWorkExperience.SoftDeletePersonWorkExperienceAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "PersonWorkExperience",
                RootEntityKey = personWorkExperienceGuid.ToString(),
                RootDisplayName = oldDetail?.CompanyName,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "PersonWorkExperience",
                        EntityKey = personWorkExperienceGuid.ToString(),
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

        public async Task DeletePersonWorkExperienceByAdminAsync(Guid personWorkExperienceGuid, bool trackChanges)
        {
            await _repository.PersonWorkExperience.DeletePersonWorkExperienceAsync(personWorkExperienceGuid);
        }

        public async Task<IEnumerable<PersonWorkExperienceDto>> SearchPersonWorkExperienceAsync(
            string? srIndustry, string? srIndustrySearchType, string? srEmploymentType, string? srEmploymentTypeSearchType, string? companyName, string? companyNameSearchType, string? jobTitle, string? jobTitleSearchType, string? department, string? departmentSearchType, string? location, string? locationSearchType, string? supervisor, string? supervisorSearchType, string? jobDescription, string? jobDescriptionSearchType, string? startDate, string? startDateSearchType, string? endDate, string? endDateSearchType, string? isCurrentEmployment, string? isCurrentEmploymentSearchType, string? lastSalary, string? lastSalarySearchType, string? reasonforLeaving, string? reasonforLeavingSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personWorkExperienceGuid)
        {
            var data = await _repository.PersonWorkExperience.SearchPersonWorkExperienceAsync(
                srIndustry, srIndustrySearchType, srEmploymentType, srEmploymentTypeSearchType, companyName, companyNameSearchType, jobTitle, jobTitleSearchType, department, departmentSearchType, location, locationSearchType, supervisor, supervisorSearchType, jobDescription, jobDescriptionSearchType, startDate, startDateSearchType, endDate, endDateSearchType, isCurrentEmployment, isCurrentEmploymentSearchType, lastSalary, lastSalarySearchType, reasonforLeaving, reasonforLeavingSearchType, remarks, remarksSearchType, 
                personGuid, personWorkExperienceGuid);
            return data.Adapt<IEnumerable<PersonWorkExperienceDto>>();
        }

        public async Task<IEnumerable<PersonWorkExperienceDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.PersonWorkExperience.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<PersonWorkExperienceDto>>();
        }

        public async Task<PersonWorkExperienceDto?> GetByPersonGuidAndPersonWorkExperienceGuidAsync(Guid personGuid, Guid personWorkExperienceGuid)
        {
            var result = await _repository.PersonWorkExperience.GetByPersonGuidAndPersonWorkExperienceGuidAsync(personGuid, personWorkExperienceGuid);
            if (result == null) return null;
            return result.Adapt<PersonWorkExperienceDto>();
        }
    }
}
