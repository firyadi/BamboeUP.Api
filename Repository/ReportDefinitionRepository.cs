using Contracts;
using Dapper;
using Repository.Extensions;
using Shared.DataTransferObjects;
using System.Data;
using System.Text;

namespace Repository
{
    public class ReportDefinitionRepository : IReportDefinitionRepository
    {
        private readonly RepositoryContext _context;

        private const string SelectColumns = @"
                rd.ReportDefinitionId, rd.ReportDefinitionGuid, rd.ProgramId,
                p.ProgramCode, p.ProgramName,
                rd.ReportScope, rd.CompanyId, c.CompanyName,
                rd.ReportKind, rd.DefinitionKey, rd.RendererType, rd.FilePath, rd.StoreProcedureName,
                rd.IsTracked, rd.RequiresPrintId, rd.PrintIdPolicy, rd.PrintIdPrefix, rd.LayoutJson,
                rd.[Version], rd.IsActive, rd.EffectiveFrom, rd.EffectiveTo,
                rd.StatusId, rd.CreatedById, rd.CreatedTime, rd.UpdatedById, rd.UpdatedTime";

        private const string FromClause = @"
                FROM [core].[ReportDefinition] rd
                INNER JOIN [core].[Programs] p ON p.ProgramId = rd.ProgramId
                LEFT JOIN [app].[Company] c ON c.CompanyId = rd.CompanyId";

        public ReportDefinitionRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReportDefinitionDto>> GetAllAsync(IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT {SelectColumns}
                {FromClause}
                WHERE rd.StatusId > 0
                ORDER BY p.ProgramCode, rd.ReportKind, rd.DefinitionKey, rd.[Version] DESC";
            return await connection.QueryAsync<ReportDefinitionDto>(sql, transaction: transaction);
        }

        public async Task<ReportDefinitionDto?> GetByGuidAsync(Guid reportDefinitionGuid, IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT {SelectColumns}
                {FromClause}
                WHERE rd.ReportDefinitionGuid = @ReportDefinitionGuid AND rd.StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<ReportDefinitionDto>(
                sql, new { ReportDefinitionGuid = reportDefinitionGuid }, transaction);
        }

        public async Task<IEnumerable<ReportDefinitionDto>> SearchAsync(
            ReportDefinitionSearchDto criteria,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            var where = new StringBuilder(" WHERE rd.StatusId > 0 ");

            if (!string.IsNullOrWhiteSpace(criteria.DefinitionKey))
            {
                var param = SqlFilterHelper.BuildFilter(
                    "rd.DefinitionKey", "@definitionKey",
                    criteria.DefinitionKeySearchType.ToString(),
                    parameters, "definitionKey", criteria.DefinitionKey);
                where.Append($" AND {param} ");
            }

            if (!string.IsNullOrWhiteSpace(criteria.ProgramCode))
            {
                var param = SqlFilterHelper.BuildFilter(
                    "p.ProgramCode", "@programCode",
                    criteria.ProgramCodeSearchType.ToString(),
                    parameters, "programCode", criteria.ProgramCode);
                where.Append($" AND {param} ");
            }

            if (!string.IsNullOrWhiteSpace(criteria.ProgramName))
            {
                var param = SqlFilterHelper.BuildFilter(
                    "p.ProgramName", "@programName",
                    criteria.ProgramNameSearchType.ToString(),
                    parameters, "programName", criteria.ProgramName);
                where.Append($" AND {param} ");
            }

            if (!string.IsNullOrWhiteSpace(criteria.ReportKind))
            {
                where.Append(" AND rd.ReportKind = @ReportKind ");
                parameters.Add("ReportKind", criteria.ReportKind.Trim().ToUpperInvariant());
            }

            if (!string.IsNullOrWhiteSpace(criteria.ReportScope))
            {
                where.Append(" AND rd.ReportScope = @ReportScope ");
                parameters.Add("ReportScope", criteria.ReportScope.Trim());
            }

            if (criteria.IsTracked.HasValue)
            {
                where.Append(" AND rd.IsTracked = @IsTracked ");
                parameters.Add("IsTracked", criteria.IsTracked.Value);
            }

            if (criteria.RequiresPrintId.HasValue)
            {
                where.Append(" AND rd.RequiresPrintId = @RequiresPrintId ");
                parameters.Add("RequiresPrintId", criteria.RequiresPrintId.Value);
            }

            if (criteria.IsActive.HasValue)
            {
                where.Append(" AND rd.IsActive = @IsActive ");
                parameters.Add("IsActive", criteria.IsActive.Value);
            }

            var sql = $@"
                SELECT {SelectColumns}
                {FromClause}
                {where}
                ORDER BY p.ProgramCode, rd.ReportKind, rd.DefinitionKey, rd.[Version] DESC";

            return await connection.QueryAsync<ReportDefinitionDto>(sql, parameters, transaction);
        }

        public async Task<Guid> CreateAsync(ReportDefinitionForCreationDto input, IDbTransaction? transaction = null)
        {
            ValidateTrackingRules(input.IsTracked, input.RequiresPrintId);

            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [core].[ReportDefinition]
                (ReportDefinitionGuid, ProgramId, ReportScope, CompanyId, ReportKind, DefinitionKey, RendererType,
                 FilePath, StoreProcedureName, LayoutJson, IsTracked, RequiresPrintId, PrintIdPolicy, PrintIdPrefix,
                 [Version], IsActive, EffectiveFrom, EffectiveTo, StatusId, CreatedById, CreatedTime)
                OUTPUT INSERTED.ReportDefinitionGuid
                VALUES
                (NEWID(), @ProgramId, @ReportScope, @CompanyId, @ReportKind, @DefinitionKey, @RendererType,
                 @FilePath, @StoreProcedureName, @LayoutJson, @IsTracked, @RequiresPrintId, @PrintIdPolicy, @PrintIdPrefix,
                 @Version, @IsActive, @EffectiveFrom, @EffectiveTo, 1, @CreatedById, SYSDATETIME())";

            return await conn.QuerySingleAsync<Guid>(sql, NormalizeInput(input), transaction);
        }

        public async Task UpdateAsync(
            Guid reportDefinitionGuid,
            ReportDefinitionForUpdateDto input,
            IDbTransaction? transaction = null)
        {
            ValidateTrackingRules(input.IsTracked, input.RequiresPrintId);

            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [core].[ReportDefinition]
                SET ProgramId = @ProgramId,
                    ReportScope = @ReportScope,
                    CompanyId = @CompanyId,
                    ReportKind = @ReportKind,
                    DefinitionKey = @DefinitionKey,
                    RendererType = @RendererType,
                    FilePath = @FilePath,
                    StoreProcedureName = @StoreProcedureName,
                    LayoutJson = @LayoutJson,
                    IsTracked = @IsTracked,
                    RequiresPrintId = @RequiresPrintId,
                    PrintIdPolicy = @PrintIdPolicy,
                    PrintIdPrefix = @PrintIdPrefix,
                    [Version] = @Version,
                    IsActive = @IsActive,
                    EffectiveFrom = @EffectiveFrom,
                    EffectiveTo = @EffectiveTo,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = SYSDATETIME()
                WHERE ReportDefinitionGuid = @ReportDefinitionGuid AND StatusId > 0";

            var affected = await conn.ExecuteAsync(sql, new
            {
                ReportDefinitionGuid = reportDefinitionGuid,
                input.ProgramId,
                ReportScope = input.ReportScope?.Trim() ?? "Standard",
                input.CompanyId,
                ReportKind = input.ReportKind?.Trim().ToUpperInvariant() ?? "RPT",
                DefinitionKey = input.DefinitionKey?.Trim() ?? string.Empty,
                RendererType = string.IsNullOrWhiteSpace(input.RendererType) ? null : input.RendererType.Trim(),
                input.FilePath,
                input.StoreProcedureName,
                input.LayoutJson,
                input.IsTracked,
                RequiresPrintId = input.IsTracked && input.RequiresPrintId,
                input.PrintIdPolicy,
                input.PrintIdPrefix,
                input.Version,
                input.IsActive,
                input.EffectiveFrom,
                input.EffectiveTo,
                input.UpdatedById
            }, transaction);

            if (affected == 0)
                throw new KeyNotFoundException($"ReportDefinition {reportDefinitionGuid} not found.");
        }

        public async Task SoftDeleteAsync(Guid reportDefinitionGuid, long deletedById, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [core].[ReportDefinition]
                SET StatusId = 0,
                    IsActive = 0,
                    UpdatedById = @DeletedById,
                    UpdatedTime = SYSDATETIME()
                WHERE ReportDefinitionGuid = @ReportDefinitionGuid AND StatusId > 0";

            var affected = await conn.ExecuteAsync(sql, new { ReportDefinitionGuid = reportDefinitionGuid, DeletedById = deletedById }, transaction);
            if (affected == 0)
                throw new KeyNotFoundException($"ReportDefinition {reportDefinitionGuid} not found.");
        }

        public async Task ReplaceParametersAsync(
            long reportDefinitionId,
            IReadOnlyList<ReportParameterForUpsertDto> parameters,
            long updatedById,
            IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string deleteSql = @"DELETE FROM [core].[ReportParameter] WHERE ReportDefinitionId = @ReportDefinitionId";
            await conn.ExecuteAsync(deleteSql, new { ReportDefinitionId = reportDefinitionId }, transaction);

            if (parameters.Count == 0)
                return;

            const string insertSql = @"
                INSERT INTO [core].[ReportParameter]
                (ReportDefinitionId, ParameterName, DisplayLabel, DataType, IsRequired, SortOrder,
                 LookupType, IsSensitive, StatusId, CreatedById, CreatedTime,
                 FieldKey, ControlType, ColumnGroup, ColumnSpan, RowGroup)
                VALUES
                (@ReportDefinitionId, @ParameterName, @DisplayLabel, @DataType, @IsRequired, @SortOrder,
                 @LookupType, @IsSensitive, 1, @UpdatedById, SYSDATETIME(),
                 @FieldKey, @ControlType, @ColumnGroup, @ColumnSpan, @RowGroup)";

            foreach (var row in parameters)
            {
                await conn.ExecuteAsync(insertSql, new
                {
                    ReportDefinitionId = reportDefinitionId,
                    ParameterName = row.ParameterName.Trim(),
                    DisplayLabel = row.DisplayLabel.Trim(),
                    DataType = row.DataType.Trim(),
                    row.IsRequired,
                    row.SortOrder,
                    LookupType = string.IsNullOrWhiteSpace(row.LookupType) ? null : row.LookupType.Trim(),
                    row.IsSensitive,
                    UpdatedById = updatedById,
                    FieldKey = string.IsNullOrWhiteSpace(row.FieldKey) ? null : row.FieldKey.Trim(),
                    ControlType = row.ControlType.Trim(),
                    row.ColumnGroup,
                    row.ColumnSpan,
                    RowGroup = string.IsNullOrWhiteSpace(row.RowGroup) ? null : row.RowGroup.Trim()
                }, transaction);
            }
        }

        private static object NormalizeInput(ReportDefinitionForCreationDto input) => new
        {
            input.ProgramId,
            ReportScope = input.ReportScope?.Trim() ?? "Standard",
            input.CompanyId,
            ReportKind = input.ReportKind?.Trim().ToUpperInvariant() ?? "RPT",
            DefinitionKey = input.DefinitionKey?.Trim() ?? string.Empty,
            RendererType = string.IsNullOrWhiteSpace(input.RendererType) ? null : input.RendererType.Trim(),
            input.FilePath,
            input.StoreProcedureName,
            input.LayoutJson,
            input.IsTracked,
            RequiresPrintId = input.IsTracked && input.RequiresPrintId,
            input.PrintIdPolicy,
            input.PrintIdPrefix,
            input.Version,
            input.IsActive,
            input.EffectiveFrom,
            input.EffectiveTo,
            input.CreatedById
        };

        private static void ValidateTrackingRules(bool isTracked, bool requiresPrintId)
        {
            if (requiresPrintId && !isTracked)
                throw new ArgumentException("RequiresPrintId requires IsTracked to be enabled.");
        }
    }
}
