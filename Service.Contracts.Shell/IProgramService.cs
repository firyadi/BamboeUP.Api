using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IProgramService
    {
        Task<IEnumerable<ProgramDto>> GetAllProgramsAsync(bool trackChanges);
        Task<ProgramDto> GetProgramByGuidAsync(Guid programGuid, bool trackChanges);
        Task<ProgramDto> CreateProgramAsync(ProgramForCreationDto input);
        Task UpdateProgramAsync(Guid programGuid, ProgramForUpdateDto input, bool trackChanges);
        Task DeleteProgramAsync(Guid programGuid, ProgramForDeleteDto input, bool trackChanges);
        Task DeleteProgramByAdminAsync(Guid programGuid, bool trackChanges);
        Task<IEnumerable<ProgramDto>> SearchProgramAsync(
            string? programName, string? programNameSearchType,
            string? programCode, string? programCodeSearchType);
    }
}
