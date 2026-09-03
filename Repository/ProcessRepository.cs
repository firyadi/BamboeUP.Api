using Contracts;
using Dapper;
using Entities.Models;
using System.Data;

namespace Repository;

public class ProcessRepository : IProcessRepository
{
    private readonly RepositoryContext _context;

    public ProcessRepository(RepositoryContext context) => _context = context;

    public async Task<IEnumerable<ProcessDefinition>> GetActiveDefinitionsAsync(string? processCode = null)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT *
            FROM [core].[ProcessDefinition]
            WHERE IsActive = 1 AND DeletedTime IS NULL AND StatusId > 0
              AND (@processCode IS NULL OR ProcessCode = @processCode)
            ORDER BY Category, ProcessName
            """;
        return await connection.QueryAsync<ProcessDefinition>(sql, new { processCode });
    }

    public async Task<ProcessDefinition?> GetDefinitionByIdAsync(long processDefinitionId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT TOP 1 * FROM [core].[ProcessDefinition]
            WHERE ProcessDefinitionId = @processDefinitionId AND DeletedTime IS NULL
            """;
        return await connection.QuerySingleOrDefaultAsync<ProcessDefinition>(sql, new { processDefinitionId });
    }

    public async Task<ProcessDefinition?> GetDefinitionByGuidAsync(Guid guid)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT TOP 1 * FROM [core].[ProcessDefinition]
            WHERE ProcessDefinitionGuid = @guid AND DeletedTime IS NULL
            """;
        return await connection.QuerySingleOrDefaultAsync<ProcessDefinition>(sql, new { guid });
    }

    public async Task<ProcessDefinition?> GetDefinitionByCodeAsync(string processCode)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT TOP 1 * FROM [core].[ProcessDefinition]
            WHERE ProcessCode = @processCode AND DeletedTime IS NULL
            """;
        return await connection.QuerySingleOrDefaultAsync<ProcessDefinition>(sql, new { processCode });
    }

    public async Task<IEnumerable<ProcessParameterDefinition>> GetParameterDefinitionsAsync(long processDefinitionId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT * FROM [core].[ProcessParameterDefinition]
            WHERE ProcessDefinitionId = @processDefinitionId AND StatusId > 0
            ORDER BY DisplayOrder, ParameterCode
            """;
        return await connection.QueryAsync<ProcessParameterDefinition>(sql, new { processDefinitionId });
    }

    public async Task<IEnumerable<ProcessStepDefinition>> GetStepDefinitionsAsync(long processDefinitionId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT * FROM [core].[ProcessStepDefinition]
            WHERE ProcessDefinitionId = @processDefinitionId AND StatusId > 0
            ORDER BY ExecutionOrder, StepNo
            """;
        return await connection.QueryAsync<ProcessStepDefinition>(sql, new { processDefinitionId });
    }

    public async Task<ProcessExecution?> GetExecutionByGuidAsync(Guid guid)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT TOP 1 * FROM [core].[ProcessExecution] WHERE ProcessExecutionGuid = @guid";
        return await connection.QuerySingleOrDefaultAsync<ProcessExecution>(sql, new { guid });
    }

    public async Task<ProcessExecution?> GetExecutionByCorrelationIdAsync(Guid correlationId)
    {
        using var connection = _context.CreateConnection();
        const string sql = "SELECT TOP 1 * FROM [core].[ProcessExecution] WHERE CorrelationId = @correlationId";
        return await connection.QuerySingleOrDefaultAsync<ProcessExecution>(sql, new { correlationId });
    }

    public async Task<IEnumerable<ProcessExecution>> SearchExecutionsAsync(string? status, string? processCode, DateTime? startedFrom, DateTime? startedTo, long? companyId, int skip, int take)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT e.*
            FROM [core].[ProcessExecution] e
            INNER JOIN [core].[ProcessDefinition] d ON d.ProcessDefinitionId = e.ProcessDefinitionId
            WHERE (@status IS NULL OR e.Status = @status)
              AND (@processCode IS NULL OR d.ProcessCode = @processCode)
              AND (@startedFrom IS NULL OR e.StartedAt >= @startedFrom)
              AND (@startedTo IS NULL OR e.StartedAt <= @startedTo)
              AND (@companyId IS NULL OR e.CompanyId = @companyId)
            ORDER BY e.StartedAt DESC, e.ProcessExecutionId DESC
            OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY
            """;
        return await connection.QueryAsync<ProcessExecution>(sql, new { status, processCode, startedFrom, startedTo, companyId, skip, take });
    }

    public async Task<int> CountExecutionsAsync(string? status, string? processCode, DateTime? startedFrom, DateTime? startedTo, long? companyId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT COUNT(1)
            FROM [core].[ProcessExecution] e
            INNER JOIN [core].[ProcessDefinition] d ON d.ProcessDefinitionId = e.ProcessDefinitionId
            WHERE (@status IS NULL OR e.Status = @status)
              AND (@processCode IS NULL OR d.ProcessCode = @processCode)
              AND (@startedFrom IS NULL OR e.StartedAt >= @startedFrom)
              AND (@startedTo IS NULL OR e.StartedAt <= @startedTo)
              AND (@companyId IS NULL OR e.CompanyId = @companyId)
            """;
        return await connection.ExecuteScalarAsync<int>(sql, new { status, processCode, startedFrom, startedTo, companyId });
    }

    public async Task<long> CreateExecutionAsync(ProcessExecution execution, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            INSERT INTO [core].[ProcessExecution]
            (ProcessExecutionGuid, ProcessDefinitionId, ExecutionNo, Status, StartedAt, CompletedAt,
             StartedById, ProgressPercent, CurrentStepCode, ErrorCount, WarningCount, CorrelationId,
             CompanyId, CompanyOfficeId, ParentExecutionId, CancellationRequestedAt, CreatedById, CreatedTime)
            VALUES
            (@ProcessExecutionGuid, @ProcessDefinitionId, @ExecutionNo, @Status, @StartedAt, @CompletedAt,
             @StartedById, @ProgressPercent, @CurrentStepCode, @ErrorCount, @WarningCount, @CorrelationId,
             @CompanyId, @CompanyOfficeId, @ParentExecutionId, @CancellationRequestedAt, @CreatedById, @CreatedTime);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            """;
        return await conn.ExecuteScalarAsync<long>(sql, execution, transaction);
    }

    public async Task UpdateExecutionAsync(ProcessExecution execution, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            UPDATE [core].[ProcessExecution]
            SET Status = @Status, StartedAt = @StartedAt, CompletedAt = @CompletedAt,
                ProgressPercent = @ProgressPercent, CurrentStepCode = @CurrentStepCode,
                ErrorCount = @ErrorCount, WarningCount = @WarningCount,
                CancellationRequestedAt = @CancellationRequestedAt
            WHERE ProcessExecutionGuid = @ProcessExecutionGuid
            """;
        await conn.ExecuteAsync(sql, execution, transaction);
    }

    public async Task<IEnumerable<ProcessParameterValue>> GetParameterValuesAsync(long processExecutionId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<ProcessParameterValue>(
            "SELECT * FROM [core].[ProcessParameterValue] WHERE ProcessExecutionId = @processExecutionId",
            new { processExecutionId });
    }

    public async Task UpsertParameterValuesAsync(long processExecutionId, IEnumerable<ProcessParameterValue> values, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        foreach (var v in values)
        {
            v.ProcessExecutionId = processExecutionId;
            const string sql = """
                MERGE [core].[ProcessParameterValue] AS t
                USING (SELECT @ProcessExecutionId AS ProcessExecutionId, @ParameterCode AS ParameterCode) AS s
                ON t.ProcessExecutionId = s.ProcessExecutionId AND t.ParameterCode = s.ParameterCode
                WHEN MATCHED THEN UPDATE SET
                    ValueText = @ValueText, ValueNumber = @ValueNumber, ValueDate = @ValueDate,
                    ValueJson = @ValueJson, ValueBlob = @ValueBlob
                WHEN NOT MATCHED THEN INSERT
                    (ProcessExecutionId, ParameterCode, ValueText, ValueNumber, ValueDate, ValueJson, ValueBlob)
                VALUES
                    (@ProcessExecutionId, @ParameterCode, @ValueText, @ValueNumber, @ValueDate, @ValueJson, @ValueBlob);
                """;
            await conn.ExecuteAsync(sql, v, transaction);
        }
    }

    public async Task<IEnumerable<ProcessStepExecution>> GetStepExecutionsAsync(long processExecutionId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<ProcessStepExecution>(
            "SELECT * FROM [core].[ProcessStepExecution] WHERE ProcessExecutionId = @processExecutionId ORDER BY ProcessStepExecutionId",
            new { processExecutionId });
    }

    public async Task UpsertStepExecutionAsync(ProcessStepExecution step, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            MERGE [core].[ProcessStepExecution] AS t
            USING (SELECT @ProcessExecutionId AS ProcessExecutionId, @StepCode AS StepCode) AS s
            ON t.ProcessExecutionId = s.ProcessExecutionId AND t.StepCode = s.StepCode
            WHEN MATCHED THEN UPDATE SET
                StepName = @StepName, Status = @Status, StartedAt = @StartedAt, CompletedAt = @CompletedAt,
                ProgressPercent = @ProgressPercent, ProcessedCount = @ProcessedCount, SuccessCount = @SuccessCount,
                WarningCount = @WarningCount, FailedCount = @FailedCount, Message = @Message,
                ErrorCode = @ErrorCode, ErrorMessage = @ErrorMessage, RetryCount = @RetryCount
            WHEN NOT MATCHED THEN INSERT
                (ProcessExecutionId, StepCode, StepName, Status, StartedAt, CompletedAt, ProgressPercent,
                 ProcessedCount, SuccessCount, WarningCount, FailedCount, Message, ErrorCode, ErrorMessage, RetryCount)
            VALUES
                (@ProcessExecutionId, @StepCode, @StepName, @Status, @StartedAt, @CompletedAt, @ProgressPercent,
                 @ProcessedCount, @SuccessCount, @WarningCount, @FailedCount, @Message, @ErrorCode, @ErrorMessage, @RetryCount);
            """;
        await conn.ExecuteAsync(sql, step, transaction);
    }

    public async Task<IEnumerable<ProcessMessage>> GetMessagesAsync(long processExecutionId, int take = 200)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT TOP (@take) * FROM [core].[ProcessMessage]
            WHERE ProcessExecutionId = @processExecutionId
            ORDER BY CreatedTime DESC
            """;
        return await connection.QueryAsync<ProcessMessage>(sql, new { processExecutionId, take });
    }

    public async Task InsertMessageAsync(ProcessMessage message, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            INSERT INTO [core].[ProcessMessage]
            (ProcessExecutionId, StepCode, Severity, Source, Message, DetailJson, CreatedTime)
            VALUES (@ProcessExecutionId, @StepCode, @Severity, @Source, @Message, @DetailJson, @CreatedTime)
            """;
        await conn.ExecuteAsync(sql, message, transaction);
    }

    public async Task<ProcessResult?> GetResultAsync(long processExecutionId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ProcessResult>(
            "SELECT TOP 1 * FROM [core].[ProcessResult] WHERE ProcessExecutionId = @processExecutionId",
            new { processExecutionId });
    }

    public async Task UpsertResultAsync(ProcessResult result, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            MERGE [core].[ProcessResult] AS t
            USING (SELECT @ProcessExecutionId AS ProcessExecutionId) AS s
            ON t.ProcessExecutionId = s.ProcessExecutionId
            WHEN MATCHED THEN UPDATE SET
                ResultStatus = @ResultStatus, ResultMessage = @ResultMessage,
                TotalRecords = @TotalRecords, SuccessRecords = @SuccessRecords,
                WarningRecords = @WarningRecords, FailedRecords = @FailedRecords,
                ResultReference = @ResultReference, CompletedAt = @CompletedAt
            WHEN NOT MATCHED THEN INSERT
                (ProcessExecutionId, ResultStatus, ResultMessage, TotalRecords, SuccessRecords,
                 WarningRecords, FailedRecords, ResultReference, CompletedAt)
            VALUES
                (@ProcessExecutionId, @ResultStatus, @ResultMessage, @TotalRecords, @SuccessRecords,
                 @WarningRecords, @FailedRecords, @ResultReference, @CompletedAt);
            """;
        await conn.ExecuteAsync(sql, result, transaction);
    }

    public async Task<IEnumerable<ProcessResultDetail>> GetResultDetailsAsync(long processExecutionId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<ProcessResultDetail>(
            "SELECT * FROM [core].[ProcessResultDetail] WHERE ProcessExecutionId = @processExecutionId ORDER BY ProcessResultDetailId",
            new { processExecutionId });
    }

    public async Task InsertResultDetailsAsync(IEnumerable<ProcessResultDetail> details, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            INSERT INTO [core].[ProcessResultDetail]
            (ProcessExecutionId, RecordKey, RecordLabel, Severity, Message, ReferenceId, CreatedTime)
            VALUES (@ProcessExecutionId, @RecordKey, @RecordLabel, @Severity, @Message, @ReferenceId, @CreatedTime)
            """;
        await conn.ExecuteAsync(sql, details, transaction);
    }

    public async Task InsertAuditAsync(ProcessAudit audit, IDbTransaction? transaction = null)
    {
        var conn = transaction?.Connection ?? _context.CreateConnection();
        const string sql = """
            INSERT INTO [core].[ProcessAudit]
            (ProcessExecutionId, Action, StepCode, UserId, ClientIp, DetailJson, CreatedTime)
            VALUES (@ProcessExecutionId, @Action, @StepCode, @UserId, @ClientIp, @DetailJson, @CreatedTime)
            """;
        await conn.ExecuteAsync(sql, audit, transaction);
    }

    public async Task<string> GenerateExecutionNoAsync()
    {
        using var connection = _context.CreateConnection();
        var year = DateTime.UtcNow.Year;
        var prefix = $"PR-{year}-";
        var seq = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) + 1 FROM [core].[ProcessExecution] WHERE ExecutionNo LIKE @prefix + '%'",
            new { prefix });
        return $"{prefix}{seq:D8}";
    }
}
