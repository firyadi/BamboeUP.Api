using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class ParameterscopeRepository : IParameterscopeRepository
    {
        private readonly RepositoryContext _context;
        private readonly ISqlDialect _dialect;
        private readonly int _maxRows = Contracts.ParameterContext.MaxResultRecord;

        public ParameterscopeRepository(RepositoryContext context, ISqlDialect dialect)
        {
            _context = context;
            _dialect = dialect;
        }

        public async Task<Parameterscope?> GetParameterscopeAsync(Guid parameterscopeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParameterId.Parametername AS ParameterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "ParameterScope")} a
LEFT JOIN {_dialect.Qualify("app", "Parameter")} j_ParameterId ON a.ParameterId = j_ParameterId.ParameterId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.ParameterscopeGuid = @parameterscopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Parameterscope>(sql, new { parameterscopeGuid });
        }

        public async Task<Parameterscope?> GetByParameterGuidAndParameterscopeGuidAsync(Guid parameterGuid, Guid parameterscopeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParameterId.Parametername AS ParameterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "ParameterScope")} a
LEFT JOIN {_dialect.Qualify("app", "Parameter")} j_ParameterId ON a.ParameterId = j_ParameterId.ParameterId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.ParameterGuid = @parameterGuid
                  AND a.ParameterscopeGuid = @parameterscopeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Parameterscope>(sql, new { parameterGuid, parameterscopeGuid });
        }

        public async Task<IEnumerable<Parameterscope>> GetAllByParameterGuidAsync(Guid parameterGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParameterId.Parametername AS ParameterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "ParameterScope")} a
LEFT JOIN {_dialect.Qualify("app", "Parameter")} j_ParameterId ON a.ParameterId = j_ParameterId.ParameterId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE a.ParameterGuid = @parameterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.ParameterscopeId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Parameterscope>(sql, new { parameterGuid });
        }

        public async Task CreateParameterscopeAsync(Parameterscope parameterscope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(parameterscope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_dialect.Qualify("app", "ParameterScope")}
                (ParameterscopeGuid, ParameterId, ParameterGuid, CreatedById, StatusId, CreatedTime, Overridevalue,
                 CompanyId, CompanyGuid, CompanyOfficeId, CompanyOfficeGuid
                )
                VALUES
                (@ParameterscopeGuid, @ParameterId, @ParameterGuid, @CreatedById, @StatusId, @CreatedTime, @Overridevalue,
                 @CompanyId, @CompanyGuid, @CompanyOfficeId, @CompanyOfficeGuid
                ){_dialect.ReturnIdentitySuffix("ParameterScopeId")}";
            parameterscope.ParameterscopeId = await conn.QuerySingleAsync<long>(sql, parameterscope, transaction);
        }

        public async Task UpdateParameterscopeAsync(Parameterscope parameterscope, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(parameterscope);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "ParameterScope")}
                SET ParameterId = @ParameterId,
                    ParameterGuid = @ParameterGuid,
                    Overridevalue = @Overridevalue,
                    CompanyId = @CompanyId,
                    CompanyGuid = @CompanyGuid,
                    CompanyOfficeId = @CompanyOfficeId,
                    CompanyOfficeGuid = @CompanyOfficeGuid,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE ParameterscopeGuid = @ParameterscopeGuid";
            await conn.ExecuteAsync(sql, parameterscope, transaction);
        }

        public async Task SoftDeleteParameterscopeAsync(Parameterscope parameterscope, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(parameterscope, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_dialect.Qualify("app", "ParameterScope")}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE ParameterscopeGuid = @ParameterscopeGuid";

            await conn.ExecuteAsync(sql, parameterscope, transaction);
        }

        public async Task DeleteParameterscopeAsync(Guid parameterscopeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "ParameterScope")} WHERE ParameterscopeGuid = @parameterscopeGuid";
            await conn.ExecuteAsync(sql, new { parameterscopeGuid }, transaction);
        }

        public async Task DeleteByParameterGuidAsync(Guid parameterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_dialect.Qualify("app", "ParameterScope")} WHERE ParameterGuid = @parameterGuid";
            await conn.ExecuteAsync(sql, new { parameterGuid }, transaction);
        }

        public async Task<IEnumerable<Parameterscope>> SearchParameterscopeAsync(
            string? overridevalue, string? overridevalueSearchType,
            Guid parameterGuid, Guid parameterscopeGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (parameterGuid != Guid.Empty)
            {
                whereClauses.Add("a.ParameterGuid = @parameterGuid");
                parameters.Add("@parameterGuid", parameterGuid);
            }
            if (parameterscopeGuid != Guid.Empty)
            {
                whereClauses.Add("a.ParameterscopeGuid = @parameterscopeGuid");
                parameters.Add("@parameterscopeGuid", parameterscopeGuid);
            }

            if (!string.IsNullOrWhiteSpace(overridevalue))
            {
                var param = SqlFilterHelper.BuildFilter("a.Overridevalue", "@overridevalue", overridevalueSearchType, parameters, "overridevalue", overridevalue, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParameterId.Parametername AS ParameterName
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName

                FROM {_dialect.Qualify("app", "ParameterScope")} a
LEFT JOIN {_dialect.Qualify("app", "Parameter")} j_ParameterId ON a.ParameterId = j_ParameterId.ParameterId
LEFT JOIN {_dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {_dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.ParameterscopeId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Parameterscope>(sql, parameters, transaction);
        }

        public async Task<string?> GetEffectiveParameterValueAsync(string parameterName, long? companyId, long? officeId)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, 1, "Value")}
                FROM (
                    -- 1. Office Override
                    SELECT ps.OverrideValue AS Value, 1 AS Priority
                    FROM {_dialect.Qualify("app", "ParameterScope")} ps
                    INNER JOIN {_dialect.Qualify("app", "Parameter")} p ON ps.ParameterId = p.ParameterId
                    WHERE p.ParameterName = @parameterName
                      AND ps.CompanyId = @companyId
                      AND ps.CompanyOfficeId = @officeId
                      AND ps.StatusId > 0
                      AND ps.DeletedTime IS NULL
                      AND @officeId IS NOT NULL

                    UNION ALL

                    -- 2. Company Override
                    SELECT ps.OverrideValue AS Value, 2 AS Priority
                    FROM {_dialect.Qualify("app", "ParameterScope")} ps
                    INNER JOIN {_dialect.Qualify("app", "Parameter")} p ON ps.ParameterId = p.ParameterId
                    WHERE p.ParameterName = @parameterName
                      AND ps.CompanyId = @companyId
                      AND ps.CompanyOfficeId IS NULL
                      AND ps.StatusId > 0
                      AND ps.DeletedTime IS NULL
                      AND @companyId IS NOT NULL

                    UNION ALL

                    -- 3. Global Value
                    SELECT p.ParameterValue AS Value, 3 AS Priority
                    FROM {_dialect.Qualify("app", "Parameter")} p
                    WHERE p.ParameterName = @parameterName
                      AND p.StatusId > 0
                      AND p.DeletedTime IS NULL
                ) a
                ORDER BY Priority ASC{_dialect.AppendLimit(1)}";
            return await connection.QueryFirstOrDefaultAsync<string>(sql, new { parameterName, companyId, officeId });
        }
    }
}
