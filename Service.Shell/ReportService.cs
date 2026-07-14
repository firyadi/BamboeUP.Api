using BamboeUp.Report.Abstractions;
using BamboeUp.Report.Engines;
using BamboeUp.Report.Services;
using Contracts;
using Microsoft.Extensions.Configuration;
using Service.Contracts.Shell;
using Service.Shell.Reporting;
using Shared.DataTransferObjects;
using System.Diagnostics;
using System.Text.Json;

namespace Service.Shell
{
    public class ReportService : IReportService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ReportHandlerSet _handlers;

        public ReportService(IRepositoryManager repository, ILoggerManager logger, IConfiguration? configuration = null)
        {
            _repository = repository;
            _logger = logger;
            var dataProvider = new RepositoryReportDataProvider(repository);
            var templateRoot = configuration?["ReportTemplateSettings:TemplateRoot"];
            var options = ReportEngineBootstrap.CreateDefaultOptions(templateRoot);
            _handlers = ReportEngineBootstrap.CreateHandlers(dataProvider, options);
        }

        public async Task<IEnumerable<ReportProgramDto>> GetAllowedReportsAsync(
            string reportType,
            Guid userGuid,
            string? companyId,
            string? officeId,
            bool isAdmin)
        {
            var normalized = NormalizeCatalogReportType(reportType);
            if (isAdmin)
                return await _repository.Report.GetAllReportsByTypeAsync(normalized);

            return await _repository.Report.GetAllowedReportsAsync(userGuid, companyId, officeId, normalized);
        }

        public async Task<IEnumerable<PrintSlipDto>> GetAllowedPrintsAsync(
            string sourceProgramCode,
            Guid userGuid,
            string? companyId,
            string? officeId,
            bool isAdmin,
            string? entityId = null)
        {
            if (string.IsNullOrWhiteSpace(sourceProgramCode))
                throw new ArgumentException("Source program code is required.");

            if (!string.IsNullOrWhiteSpace(entityId))
            {
                await PrintEntityValidator.ValidateAsync(
                    _repository,
                    sourceProgramCode.Trim(),
                    entityId);
            }

            if (isAdmin)
                return await _repository.Report.GetAllPrintSlipsBySourceAsync(sourceProgramCode.Trim());

            return await _repository.Report.GetAllowedPrintsAsync(
                userGuid,
                companyId,
                officeId,
                sourceProgramCode.Trim());
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
                IsTracked = definition.IsTracked,
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

            ReportProgramDto? selectedReport;
            if (reportKind == "DOC")
            {
                if (string.IsNullOrWhiteSpace(request.SourceProgramCode))
                    throw new ArgumentException("SourceProgramCode is required for DOC print.");

                await PrintEntityValidator.ValidateAsync(
                    _repository,
                    request.SourceProgramCode,
                    entityId: null,
                    parameters: request.Parameters);

                var allowedPrints = (await GetAllowedPrintsAsync(
                    request.SourceProgramCode,
                    userGuid,
                    companyId,
                    officeId,
                    isAdmin,
                    ResolveEntityIdForValidation(request.Parameters))).ToList();

                var selectedPrint = allowedPrints.FirstOrDefault(p => p.ProgramId == request.ProgramId);
                if (selectedPrint == null)
                    throw new UnauthorizedAccessException("You do not have permission to run this print slip.");

                selectedReport = new ReportProgramDto
                {
                    ProgramId = selectedPrint.ProgramId,
                    ProgramGuid = selectedPrint.ProgramGuid,
                    ProgramCode = selectedPrint.ProgramCode,
                    ProgramName = selectedPrint.ProgramName,
                    ProgramType = "DOC",
                    IsTracked = selectedPrint.IsTracked,
                    RequiresPrintId = selectedPrint.RequiresPrintId,
                    ReportDefinitionId = selectedPrint.ReportDefinitionId
                };
            }
            else
            {
                var allowedReports = (await GetAllowedReportsAsync(reportKind, userGuid, companyId, officeId, isAdmin)).ToList();
                selectedReport = allowedReports.FirstOrDefault(r => r.ProgramId == request.ProgramId);
                if (selectedReport == null)
                    throw new UnauthorizedAccessException("You do not have permission to run this report.");
            }

            var definition = await _repository.Report.ResolveDefinitionAsync(request.ProgramId, companyIdLong, reportKind);
            var schemaFields = definition == null
                ? new List<ReportParameterDefinitionDto>()
                : (await _repository.Report.GetParametersAsync(definition.ReportDefinitionId)).ToList();

            var normalizedParameters = ReportParameterValidator.ValidateAndNormalize(
                request,
                schemaFields,
                isDocPrint: reportKind == "DOC");

            var isTracked = definition?.IsTracked == true;
            var requiresPrintId = isTracked && definition?.RequiresPrintId == true;

            var executionGuid = Guid.NewGuid();
            string? printId = null;
            string? printIdMasked = null;

            if (requiresPrintId)
            {
                if (request.IsReprint && string.IsNullOrWhiteSpace(request.ReprintOfPrintId))
                    throw new ArgumentException("Original Print ID is required for reprint.");
                if (request.IsReprint && string.IsNullOrWhiteSpace(request.ReprintReason))
                    throw new ArgumentException("Reprint reason is required.");

                var prefix = string.IsNullOrWhiteSpace(definition!.PrintIdPrefix) ? "RP" : definition.PrintIdPrefix!;
                var period = request.DateFrom?.ToString("yyyyMM")
                             ?? (normalizedParameters.TryGetValue("DateFrom", out var df) && DateTime.TryParse(df, out var dfrom) ? dfrom.ToString("yyyyMM") : DateTime.UtcNow.ToString("yyyyMM"));
                (printId, printIdMasked) = ReportPrintIdGenerator.Generate(prefix, period);
            }

            var maskedLogParameters = ReportParameterValidator.MaskForLog(normalizedParameters, schemaFields);
            var parametersJson = JsonSerializer.Serialize(maskedLogParameters);

            if (isTracked)
            {
                var user = await _repository.User.GetUserAsync(userGuid, trackChanges: false);
                var userDisplayName = string.IsNullOrWhiteSpace(user?.FullName) ? user?.UserName : user?.FullName;

                await _repository.ReportExecutionLog.InsertAsync(new ReportExecutionLogInsert
                {
                    ReportExecutionGuid = executionGuid,
                    ProgramId = request.ProgramId,
                    ProgramCode = selectedReport.ProgramCode ?? request.ProgramCode,
                    ProgramName = selectedReport.ProgramName ?? string.Empty,
                    ReportDefinitionId = definition?.ReportDefinitionId,
                    UserId = userId,
                    UserDisplayName = userDisplayName,
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
                    OutputFormat = reportKind switch
                    {
                        "PVT" => "Pivot",
                        "DOC" => "PDF",
                        _ => "PDF"
                    }
                });
            }

            try
            {
                var branding = await ResolveReportBrandingAsync(companyIdLong, officeIdLong);

                var context = new ReportRunContext
                {
                    ProgramId = request.ProgramId,
                    ProgramCode = selectedReport.ProgramCode ?? request.ProgramCode,
                    ProgramName = selectedReport.ProgramName ?? string.Empty,
                    ReportKind = reportKind,
                    ReportDefinitionId = definition?.ReportDefinitionId,
                    DefinitionKey = definition?.DefinitionKey,
                    FilePath = definition?.FilePath,
                    RendererType = definition?.RendererType,
                    StoreProcedureName = definition?.StoreProcedureName,
                    LayoutJson = definition?.LayoutJson,
                    UserId = userId,
                    CompanyId = companyIdLong,
                    CompanyOfficeId = officeIdLong,
                    CompanyName = branding.CompanyName,
                    CompanyOfficeName = branding.OfficeName,
                    CompanyLogo = branding.CompanyLogo,
                    Parameters = BuildRunParameters(request, normalizedParameters),
                    Print = new ReportPrintContext
                    {
                        RequiresPrintId = requiresPrintId,
                        ReportPrintId = printId,
                        ReportPrintIdMasked = printIdMasked,
                        IsReprint = request.IsReprint,
                        ReprintOfPrintId = request.ReprintOfPrintId,
                        ReprintReason = request.ReprintReason
                    }
                };

                var handler = _handlers.Resolve(reportKind);
                var handlerResult = await handler.RunAsync(context);

                if (isTracked)
                {
                    await _repository.ReportExecutionLog.UpdateAsync(new ReportExecutionLogUpdate
                    {
                        ReportExecutionGuid = executionGuid,
                        Status = handlerResult.Success ? "Completed" : "Failed",
                        DurationMs = (int)sw.ElapsedMilliseconds,
                        ErrorMessage = handlerResult.Success ? null : handlerResult.Message
                    });
                }

                return new ReportRunResultDto
                {
                    Success = handlerResult.Success,
                    Message = handlerResult.Message,
                    ReportExecutionGuid = isTracked ? executionGuid : Guid.Empty,
                    ReportPrintId = isAdmin ? printId : null,
                    ReportPrintIdMasked = printIdMasked,
                    OutputContentType = handlerResult.OutputContentType,
                    OutputFileName = handlerResult.OutputFileName,
                    OutputBase64 = handlerResult.OutputBytes is { Length: > 0 } bytes
                        ? Convert.ToBase64String(bytes)
                        : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Report run failed: {ex.Message}");
                if (isTracked)
                {
                    await _repository.ReportExecutionLog.UpdateAsync(new ReportExecutionLogUpdate
                {
                    ReportExecutionGuid = executionGuid,
                    Status = "Failed",
                    DurationMs = (int)sw.ElapsedMilliseconds,
                    ErrorMessage = ex.Message
                });
                }
                throw;
            }
        }

        public Task<ReportExecutionLogDto?> GetExecutionByPrintIdAsync(string reportPrintId, bool includeFullPrintId)
            => _repository.ReportExecutionLog.GetByPrintIdAsync(reportPrintId, includeFullPrintId);

        public async Task<PagedResult<ReportExecutionLogDto>> GetExecutionLogsAsync(
            ReportExecutionLogQueryDto query,
            Guid userGuid,
            string? companyId,
            string? officeId,
            bool isAdmin)
        {
            var result = await _repository.ReportExecutionLog.GetPagedAsync(
                userGuid,
                companyId,
                officeId,
                scoped: !isAdmin,
                query);

            var maskedItems = result.Items
                .Select(item => item with { ReportPrintId = null })
                .ToList();

            return result with { Items = maskedItems };
        }

        private static string NormalizeReportType(string reportType)
        {
            var tp = (reportType ?? "rpt").Trim().ToUpperInvariant();
            return tp switch
            {
                "RPT" => "RPT",
                "PVT" => "PVT",
                "DOC" => "DOC",
                _ => throw new ArgumentException("Invalid report type. Use rpt, pvt, or doc.")
            };
        }

        private static string NormalizeCatalogReportType(string reportType)
        {
            var normalized = NormalizeReportType(reportType);
            if (normalized == "DOC")
            {
                throw new ArgumentException(
                    "DOC slips are not available through the report catalog. Use GET /api/prints/allowed.");
            }

            return normalized;
        }

        private static string? ResolveEntityIdForValidation(IReadOnlyDictionary<string, string?> parameters)
        {
            if (parameters.TryGetValue("EntityGuid", out var entityGuid) && !string.IsNullOrWhiteSpace(entityGuid))
                return entityGuid;

            foreach (var kv in parameters)
            {
                if (!kv.Key.EndsWith("Guid", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (kv.Key.Equals("CompanyGuid", StringComparison.OrdinalIgnoreCase)
                    || kv.Key.Equals("CompanyOfficeGuid", StringComparison.OrdinalIgnoreCase)
                    || kv.Key.Equals("ProgramGuid", StringComparison.OrdinalIgnoreCase)
                    || kv.Key.Equals("UserGuid", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(kv.Value))
                    return kv.Value;
            }

            return null;
        }

        private static long? ParseLong(string? value)
            => long.TryParse(value, out var id) ? id : null;

        private async Task<(string? CompanyName, string? OfficeName, byte[]? CompanyLogo)> ResolveReportBrandingAsync(
            long? companyId,
            long? officeId)
        {
            string? companyName = null;
            byte[]? companyLogo = null;
            string? officeName = null;

            if (companyId.HasValue)
            {
                var company = await _repository.Company.GetCompanyByIdAsync(companyId.Value, trackChanges: false);
                companyName = company?.CompanyName;
                companyLogo = company?.CompanyLogo;
            }

            if (officeId.HasValue)
            {
                var office = await _repository.CompanyOffice.GetCompanyOfficeByIdAsync(officeId.Value, trackChanges: false);
                officeName = office?.CompanyOfficeName;
            }

            return (companyName, officeName, companyLogo);
        }

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
