using Shared.Process;

namespace Service.Modules.Processes.Handlers;

/// <summary>
/// Payroll process steps for PAYROLL_RUN.
/// Mirrors app.usp_PayrollRun phases (VALIDATE → CALCULATE → COMMIT).
/// Full persistence is in SQL; handlers enforce params and emit progress/result rows.
/// </summary>
public sealed class PayrollValidatePeriodStepHandler : IProcessStepHandler
{
    public string HandlerKey => "PayrollValidatePeriodStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Validating payroll period...");

        if (!TryGetRequiredLong(context, "CompanyId", out _))
            return ProcessStepResult.Fail("CompanyId is required.", "PAYROLL_COMPANY_REQUIRED");

        if (!TryGetRequiredInt(context, "PeriodYear", out var year) || year is < 2000 or > 2100)
            return ProcessStepResult.Fail("PeriodYear must be between 2000 and 2100.", "PAYROLL_YEAR_INVALID");

        if (!TryGetRequiredInt(context, "PeriodMonth", out var month) || month is < 1 or > 12)
            return ProcessStepResult.Fail("PeriodMonth must be between 1 and 12.", "PAYROLL_MONTH_INVALID");

        await context.AddMessageAsync(
            ProcessMessageSeverity.Information,
            $"Period {year:D4}-{month:D2} validated. Call app.usp_PayrollRun @Phase='VALIDATE' to persist Draft/Validated run.");

        await context.ReportProgressAsync(1, 1, 0, 0);
        return ProcessStepResult.Ok($"Payroll period {year:D4}-{month:D2} is ready.", 1, 1, 0, 0);
    }

    private static bool TryGetRequiredLong(ProcessStepContext context, string key, out long value)
    {
        value = 0;
        return context.Parameters.TryGetValue(key, out var raw)
            && !string.IsNullOrWhiteSpace(raw)
            && long.TryParse(raw, out value);
    }

    private static bool TryGetRequiredInt(ProcessStepContext context, string key, out int value)
    {
        value = 0;
        return context.Parameters.TryGetValue(key, out var raw)
            && !string.IsNullOrWhiteSpace(raw)
            && int.TryParse(raw, out value);
    }
}

public sealed class PayrollCalculatePayStepHandler : IProcessStepHandler
{
    public string HandlerKey => "PayrollCalculatePayStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Calculating payroll lines (demo)...");

        // Deterministic demo lines — same spirit as app.usp_PayrollRun Demo CTE when Employee is empty.
        var lines = new[]
        {
            ("DEMO-001", "Demo Employee One", 5_500_000m, 750_000m, 200_000m, 100_000m),
            ("DEMO-002", "Demo Employee Two", 6_200_000m, 800_000m, 0m, 150_000m),
            ("DEMO-003", "Demo Employee Three", 4_800_000m, 500_000m, 300_000m, 50_000m),
            ("DEMO-004", "Demo Employee Four", 7_100_000m, 900_000m, 100_000m, 200_000m),
            ("DEMO-005", "Demo Employee Five", 3_900_000m, 400_000m, 0m, 0m),
        };

        var records = new List<ProcessStepRecordResult>();
        var success = 0;
        var warnings = 0;
        var failed = 0;

        foreach (var (empNo, name, baseSalary, allowance, overtime, deduction) in lines)
        {
            var gross = baseSalary + allowance + overtime;
            var tax = Math.Round(gross * 0.05m, 0);
            var net = gross - deduction - tax;

            string severity;
            string message;
            if (baseSalary < 3_000_000m)
            {
                severity = ProcessMessageSeverity.Error;
                message = "Base salary below minimum threshold";
                failed++;
            }
            else if (deduction > baseSalary * 0.3m)
            {
                severity = ProcessMessageSeverity.Warning;
                message = $"Deduction exceeds 30% of base; Net={net:N0}";
                warnings++;
            }
            else
            {
                severity = ProcessMessageSeverity.Information;
                message = $"Gross={gross:N0}, Tax={tax:N0}, Net={net:N0}";
                success++;
            }

            records.Add(new ProcessStepRecordResult
            {
                RecordKey = empNo,
                RecordLabel = name,
                Severity = severity,
                Message = message
            });
        }

        var total = lines.Length;
        await context.ReportProgressAsync(total, success, warnings, failed);
        await context.AddMessageAsync(
            ProcessMessageSeverity.Information,
            "Persist via app.usp_PayrollRun @Phase='CALCULATE' (or Phase='RUN').");

        return new ProcessStepResult
        {
            Success = failed == 0,
            CompletedWithWarnings = warnings > 0,
            Message = $"Calculated {total} payroll lines",
            ProcessedCount = total,
            SuccessCount = success,
            WarningCount = warnings,
            FailedCount = failed,
            ErrorMessage = failed > 0 ? "One or more lines failed calculation." : null,
            ErrorCode = failed > 0 ? "PAYROLL_CALC_FAILED" : null,
            Records = records
        };
    }
}

public sealed class PayrollCommitStepHandler : IProcessStepHandler
{
    public string HandlerKey => "PayrollCommitStep";

    public async Task<ProcessStepResult> ExecuteAsync(ProcessStepContext context, CancellationToken cancellationToken = default)
    {
        var validateOnly = context.Parameters.TryGetValue("ValidateOnly", out var raw)
            && string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase);

        if (validateOnly)
        {
            await context.AddMessageAsync(ProcessMessageSeverity.Information, "Validate-only mode — payroll not posted.");
            return ProcessStepResult.Ok("Validation complete (no post).", 0, 0, 0, 0);
        }

        await context.AddMessageAsync(ProcessMessageSeverity.Information, "Posting payroll run...");
        await context.ReportProgressAsync(5, 5, 0, 0);
        await context.AddMessageAsync(
            ProcessMessageSeverity.Information,
            "Persist via app.usp_PayrollRun @Phase='COMMIT' (or Phase='RUN') to set RunStatus=Posted.");

        return ProcessStepResult.Ok("Payroll posted successfully (demo).", 5, 5, 0, 0);
    }
}
