using Contracts;
using Dapper;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly RepositoryContext _context;

        public ReportRepository(RepositoryContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<ReportProgramDto>> GetAllowedReportsAsync(
            Guid userGuid,
            string? companyId,
            string? officeId,
            string reportType,
            IDbTransaction? transaction = null)
        {
            return QueryReportsAsync(userGuid, companyId, officeId, reportType, scoped: true, transaction);
        }

        public Task<IEnumerable<ReportProgramDto>> GetAllReportsByTypeAsync(string reportType, IDbTransaction? transaction = null)
        {
            return QueryReportsAsync(null, null, null, reportType, scoped: false, transaction);
        }

        private async Task<IEnumerable<ReportProgramDto>> QueryReportsAsync(
            Guid? userGuid,
            string? companyId,
            string? officeId,
            string reportType,
            bool scoped,
            IDbTransaction? transaction)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("ReportType", reportType.ToUpperInvariant());

            var sql = new StringBuilder(@"
                SELECT DISTINCT
                    p.ProgramId,
                    p.ProgramGuid,
                    p.ProgramCode,
                    p.ProgramName,
                    p.ProgramType,
                    p.StoreProcedureName,
                    ISNULL(p.IsProgramPrintAble, 0) AS IsProgramPrintAble,
                    rd.ReportScope,
                    ISNULL(rd.IsTracked, 0) AS IsTracked,
                    ISNULL(rd.RequiresPrintId, 0) AS RequiresPrintId,
                    rd.ReportDefinitionId
                FROM [core].[Programs] p");

            if (scoped)
            {
                sql.Append(@"
                INNER JOIN [core].[UserGroupProgram] ugp ON ugp.ProgramsId = p.ProgramId
                INNER JOIN [core].[UserGroupScope] ugs ON ugs.UserGroupId = ugp.UserGroupId
                INNER JOIN [core].[Users] u ON u.UserId = ugs.UserId");
            }

            sql.Append(@"
                OUTER APPLY (
                    SELECT TOP 1 rd.*
                    FROM [core].[ReportDefinition] rd
                    WHERE rd.ProgramId = p.ProgramId
                      AND rd.ReportKind = @ReportType
                      AND rd.IsActive = 1
                      AND rd.StatusId > 0");

            if (long.TryParse(companyId, out var companyIdLong))
            {
                sql.Append(" AND (rd.CompanyId IS NULL OR rd.CompanyId = @CompanyIdLong)");
                parameters.Add("CompanyIdLong", companyIdLong);
            }
            else
            {
                sql.Append(" AND rd.CompanyId IS NULL");
            }

            sql.Append(@"
                    ORDER BY CASE WHEN rd.CompanyId IS NULL THEN 1 ELSE 0 END
                ) rd
                WHERE p.ProgramType = @ReportType
                  AND p.ProgramType IN (N'RPT', N'PVT')
                  AND p.StatusId > 0
                  AND p.DeletedTime IS NULL
                  AND p.IsActive = 1");

            if (scoped)
            {
                parameters.Add("UserGuid", userGuid);
                sql.Append(@"
                  AND u.UserGuid = @UserGuid
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                  AND ugp.StatusId > 0 AND ugp.DeletedTime IS NULL
                  AND ugp.IsUserGroupViewAble = 1");

                if (!string.IsNullOrEmpty(companyId))
                {
                    if (Guid.TryParse(companyId, out var companyGuid))
                    {
                        sql.Append(" AND ugs.CompanyGuid = @CompanyGuid");
                        parameters.Add("CompanyGuid", companyGuid);
                    }
                    else if (long.TryParse(companyId, out companyIdLong))
                    {
                        sql.Append(" AND ugs.CompanyId = @CompanyIdLong");
                        if (!parameters.ParameterNames.Contains("CompanyIdLong"))
                            parameters.Add("CompanyIdLong", companyIdLong);
                    }
                }

                if (!string.IsNullOrEmpty(officeId))
                {
                    if (Guid.TryParse(officeId, out var officeGuid))
                    {
                        sql.Append(" AND (ugs.CompanyOfficeGuid = @OfficeGuid OR ugs.CompanyOfficeGuid IS NULL)");
                        parameters.Add("OfficeGuid", officeGuid);
                    }
                    else if (long.TryParse(officeId, out var officeIdLong))
                    {
                        sql.Append(" AND (ugs.CompanyOfficeId = @OfficeIdLong OR ugs.CompanyOfficeId IS NULL)");
                        parameters.Add("OfficeIdLong", officeIdLong);
                    }
                }
                else
                {
                    sql.Append(" AND ugs.CompanyOfficeId IS NULL");
                }
            }

            sql.Append(" ORDER BY p.ProgramCode, p.ProgramId ASC");

            return await connection.QueryAsync<ReportProgramDto>(sql.ToString(), parameters, transaction);
        }

        public async Task<ResolvedReportDefinitionRow?> ResolveDefinitionAsync(
            long programId,
            long? companyId,
            string reportKind,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            const string customSql = @"
                SELECT TOP 1 *
                FROM [core].[ReportDefinition]
                WHERE ProgramId = @ProgramId
                  AND ReportKind = @ReportKind
                  AND CompanyId = @CompanyId
                  AND IsActive = 1
                  AND StatusId > 0
                ORDER BY [Version] DESC";

            if (companyId.HasValue)
            {
                var custom = await connection.QuerySingleOrDefaultAsync<ResolvedReportDefinitionRow>(
                    customSql,
                    new { ProgramId = programId, ReportKind = reportKind.ToUpperInvariant(), CompanyId = companyId },
                    transaction);
                if (custom != null)
                    return custom;
            }

            const string standardSql = @"
                SELECT TOP 1 *
                FROM [core].[ReportDefinition]
                WHERE ProgramId = @ProgramId
                  AND ReportKind = @ReportKind
                  AND CompanyId IS NULL
                  AND ReportScope = N'Standard'
                  AND IsActive = 1
                  AND StatusId > 0
                ORDER BY [Version] DESC";

            return await connection.QuerySingleOrDefaultAsync<ResolvedReportDefinitionRow>(
                standardSql,
                new { ProgramId = programId, ReportKind = reportKind.ToUpperInvariant() },
                transaction);
        }

        public async Task<IEnumerable<ReportParameterDefinitionDto>> GetParametersAsync(
            long reportDefinitionId,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            const string sql = @"
                SELECT ReportParameterId, ReportDefinitionId, FieldKey, ParameterName, DisplayLabel,
                       ControlType, DataType, IsRequired, DefaultValue, SortOrder,
                       ColumnGroup, ColumnSpan, RowGroup, LookupType, NULL AS LookupConfig,
                       VisibleWhen, IsSensitive
                FROM [core].[ReportParameter]
                WHERE ReportDefinitionId = @ReportDefinitionId
                  AND StatusId > 0
                ORDER BY ColumnGroup, SortOrder, ReportParameterId";
            return await connection.QueryAsync<ReportParameterDefinitionDto>(sql, new { reportDefinitionId }, transaction);
        }

        public async Task<IEnumerable<ReportLookupItemDto>> LookupAsync(
            string lookupType,
            string? query,
            long? companyId,
            long? officeId,
            string? lookupConfig,
            int take,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var q = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
            take = Math.Clamp(take, 1, 50);

            return lookupType.ToUpperInvariant() switch
            {
                "EMPLOYEE" => await LookupEmployeesAsync(connection, q, companyId, officeId, take, transaction),
                "ORGANIZATIONUNIT" => await LookupOrganizationUnitsAsync(connection, q, take, transaction),
                _ => Array.Empty<ReportLookupItemDto>()
            };
        }

        private static Task<IEnumerable<ReportLookupItemDto>> LookupEmployeesAsync(
            IDbConnection connection,
            string? q,
            long? companyId,
            long? officeId,
            int take,
            IDbTransaction? transaction)
        {
            const string sql = @"
                SELECT TOP (@Take)
                    CAST(u.UserId AS NVARCHAR(20)) AS Id,
                    CONCAT(u.FullName, N' (', u.UserName, N')') AS Label
                FROM [core].[Users] u
                INNER JOIN [core].[UserGroupScope] ugs ON ugs.UserId = u.UserId
                WHERE u.StatusId > 0 AND u.DeletedTime IS NULL
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                  AND (@CompanyId IS NULL OR ugs.CompanyId = @CompanyId)
                  AND (@OfficeId IS NULL OR ugs.CompanyOfficeId IS NULL OR ugs.CompanyOfficeId = @OfficeId)
                  AND (@Q IS NULL OR u.FullName LIKE N'%' + @Q + N'%' OR u.UserName LIKE N'%' + @Q + N'%')
                GROUP BY u.UserId, u.FullName, u.UserName
                ORDER BY u.FullName, u.UserName";

            return connection.QueryAsync<ReportLookupItemDto>(sql, new { Take = take, Q = q, CompanyId = companyId, OfficeId = officeId }, transaction);
        }

        private static Task<IEnumerable<ReportLookupItemDto>> LookupOrganizationUnitsAsync(
            IDbConnection connection,
            string? q,
            int take,
            IDbTransaction? transaction)
        {
            const string sql = @"
                SELECT TOP (@Take)
                    CAST(ou.OrganizationUnitId AS NVARCHAR(20)) AS Id,
                    CONCAT(ou.OrganizationUnitName, N' (', ou.OrganizationUnitCode, N')') AS Label
                FROM [app].[OrganizationUnit] ou
                WHERE ou.StatusId > 0 AND ou.DeletedTime IS NULL
                  AND (@Q IS NULL OR ou.OrganizationUnitName LIKE N'%' + @Q + N'%' OR ou.OrganizationUnitCode LIKE N'%' + @Q + N'%')
                ORDER BY ou.OrganizationUnitName, ou.OrganizationUnitCode";

            return connection.QueryAsync<ReportLookupItemDto>(sql, new { Take = take, Q = q }, transaction);
        }

        public Task<IEnumerable<PrintSlipDto>> GetAllowedPrintsAsync(
            Guid userGuid,
            string? companyId,
            string? officeId,
            string sourceProgramCode,
            IDbTransaction? transaction = null)
            => QueryPrintSlipsAsync(userGuid, companyId, officeId, sourceProgramCode, scoped: true, transaction);

        public Task<IEnumerable<PrintSlipDto>> GetAllPrintSlipsBySourceAsync(
            string sourceProgramCode,
            IDbTransaction? transaction = null)
            => QueryPrintSlipsAsync(null, companyId: null, officeId: null, sourceProgramCode, scoped: false, transaction);

        private async Task<IEnumerable<PrintSlipDto>> QueryPrintSlipsAsync(
            Guid? userGuid,
            string? companyId,
            string? officeId,
            string sourceProgramCode,
            bool scoped,
            IDbTransaction? transaction)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("SourceProgramCode", sourceProgramCode.Trim());

            var sql = new StringBuilder(@"
                SELECT DISTINCT
                    p.ProgramId,
                    p.ProgramGuid,
                    p.ProgramCode,
                    p.ProgramName,
                    p.RowIndex AS SortOrder,
                    rd.DefinitionKey,
                    ISNULL(rd.IsTracked, 0) AS IsTracked,
                    ISNULL(rd.RequiresPrintId, 0) AS RequiresPrintId,
                    rd.ReportDefinitionId
                FROM [core].[Programs] parent
                INNER JOIN [core].[Programs] p ON p.ParentId = parent.ProgramId");

            if (scoped)
            {
                sql.Append(@"
                INNER JOIN [core].[UserGroupProgram] ugp ON ugp.ProgramsId = p.ProgramId
                INNER JOIN [core].[UserGroupScope] ugs ON ugs.UserGroupId = ugp.UserGroupId
                INNER JOIN [core].[Users] u ON u.UserId = ugs.UserId");
            }

            sql.Append(@"
                OUTER APPLY (
                    SELECT TOP 1 rd.ReportDefinitionId, rd.DefinitionKey, rd.IsTracked, rd.RequiresPrintId
                    FROM [core].[ReportDefinition] rd
                    WHERE rd.ProgramId = p.ProgramId
                      AND rd.ReportKind = N'DOC'
                      AND rd.IsActive = 1
                      AND rd.StatusId > 0");

            if (long.TryParse(companyId, out var companyIdLong))
            {
                sql.Append(" AND (rd.CompanyId IS NULL OR rd.CompanyId = @CompanyIdLong)");
                parameters.Add("CompanyIdLong", companyIdLong);
            }
            else
            {
                sql.Append(" AND rd.CompanyId IS NULL");
            }

            sql.Append(@"
                    ORDER BY CASE WHEN rd.CompanyId IS NULL THEN 1 ELSE 0 END
                ) rd
                WHERE parent.ProgramCode = @SourceProgramCode
                  AND parent.StatusId > 0 AND parent.DeletedTime IS NULL AND parent.IsActive = 1
                  AND p.ProgramType = N'DOC'
                  AND p.StatusId > 0 AND p.DeletedTime IS NULL AND p.IsActive = 1");

            if (scoped)
            {
                parameters.Add("UserGuid", userGuid);
                sql.Append(@"
                  AND u.UserGuid = @UserGuid
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                  AND ugp.StatusId > 0 AND ugp.DeletedTime IS NULL
                  AND ugp.IsUserGroupViewAble = 1");

                if (!string.IsNullOrEmpty(companyId))
                {
                    if (Guid.TryParse(companyId, out var companyGuid))
                    {
                        sql.Append(" AND ugs.CompanyGuid = @CompanyGuid");
                        parameters.Add("CompanyGuid", companyGuid);
                    }
                    else if (long.TryParse(companyId, out companyIdLong))
                    {
                        sql.Append(" AND ugs.CompanyId = @CompanyIdLong");
                        if (!parameters.ParameterNames.Contains("CompanyIdLong"))
                            parameters.Add("CompanyIdLong", companyIdLong);
                    }
                }

                if (!string.IsNullOrEmpty(officeId))
                {
                    if (Guid.TryParse(officeId, out var officeGuid))
                    {
                        sql.Append(" AND (ugs.CompanyOfficeGuid = @OfficeGuid OR ugs.CompanyOfficeGuid IS NULL)");
                        parameters.Add("OfficeGuid", officeGuid);
                    }
                    else if (long.TryParse(officeId, out var officeIdLong))
                    {
                        sql.Append(" AND (ugs.CompanyOfficeId = @OfficeIdLong OR ugs.CompanyOfficeId IS NULL)");
                        parameters.Add("OfficeIdLong", officeIdLong);
                    }
                }
                else
                {
                    sql.Append(" AND ugs.CompanyOfficeId IS NULL");
                }
            }

            sql.Append(" ORDER BY p.RowIndex, p.ProgramCode, p.ProgramId ASC");

            return await connection.QueryAsync<PrintSlipDto>(sql.ToString(), parameters, transaction);
        }

        public async Task<DataTable> ExecuteReportDataAsync(
            string storedProcedureName,
            IReadOnlyDictionary<string, object?> parameters,
            IDbTransaction? transaction = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcedureName))
                throw new ArgumentException("Stored procedure name is required.", nameof(storedProcedureName));

            using var connection = _context.CreateConnection();
            var dynamicParams = new DynamicParameters();
            foreach (var kv in parameters)
                dynamicParams.Add(kv.Key, kv.Value);

            using var reader = await connection.ExecuteReaderAsync(
                storedProcedureName.Trim(),
                dynamicParams,
                transaction,
                commandType: CommandType.StoredProcedure);

            var table = new DataTable();
            table.Load(reader);
            return table;
        }
    }
}
