using Shared.DataTransferObjects;

namespace Service.Contracts.Modules;

public interface IProcessExecutionService
{
    Task<IEnumerable<ProcessDefinitionDto>> GetDefinitionsAsync(string? processCode = null);
    Task<ProcessDefinitionDto?> GetDefinitionAsync(Guid guid);
    Task<ProcessValidationResultDto> ValidateAsync(Guid definitionGuid, ProcessValidateRequestDto input);
    Task<ProcessExecutionDto> ExecuteAsync(Guid definitionGuid, ProcessExecuteRequestDto input, Guid? idempotencyKey = null);
    Task<ProcessExecutionDto?> GetExecutionAsync(Guid guid);
    Task<IEnumerable<ProcessExecutionDto>> SearchExecutionsAsync(ProcessExecutionSearchDto input);
    Task<IEnumerable<ProcessMessageDto>> GetMessagesAsync(Guid guid);
    Task<IEnumerable<ProcessResultDetailDto>> GetResultDetailsAsync(Guid guid);
    Task CancelAsync(Guid guid);
    Task<ProcessExecutionDto> RetryAsync(Guid guid, ProcessRetryRequestDto input);
}
