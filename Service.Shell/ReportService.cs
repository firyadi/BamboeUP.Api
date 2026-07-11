using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Handlers;
using BamboeUp.Report.Services;
using Contracts;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using System.Diagnostics;
using System.Text.Json;

namespace Service.Shell
{
    public class ReportService : IReportService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly StubReportHandler _reportHandler = new();
        private readonly StubPivotHandler _pivotHandler = new();

        public ReportService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportProgramDto>> GetAllowedReportsAsync(
            string reportType,
            Guid userGuid,
            string? companyId,
            string? officeId,
            bool isAdmin)
        {
            var normalized = NormalizeReportType(reportType);
            if (isAdmin)
                return await _repository.Report.GetAllReportsByTypeAsync(normalized);

            return await _repository.Report.GetAllowedReportsAsync(userGuid, companyId, officeId, normalized);
        }

        public async Task<ReportParameterSchemaDto> GetReportParameterSchemaAsync(
            long programId,
            long? companyId,
            long? officeId,
            string reportKind,
            string? companyName = null,
            string? officeName = null)
        {
            var normalized = NormalizeReportType(reportKind);
            var definition = await _repository.Report.ResolveDefinitionAsync(programId, companyId, normalized);
            if (definition == null)
            {
                return new ReportParameterSchemaDto
                {
                    SystemContext = BuildSystemContext(companyId, officeId, companyName, officeName)
                };
            }

            var fields = (await _repository.Report.GetParametersAsync(definition.ReportDefinitionId)).ToList();
            var layoutColumns = fields.Count == 0
                ? (byte)3
                : (byte)Math.Clamp(fields.Max(f => (int)f.ColumnGroup), 1, 3);

            return new ReportParameterSchemaDto
            {
                ReportDefinitionId = definition.ReportDefinitionId,
                DefinitionKey = definition.DefinitionKey,
                LayoutColumns = layoutColumns,
                RequiresPrintId = definition.RequiresPrintId,
                SystemContext = BuildSystemContext(companyId, officeId, companyName, officeName),
                Fields = fields
            };
        }

        public async Task<IEnumerable<ReportLookupItemDto>> LookupAsync(
            string lookupType,
            string? query,
            long? companyId,
            long? officeId,
            string? lookupConfig,
            int take)
        {
            var normalized = (lookupType ?? string.Empty).Trim();

            if (normalized.Equals("StandardReference", StringComparison.OrdinalIgnoreCase))
                return await LookupStandardReferenceAsync(query, companyId, officeId, lookupConfig, take);

            return await _repository.Report.LookupAsync(normalized, query, companyId, officeId, lookupConfig, take);
        }

        private async Task<IEnumerable<ReportLookupItemDto>> LookupStandardReferenceAsync(
            string? query,
            long? companyId,
            long? officeId,
            string? lookupConfig,
            int take)
        {
            if (!companyId.HasValue || !officeId.HasValue)
                return Array.Empty<ReportLookupItemDto>();

            var initial = string.IsNullOrWhiteSpace(lookupConfig) ? null : lookupConfig.Trim();
            if (string.IsNullOrWhiteSpace(initial))
                return Array.Empty<ReportLookupItemDto>();

            var items = await _repository.StandardReferenceItemDisplay.GetItemsAsync(
                companyId.Value,
                officeId.Value,
                null,
                initial);

            var q = string.IsNullOrWhiteSpace(query) ? null : query.Trim();

            return items
                .Where(i => q == null ||
                            (i.StandardReferenceItemName?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (i.StandardReferenceItemInitial?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false))
                .Take(Math.Clamp(take, 1, 50))
                .Select(i => new ReportLookupItemDto
                {
                    Id = i.StandardReferenceItemId.ToString(),
                    Label = $"{i.StandardReferenceItemName} ({i.StandardReferenceItemInitial})"
                })
                .ToList();
        }

        private static ReportSystemContextDto BuildSystemContext(
            long? companyId, long? officeId, string? companyName, string? officeName)
            => new()
            {
                CompanyId = companyId,
                CompanyName = companyName,
                OfficeId = officeId,
                OfficeName = officeName
            };

        public async Task<ReportRunResultDto> RunReportAsync(
            ReportRunRequestDto request,
            Guid userGuid,
            long userId,
            string? companyId,
            string? officeId,
            bool isAdmin)
        {
            var sw = Stopwatch.StartNew();
            var reportKind = NormalizeReportType(request.ReportKind);
            var companyIdLong = request.CompanyId ?? ParseLong(companyId);
            var officeIdLong = request.CompanyOfficeId ?? ParseLong(officeId);

            var allowed = (await GetAllowedReportsAsync(reportKind, userGuid, companyId, officeId, isAdmin))
                .Any(r => r.ProgramId == request.ProgramId);
            if (!allowed)
                throw new UnauthorizedAccessException("You do not have permission to run this report.");

            var definition = await _repository.Report.ResolveDefinitionAsync(request.ProgramId, companyIdLong, reportKind);
            var schemaFields = definition == null
                ? new List<ReportParameterDefinitionDto>()
                : (await _repository.Report.GetParametersAsync(definition.ReportDefinitionId)).ToList();

            var normalizedParameters = ReportParameterValidator.ValidateAndNormalize(request, schemaFields);

            var executionGuid = Guid.NewGuid();
            string? printId = null;
            string? printIdMasked = null;

            if (definition?.RequiresPrintId == true)
            {
                if (request.IsReprint && string.IsNullOrWhiteSpace(request.ReprintOfPrintId))
                    throw new ArgumentException("Original Print ID is required for reprint.");
                if (request.IsReprint && string.IsNullOrWhiteSpace(request.ReprintReason))
                    throw new ArgumentException("Reprint reason is required.");

                var prefix = string.IsNullOrWhiteSpace(definition.PrintIdPrefix) ? "RP" : definition.PrintIdPrefix!;
                var period = request.DateFrom?.ToString("yyyyMM")
                             ?? (normalizedParameters.TryGetValue("DateFrom", out var df) && DateTime.TryParse(df, out var dfrom) ? dfrom.ToString("yyyyMM") : DateTime.UtcNow.ToString("yyyyMM"));
                (printId, printIdMasked) = ReportPrintIdGenerator.Generate(prefix, period);
            }

            var maskedLogParameters = ReportParameterValidator.MaskForLog(normalizedParameters, schemaFields);
            var parametersJson = JsonSerializer.Serialize(maskedLogParameters);

            await _repository.Report.InsertExecutionLogAsync(new ReportExecutionLogInsert
            {
                ReportExecutionGuid = executionGuid,
                ProgramId = request.ProgramId,
                ReportDefinitionId = definition?.ReportDefinitionId,
                UserId = userId,
                CompanyId = companyIdLong,
                CompanyOfficeId = officeIdLong,
                ReportKind = reportKind,
                ParametersJson = parametersJson,
                ReportPrintId = printId,
                ReportPrintIdMasked = printIdMasked,
                IsReprint = request.IsReprint,
                ReprintOfPrintId = request.ReprintOfPrintId,
                ReprintReason = request.ReprintReason,
                Status = "Running",
                OutputFormat = reportKind == "PVT" ? "Pivot" : "PDF"
            });

            try
            {
                var context = new ReportRunContext
                {
                    ProgramId = request.ProgramId,
                    ProgramCode = request.ProgramCode,
                    ReportKind = reportKind,
                    ReportDefinitionId = definition?.ReportDefinitionId,
                    DefinitionKey = definition?.DefinitionKey,
                    FilePath = definition?.FilePath,
                    StoreProcedureName = definition?.StoreProcedureName,
                    UserId = userId,
                    CompanyId = companyIdLong,
                    CompanyOfficeId = officeIdLong,
                    Parameters = BuildRunParameters(request, normalizedParameters),
                    Print = new ReportPrintContext
                    {
                        RequiresPrintId = definition?.RequiresPrintId == true,
                        ReportPrintId = printId,
                        ReportPrintIdMasked = printIdMasked,
                        IsReprint = request.IsReprint,
                        ReprintOfPrintId = request.ReprintOfPrintId,
                        ReprintReason = request.ReprintReason
                    }
                };

                var handler = reportKind == "PVT" ? (IReportHandler)_pivotHandler : _reportHandler;
                var handlerResult = await handler.RunAsync(context);

                await _repository.Report.UpdateExecutionLogAsync(new ReportExecutionLogUpdate
                {
                    ReportExecutionGuid = executionGuid,
                    Status = handlerResult.Success ? "Completed" : "Failed",
                    DurationMs = (int)sw.ElapsedMilliseconds,
                    ErrorMessage = handlerResult.Success ? null : handlerResult.Message
                });

                return new ReportRunResultDto
                {
                    Success = handlerResult.Success,
                    Message = handlerResult.Message,
                    ReportExecutionGuid = executionGuid,
                    ReportPrintId = isAdmin ? printId : null,
                    ReportPrintIdMasked = printIdMasked
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Report run failed: {ex.Message}");
                await _repository.Report.UpdateExecutionLogAsync(new ReportExecutionLogUpdate
                {
                    ReportExecutionGuid = executionGuid,
                    Status = "Failed",
                    DurationMs = (int)sw.ElapsedMilliseconds,
                    ErrorMessage = ex.Message
                });
                throw;
            }
        }

        public Task<ReportExecutionLogDto?> GetExecutionByPrintIdAsync(string reportPrintId, bool includeFullPrintId)
            => _repository.Report.GetExecutionByPrintIdAsync(reportPrintId, includeFullPrintId);

        private static string NormalizeReportType(string reportType)
        {
            var tp = (reportType ?? "rpt").Trim().ToUpperInvariant();
            return tp switch
            {
                "RPT" => "RPT",
                "PVT" => "PVT",
                _ => throw new ArgumentException("Invalid report type. Use rpt or pvt.")
            };
        }

        private static long? ParseLong(string? value)
            => long.TryParse(value, out var id) ? id : null;

        private static Dictionary<string, object?> BuildRunParameters(
            ReportRunRequestDto request,
            IReadOnlyDictionary<string, string?> normalizedParameters)
        {
            var parameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            foreach (var kv in normalizedParameters)
                parameters[kv.Key] = kv.Value;

            if (request.DateFrom.HasValue)
                parameters["DateFrom"] = request.DateFrom;
            if (request.DateTo.HasValue)
                parameters["DateTo"] = request.DateTo;

            return parameters;
        }
    }
}
