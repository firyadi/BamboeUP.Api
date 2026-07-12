using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public partial class ParameterRepository : IParameterRepository
    {
        private readonly RepositoryContext _context;

        public ParameterRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Parameter?> GetParameterAsync(Guid parameterGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Parameter] a

                WHERE a.ParameterGuid = @parameterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Parameter>(sql, new { parameterGuid });
        }

        public async Task<Parameter?> GetParameterByIdAsync(long parameterId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Parameter] a

                WHERE a.ParameterId = @parameterId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Parameter>(sql, new { parameterId });
        }

        public async Task<IEnumerable<Parameter>> GetAllParametersAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Parameter] a

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.ParameterId DESC";
            return await connection.QueryAsync<Parameter>(sql);
        }

        public async Task CreateParameterAsync(Parameter parameter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(parameter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Parameter]
                (ParameterGuid, CreatedById, StatusId, CreatedTime, Parametername, Parametervalue

                )
                VALUES
                (@ParameterGuid, @CreatedById, @StatusId, @CreatedTime, @Parametername, @Parametervalue

                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            parameter.ParameterId = await conn.QuerySingleAsync<long>(sql, parameter, transaction);
        }

        public async Task UpdateParameterAsync(Parameter parameter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(parameter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Parameter]
                SET Parametername = @Parametername,
                    Parametervalue = @Parametervalue,

                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE ParameterGuid = @ParameterGuid";
            await conn.ExecuteAsync(sql, parameter, transaction);
        }

        public async Task SoftDeleteParameterAsync(Parameter parameter, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(parameter, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Parameter]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE ParameterGuid = @ParameterGuid";

            await conn.ExecuteAsync(sql, parameter, transaction);
        }

        public async Task DeleteParameterAsync(Guid parameterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Parameter] WHERE ParameterGuid = @parameterGuid";
            await conn.ExecuteAsync(sql, new { parameterGuid }, transaction);
        }

        public async Task<IEnumerable<Parameter>> SearchParameterAsync(
            string? parametername, string? parameternameSearchType,
            string? parametervalue, string? parametervalueSearchType,

            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(parametername))
            {
                var param = SqlFilterHelper.BuildFilter("a.Parametername", "@parametername", parameternameSearchType, parameters, "parametername", parametername);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(parametervalue))
            {
                var param = SqlFilterHelper.BuildFilter("a.Parametervalue", "@parametervalue", parametervalueSearchType, parameters, "parametervalue", parametervalue);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*

                FROM [app].[Parameter] a

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.ParameterId DESC";

            return await connection.QueryAsync<Parameter>(sql, parameters, transaction);
        }
    }
}
