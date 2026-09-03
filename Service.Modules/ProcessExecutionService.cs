using Contracts;
using Entities.Models;
using Mapster;
using Service.Contracts.Modules;
using Service.Modules.Processes;
using Shared.DataTransferObjects;
using Shared.Process;

namespace Service.Modules;

public class ProcessExecutionService : IProcessExecutionService
{
    private readonly IProcessRepository _processRepository;
    private readonly IProcessOrchestrator _orchestrator;
    private readonly IUserContext _userContext;

    public ProcessExecutionService(
        IProcessRepository processRepository,
        IProcessOrchestrator orchestrator,
        IUserContext userContext)
    {
        _processRepository = processRepository;
        _orchestrator = orchestrator;
        _userContext = userContext;
    }

    public async Task<IEnumerable<ProcessDefinitionDto>> GetDefinitionsAsync(string? processCode = null)
    {
        var defs = await _processRepository.GetActiveDefinitionsAsync(processCode);
        var result = new List<ProcessDefinitionDto>();
        foreach (var def in defs)
        {
            var dto = def.Adapt<ProcessDefinitionDto>();
            dto.Parameters = (await _processRepository.GetParameterDefinitionsAsync(def.ProcessDefinitionId))
                .Adapt<List<ProcessParameterDefinitionDto>>();
            dto.Steps = (await _processRepository.GetStepDefinitionsAsync(def.ProcessDefinitionId))
                .Adapt<List<ProcessStepDefinitionDto>>();
            result.Add(dto);
        }
        return result;
    }

    public async Task<ProcessDefinitionDto?> GetDefinitionAsync(Guid guid)
    {
        var def = await _processRepository.GetDefinitionByGuidAsync(guid);
        if (def is null) return null;
        var dto = def.Adapt<ProcessDefinitionDto>();
        dto.Parameters = (await _processRepository.GetParameterDefinitionsAsync(def.ProcessDefinitionId))
            .Adapt<List<ProcessParameterDefinitionDto>>();
        dto.Steps = (await _processRepository.GetStepDefinitionsAsync(def.ProcessDefinitionId))
            .Adapt<List<ProcessStepDefinitionDto>>();
        return dto;
    }

    public async Task<ProcessValidationResultDto> ValidateAsync(Guid definitionGuid, ProcessValidateRequestDto input)
    {
        var def = await _processRepository.GetDefinitionByGuidAsync(definitionGuid)
            ?? throw new KeyNotFoundException("Process definition not found");

        var paramDefs = (await _processRepository.GetParameterDefinitionsAsync(def.ProcessDefinitionId)).ToList();
        var messages = new List<ProcessValidationMessageDto>();
        var values = input.Parameters.ToDictionary(p => p.ParameterCode, StringComparer.OrdinalIgnoreCase);

        foreach (var p in paramDefs.Where(x => x.IsRequired))
        {
            if (!values.TryGetValue(p.ParameterCode, out var val) || string.IsNullOrWhiteSpace(val?.ValueText))
            {
                messages.Add(new ProcessValidationMessageDto
                {
                    Severity = ProcessMessageSeverity.Error,
                    ParameterCode = p.ParameterCode,
                    Message = $"{p.ParameterName} is required."
                });
            }
        }

        return new ProcessValidationResultDto
        {
            IsValid = messages.Count == 0,
            Messages = messages
        };
    }

    public async Task<ProcessExecutionDto> ExecuteAsync(Guid definitionGuid, ProcessExecuteRequestDto input, Guid? idempotencyKey = null)
    {
        var def = await _processRepository.GetDefinitionByGuidAsync(definitionGuid)
            ?? throw new KeyNotFoundException("Process definition not found");

        if (idempotencyKey.HasValue)
        {
            var existing = await _processRepository.GetExecutionByCorrelationIdAsync(idempotencyKey.Value);
            if (existing is not null)
                return (await GetExecutionAsync(existing.ProcessExecutionGuid))!;
        }

        var correlationId = idempotencyKey ?? input.CorrelationId ?? Guid.NewGuid();
        var executionNo = await _processRepository.GenerateExecutionNoAsync();
        var isSync = def.ExecutionMode == ProcessExecutionMode.Sync;

        var execution = new ProcessExecution
        {
            ProcessExecutionGuid = Guid.NewGuid(),
            ProcessDefinitionId = def.ProcessDefinitionId,
            ExecutionNo = executionNo,
            Status = isSync ? ProcessExecutionStatus.Running : ProcessExecutionStatus.Queued,
            StartedAt = isSync ? DateTime.UtcNow : null,
            StartedById = _userContext.UserId,
            CreatedById = _userContext.UserId,
            CorrelationId = correlationId,
            CompanyId = input.CompanyId ?? _userContext.CompanyId,
            CompanyOfficeId = input.CompanyOfficeId ?? _userContext.OfficeId
        };

        execution.ProcessExecutionId = await _processRepository.CreateExecutionAsync(execution);
        await SaveParametersAsync(execution.ProcessExecutionId, input.Parameters);
        await _processRepository.InsertAuditAsync(new ProcessAudit
        {
            ProcessExecutionId = execution.ProcessExecutionId,
            Action = "Execute",
            UserId = _userContext.UserId,
            CreatedTime = DateTime.UtcNow
        });

        if (isSync)
            await _orchestrator.RunExecutionAsync(execution.ProcessExecutionGuid);
        // Async/Background: remain Queued for ProcessExecutionWorker pickup.

        return (await GetExecutionAsync(execution.ProcessExecutionGuid))!;
    }

    public async Task<ProcessExecutionDto?> GetExecutionAsync(Guid guid)
    {
        var execution = await _processRepository.GetExecutionByGuidAsync(guid);
        if (execution is null) return null;

        var def = await _processRepository.GetDefinitionByIdAsync(execution.ProcessDefinitionId);

        var dto = execution.Adapt<ProcessExecutionDto>();
        dto.ProcessDefinitionGuid = def?.ProcessDefinitionGuid ?? Guid.Empty;
        dto.ProcessCode = def?.ProcessCode ?? string.Empty;
        dto.ProcessName = def?.ProcessName ?? string.Empty;
        dto.Parameters = (await _processRepository.GetParameterValuesAsync(execution.ProcessExecutionId))
            .Adapt<List<ProcessParameterValueDto>>();
        dto.Steps = (await _processRepository.GetStepExecutionsAsync(execution.ProcessExecutionId))
            .Adapt<List<ProcessStepExecutionDto>>();
        dto.Result = (await _processRepository.GetResultAsync(execution.ProcessExecutionId))?.Adapt<ProcessResultDto>();
        return dto;
    }

    public async Task<IEnumerable<ProcessExecutionDto>> SearchExecutionsAsync(ProcessExecutionSearchDto input)
    {
        var skip = Math.Max(0, (input.Page - 1) * input.PageSize);
        var items = await _processRepository.SearchExecutionsAsync(
            input.Status, input.ProcessCode, input.StartedFrom, input.StartedTo, input.CompanyId, skip, input.PageSize);

        var result = new List<ProcessExecutionDto>();
        foreach (var item in items)
        {
            var mapped = await GetExecutionAsync(item.ProcessExecutionGuid);
            if (mapped is not null) result.Add(mapped);
        }
        return result;
    }

    public async Task<IEnumerable<ProcessMessageDto>> GetMessagesAsync(Guid guid)
    {
        var execution = await _processRepository.GetExecutionByGuidAsync(guid)
            ?? throw new KeyNotFoundException("Execution not found");
        return (await _processRepository.GetMessagesAsync(execution.ProcessExecutionId))
            .Adapt<IEnumerable<ProcessMessageDto>>();
    }

    public async Task<IEnumerable<ProcessResultDetailDto>> GetResultDetailsAsync(Guid guid)
    {
        var execution = await _processRepository.GetExecutionByGuidAsync(guid)
            ?? throw new KeyNotFoundException("Execution not found");
        return (await _processRepository.GetResultDetailsAsync(execution.ProcessExecutionId))
            .Adapt<IEnumerable<ProcessResultDetailDto>>();
    }

    public async Task CancelAsync(Guid guid)
    {
        var execution = await _processRepository.GetExecutionByGuidAsync(guid)
            ?? throw new KeyNotFoundException("Execution not found");

        if (!ProcessExecutionStateMachine.CanTransition(execution.Status, ProcessExecutionStatus.Stopping))
            throw new InvalidOperationException($"Cannot cancel execution in status {execution.Status}");

        execution.CancellationRequestedAt = DateTime.UtcNow;
        ProcessExecutionStateMachine.EnsureTransition(execution.Status, ProcessExecutionStatus.Stopping);
        execution.Status = ProcessExecutionStatus.Stopping;
        await _processRepository.UpdateExecutionAsync(execution);
    }

    public async Task<ProcessExecutionDto> RetryAsync(Guid guid, ProcessRetryRequestDto input)
    {
        var source = await _processRepository.GetExecutionByGuidAsync(guid)
            ?? throw new KeyNotFoundException("Execution not found");

        var def = await _processRepository.GetDefinitionByIdAsync(source.ProcessDefinitionId)
            ?? throw new KeyNotFoundException("Process definition not found");

        if (!def.AllowRetry)
            throw new InvalidOperationException("Retry is not allowed for this process.");

        var parameters = (await _processRepository.GetParameterValuesAsync(source.ProcessExecutionId))
            .Adapt<List<ProcessParameterValueDto>>();

        return await ExecuteAsync(def.ProcessDefinitionGuid, new ProcessExecuteRequestDto
        {
            Parameters = parameters,
            CompanyId = source.CompanyId,
            CompanyOfficeId = source.CompanyOfficeId
        });
    }

    private async Task SaveParametersAsync(long executionId, IEnumerable<ProcessParameterValueDto> parameters)
    {
        var entities = parameters.Select(p => new ProcessParameterValue
        {
            ProcessExecutionId = executionId,
            ParameterCode = p.ParameterCode,
            ValueText = p.ValueText,
            ValueNumber = p.ValueNumber,
            ValueDate = p.ValueDate,
            ValueJson = p.ValueJson
        });
        await _processRepository.UpsertParameterValuesAsync(executionId, entities);
    }
}
