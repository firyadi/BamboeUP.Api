using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class PositionLevelRepository(RepositoryContext context, ISqlDialect dialect) : IPositionLevelRepository
    {
        private readonly string _table = dialect.Qualify("app", "PositionLevel");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<PositionLevel?> GetPositionLevelAsync(Guid positionLevelGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.PositionLevelGuid = @positionLevelGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PositionLevel>(sql, new { positionLevelGuid });
        }

        public async Task<IEnumerable<PositionLevel>> GetAllPositionLevelsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.PositionLevelId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PositionLevel>(sql);
        }

        public async Task CreatePositionLevelAsync(PositionLevel positionLevel, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PositionLevelGuid, CreatedById, StatusId, CreatedTime, PositionLevelCode, PositionLevelName, Ranking, PositionLevelDescription
                )
                VALUES
                (@PositionLevelGuid, @CreatedById, @StatusId, @CreatedTime, @PositionLevelCode, @PositionLevelName, @Ranking, @PositionLevelDescription
                ){dialect.ReturnIdentitySuffix("PositionLevelId")}";
            positionLevel.PositionLevelId = await conn.QuerySingleAsync<long>(sql, positionLevel, transaction);
        }

        public async Task UpdatePositionLevelAsync(PositionLevel positionLevel, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     PositionLevelCode = @PositionLevelCode,
                    PositionLevelName = @PositionLevelName,
                    Ranking = @Ranking,
                    PositionLevelDescription = @PositionLevelDescription,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PositionLevelGuid = @PositionLevelGuid";
            await conn.ExecuteAsync(sql, positionLevel, transaction);
        }

        public async Task SoftDeletePositionLevelAsync(PositionLevel positionLevel, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PositionLevelGuid = @PositionLevelGuid";

            await conn.ExecuteAsync(sql, positionLevel, transaction);
        }

        public async Task DeletePositionLevelAsync(Guid positionLevelGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PositionLevelGuid = @positionLevelGuid";
            await conn.ExecuteAsync(sql, new { positionLevelGuid }, transaction);
        }

        public async Task<IEnumerable<PositionLevel>> SearchPositionLevelAsync(
            string? positionLevelCode,
            string? positionLevelCodeSearchType,
            string? positionLevelName,
            string? positionLevelNameSearchType,
            string? ranking,
            string? rankingSearchType,
            string? positionLevelDescription,
            string? positionLevelDescriptionSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(positionLevelCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionLevelCode", "@positionLevelCode", positionLevelCodeSearchType, parameters, "positionLevelCode", positionLevelCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionLevelName))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionLevelName", "@positionLevelName", positionLevelNameSearchType, parameters, "positionLevelName", positionLevelName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(ranking))
            {
                var param = SqlFilterHelper.BuildFilter("a.Ranking", "@ranking", rankingSearchType, parameters, "ranking", ranking, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionLevelDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionLevelDescription", "@positionLevelDescription", positionLevelDescriptionSearchType, parameters, "positionLevelDescription", positionLevelDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PositionLevelId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PositionLevel>(sql, parameters, transaction);
        }
    }
}
