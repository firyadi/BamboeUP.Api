using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class PositionRepository(RepositoryContext context, ISqlDialect dialect) : IPositionRepository
    {
        private readonly string _table = dialect.Qualify("app", "Position");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Position?> GetPositionAsync(Guid positionGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PositionGradeID.PositionGradeName AS PositionGradeName
, j_PositionLevelID.PositionLevelName AS PositionLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "PositionGrade")} j_PositionGradeID ON a.PositionGradeID = j_PositionGradeID.PositionGradeId
LEFT JOIN {dialect.Qualify("app", "PositionLevel")} j_PositionLevelID ON a.PositionLevelID = j_PositionLevelID.PositionLevelId


                WHERE a.PositionGuid = @positionGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Position>(sql, new { positionGuid });
        }

        public async Task<IEnumerable<Position>> GetAllPositionsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PositionGradeID.PositionGradeName AS PositionGradeName
, j_PositionLevelID.PositionLevelName AS PositionLevelName



                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "PositionGrade")} j_PositionGradeID ON a.PositionGradeID = j_PositionGradeID.PositionGradeId
LEFT JOIN {dialect.Qualify("app", "PositionLevel")} j_PositionLevelID ON a.PositionLevelID = j_PositionLevelID.PositionLevelId


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.PositionId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Position>(sql);
        }

        public async Task CreatePositionAsync(Position position, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PositionGuid, CreatedById, StatusId, CreatedTime, PositionCode, PositionName, Summary, PositionGradeId, PositionLevelId, ValidFrom, Validto, PositionDescription
                )
                VALUES
                (@PositionGuid, @CreatedById, @StatusId, @CreatedTime, @PositionCode, @PositionName, @Summary, @PositionGradeId, @PositionLevelId, @ValidFrom, @Validto, @PositionDescription
                ){dialect.ReturnIdentitySuffix("PositionId")}";
            position.PositionId = await conn.QuerySingleAsync<long>(sql, position, transaction);
        }

        public async Task UpdatePositionAsync(Position position, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     PositionCode = @PositionCode,
                    PositionName = @PositionName,
                    Summary = @Summary,
                    PositionGradeId = @PositionGradeId,
                    PositionLevelId = @PositionLevelId,
                    ValidFrom = @ValidFrom,
                    Validto = @Validto,
                    PositionDescription = @PositionDescription,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PositionGuid = @PositionGuid";
            await conn.ExecuteAsync(sql, position, transaction);
        }

        public async Task SoftDeletePositionAsync(Position position, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PositionGuid = @PositionGuid";

            await conn.ExecuteAsync(sql, position, transaction);
        }

        public async Task DeletePositionAsync(Guid positionGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PositionGuid = @positionGuid";
            await conn.ExecuteAsync(sql, new { positionGuid }, transaction);
        }

        public async Task<IEnumerable<Position>> SearchPositionAsync(
            string? positionCode,
            string? positionCodeSearchType,
            string? positionName,
            string? positionNameSearchType,
            string? summary,
            string? summarySearchType,
            string? validFrom,
            string? validFromSearchType,
            string? validto,
            string? validtoSearchType,
            string? positionDescription,
            string? positionDescriptionSearchType,
string? positionGradeName, string? positionGradeNameSearchType,
string? positionLevelName, string? positionLevelNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(positionCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionCode", "@positionCode", positionCodeSearchType, parameters, "positionCode", positionCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionName))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionName", "@positionName", positionNameSearchType, parameters, "positionName", positionName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(summary))
            {
                var param = SqlFilterHelper.BuildFilter("a.Summary", "@summary", summarySearchType, parameters, "summary", summary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(validFrom))
            {
                var param = SqlFilterHelper.BuildFilter("a.ValidFrom", "@validFrom", validFromSearchType, parameters, "validFrom", validFrom, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(validto))
            {
                var param = SqlFilterHelper.BuildFilter("a.Validto", "@validto", validtoSearchType, parameters, "validto", validto, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionDescription", "@positionDescription", positionDescriptionSearchType, parameters, "positionDescription", positionDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(positionGradeName)) { var param = SqlFilterHelper.BuildFilter("j_PositionGradeID.PositionGradeName", "@positionGradeName", positionGradeNameSearchType, parameters, "positionGradeName", positionGradeName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(positionLevelName)) { var param = SqlFilterHelper.BuildFilter("j_PositionLevelID.PositionLevelName", "@positionLevelName", positionLevelNameSearchType, parameters, "positionLevelName", positionLevelName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PositionGradeID.PositionGradeName AS PositionGradeName
, j_PositionLevelID.PositionLevelName AS PositionLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "PositionGrade")} j_PositionGradeID ON a.PositionGradeID = j_PositionGradeID.PositionGradeId
LEFT JOIN {dialect.Qualify("app", "PositionLevel")} j_PositionLevelID ON a.PositionLevelID = j_PositionLevelID.PositionLevelId


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PositionId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Position>(sql, parameters, transaction);
        }
    }
}
