using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class PositionGradeRepository(RepositoryContext context, ISqlDialect dialect) : IPositionGradeRepository
    {
        private readonly string _table = dialect.Qualify("app", "PositionGrade");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<PositionGrade?> GetPositionGradeAsync(Guid positionGradeGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE a.PositionGradeGuid = @positionGradeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PositionGrade>(sql, new { positionGradeGuid });
        }

        public async Task<IEnumerable<PositionGrade>> GetAllPositionGradesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}



                FROM {_table} a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.PositionGradeId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PositionGrade>(sql);
        }

        public async Task CreatePositionGradeAsync(PositionGrade positionGrade, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PositionGradeGuid, CreatedById, StatusId, CreatedTime, PositionGradeCode, PositionGradeName, Interval, Ranking, PositionGradeDescription
                )
                VALUES
                (@PositionGradeGuid, @CreatedById, @StatusId, @CreatedTime, @PositionGradeCode, @PositionGradeName, @Interval, @Ranking, @PositionGradeDescription
                ){dialect.ReturnIdentitySuffix("PositionGradeId")}";
            positionGrade.PositionGradeId = await conn.QuerySingleAsync<long>(sql, positionGrade, transaction);
        }

        public async Task UpdatePositionGradeAsync(PositionGrade positionGrade, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     PositionGradeCode = @PositionGradeCode,
                    PositionGradeName = @PositionGradeName,
                    Interval = @Interval,
                    Ranking = @Ranking,
                    PositionGradeDescription = @PositionGradeDescription,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PositionGradeGuid = @PositionGradeGuid";
            await conn.ExecuteAsync(sql, positionGrade, transaction);
        }

        public async Task SoftDeletePositionGradeAsync(PositionGrade positionGrade, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PositionGradeGuid = @PositionGradeGuid";

            await conn.ExecuteAsync(sql, positionGrade, transaction);
        }

        public async Task DeletePositionGradeAsync(Guid positionGradeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PositionGradeGuid = @positionGradeGuid";
            await conn.ExecuteAsync(sql, new { positionGradeGuid }, transaction);
        }

        public async Task<IEnumerable<PositionGrade>> SearchPositionGradeAsync(
            string? positionGradeCode,
            string? positionGradeCodeSearchType,
            string? positionGradeName,
            string? positionGradeNameSearchType,
            string? interval,
            string? intervalSearchType,
            string? ranking,
            string? rankingSearchType,
            string? positionGradeDescription,
            string? positionGradeDescriptionSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(positionGradeCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionGradeCode", "@positionGradeCode", positionGradeCodeSearchType, parameters, "positionGradeCode", positionGradeCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionGradeName))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionGradeName", "@positionGradeName", positionGradeNameSearchType, parameters, "positionGradeName", positionGradeName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(interval))
            {
                var param = SqlFilterHelper.BuildFilter("a.Interval", "@interval", intervalSearchType, parameters, "interval", interval, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(ranking))
            {
                var param = SqlFilterHelper.BuildFilter("a.Ranking", "@ranking", rankingSearchType, parameters, "ranking", ranking, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(positionGradeDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.PositionGradeDescription", "@positionGradeDescription", positionGradeDescriptionSearchType, parameters, "positionGradeDescription", positionGradeDescription, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}


                FROM {_table} a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PositionGradeId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PositionGrade>(sql, parameters, transaction);
        }
    }
}
