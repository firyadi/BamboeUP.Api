using Entities.Models;
using System.Data;

namespace Contracts;

public interface IProcessRepository
{
    Task<IEnumerable<ProcessDefinition>> GetActiveDefinitionsAsync(string? processCode = null);
    Task<ProcessDefinition?> GetDefinitionByIdAsync(long processDefinitionId);
    Task<ProcessDefinition?> GetDefinitionByGuidAsync(Guid guid);
    Task<ProcessDefinition?> GetDefinitionByCodeAsync(string processCode);
    Task<IEnumerable<ProcessParameterDefinition>> GetParameterDefinitionsAsync(long processDefinitionId);
    Task<IEnumerable<ProcessStepDefinition>> GetStepDefinitionsAsync(long processDefinitionId);

    Task<ProcessExecution?> GetExecutionByGuidAsync(Guid guid);
    Task<ProcessExecution?> GetExecutionByCorrelationIdAsync(Guid correlationId);
    Task<IEnumerable<ProcessExecution>> SearchExecutionsAsync(string? status, string? processCode, DateTime? startedFrom, DateTime? startedTo, long? companyId, int skip, int take);
    Task<int> CountExecutionsAsync(string? status, string? processCode, DateTime? startedFrom, DateTime? startedTo, long? companyId);

    Task<long> CreateExecutionAsync(ProcessExecution execution, IDbTransaction? transaction = null);
    Task UpdateExecutionAsync(ProcessExecution execution, IDbTransaction? transaction = null);

    Task<IEnumerable<ProcessParameterValue>> GetParameterValuesAsync(long processExecutionId);
    Task UpsertParameterValuesAsync(long processExecutionId, IEnumerable<ProcessParameterValue> values, IDbTransaction? transaction = null);

    Task<IEnumerable<ProcessStepExecution>> GetStepExecutionsAsync(long processExecutionId);
    Task UpsertStepExecutionAsync(ProcessStepExecution step, IDbTransaction? transaction = null);

    Task<IEnumerable<ProcessMessage>> GetMessagesAsync(long processExecutionId, int take = 200);
    Task InsertMessageAsync(ProcessMessage message, IDbTransaction? transaction = null);

    Task<ProcessResult?> GetResultAsync(long processExecutionId);
    Task UpsertResultAsync(ProcessResult result, IDbTransaction? transaction = null);

    Task<IEnumerable<ProcessResultDetail>> GetResultDetailsAsync(long processExecutionId);
    Task InsertResultDetailsAsync(IEnumerable<ProcessResultDetail> details, IDbTransaction? transaction = null);

    Task InsertAuditAsync(ProcessAudit audit, IDbTransaction? transaction = null);
    Task<string> GenerateExecutionNoAsync();
}
