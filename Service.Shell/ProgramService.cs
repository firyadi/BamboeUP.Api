using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public class ProgramService : IProgramService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public ProgramService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<ProgramDto>> GetAllProgramsAsync(bool trackChanges)
        {
            var entities = await _repository.Program.GetAllProgramsAsync(trackChanges);
            return entities.Adapt<IEnumerable<ProgramDto>>();
        }

        public async Task<ProgramDto> GetProgramByGuidAsync(Guid programGuid, bool trackChanges)
        {
            var entity = await _repository.Program.GetProgramAsync(programGuid, trackChanges);
            return entity.Adapt<ProgramDto>();
        }

        public async Task<ProgramDto> CreateProgramAsync(ProgramForCreationDto input)
        {
            var model = input.Adapt<Programs>();
            await _repository.Program.CreateProgramAsync(model);
            return model.Adapt<ProgramDto>();
        }

        public async Task UpdateProgramAsync(Guid programGuid, ProgramForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<Programs>();
            model.ProgramGuid = programGuid;
            await _repository.Program.UpdateProgramAsync(model);
        }

        public async Task DeleteProgramAsync(Guid programGuid, ProgramForDeleteDto input, bool trackChanges)
        {
            var program = new Programs
            {
                ProgramGuid = programGuid
            };

            await _repository.Program.SoftDeleteProgramAsync(program, input.DeletedById);
        }

        public async Task DeleteProgramByAdminAsync(Guid programGuid, bool trackChanges)
        {
            await _repository.Program.DeleteProgramAsync(programGuid);
        }

        public async Task<IEnumerable<ProgramDto>> SearchProgramAsync(
            string? programName, string? programNameSearchType,
            string? programCode, string? programCodeSearchType)
        {
            var data = await _repository.Program.SearchProgramAsync(
                programName, programNameSearchType, programCode, programCodeSearchType);
            return data.Adapt<IEnumerable<ProgramDto>>();
        }
    }
}
