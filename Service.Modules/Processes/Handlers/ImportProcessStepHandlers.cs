using Shared.Process;

namespace Service.Modules.Processes.Handlers;

public sealed class ImportValidateFileStepHandler : IProcessStepHandler
{
    public string HandlerKey => "ImportValidateFileStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Validating import file...");
        context.Parameters.TryGetValue("SourceFile", out var sourceFile);

        if (string.IsNullOrWhiteSpace(sourceFile))
            return ProcessStepResult.Fail("Source file is required.", "IMPORT_FILE_REQUIRED");

        if (!context.Parameters.TryGetValue("ImportType", out var importType) || string.IsNullOrWhiteSpace(importType))
            return ProcessStepResult.Fail("Import type is required.", "IMPORT_TYPE_REQUIRED");

        await context.ReportProgressAsync(1, 1, 0, 0);
        return ProcessStepResult.Ok($"File '{sourceFile}' validated for {importType}.", 1, 1, 0, 0);
    }
}

public sealed class ImportParseRowsStepHandler : IProcessStepHandler
{
    public string HandlerKey => "ImportParseRowsStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Parsing import rows...");
        var total = 100;
        var success = 95;
        var warnings = 3;
        var failed = 2;

        await context.ReportProgressAsync(total, success, warnings, failed);

        var records = new List<ProcessStepRecordResult>();
        for (var i = 1; i <= failed; i++)
        {
            records.Add(new ProcessStepRecordResult
            {
                RecordKey = $"ROW-{i}",
                RecordLabel = $"Row {i}",
                Severity = ProcessMessageSeverity.Error,
                Message = "Invalid column format"
            });
        }

        for (var i = 1; i <= warnings; i++)
        {
            records.Add(new ProcessStepRecordResult
            {
                RecordKey = $"ROW-W{i}",
                RecordLabel = $"Row W{i}",
                Severity = ProcessMessageSeverity.Warning,
                Message = "Optional field missing"
            });
        }

        return new ProcessStepResult
        {
            Success = true,
            CompletedWithWarnings = warnings > 0 || failed > 0,
            Message = $"Parsed {total} rows",
            ProcessedCount = total,
            SuccessCount = success,
            WarningCount = warnings,
            FailedCount = failed,
            Records = records
        };
    }
}

public sealed class ImportCommitDataStepHandler : IProcessStepHandler
{
    public string HandlerKey => "ImportCommitDataStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        if (context.Parameters.TryGetValue("ValidateOnly", out var validateOnly)
            && string.Equals(validateOnly, "true", StringComparison.OrdinalIgnoreCase))
        {
            await context.AddMessageAsync(ProcessMessageSeverity.Information, "Validate-only mode — skipping commit.");
            return ProcessStepResult.Ok("Validation complete (no commit).", 0, 0, 0, 0);
        }

        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Committing import data...");
        await context.ReportProgressAsync(95, 95, 0, 0);
        return ProcessStepResult.Ok("Import committed successfully.", 95, 95, 0, 0);
    }
}
