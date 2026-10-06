using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class TempContoh1Repository(RepositoryContext context, ISqlDialect dialect) : ITempContoh1Repository
    {
        private readonly string _table = dialect.Qualify("app", "TempContoh1");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<TempContoh1?> GetTempContoh1Async(Guid tempContoh1Guid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAwardType", "SrAwardType", "AwardType")}


                WHERE a.TempContoh1Guid = @tempContoh1Guid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<TempContoh1>(sql, new { tempContoh1Guid });
        }

        public async Task<IEnumerable<TempContoh1>> GetAllTempContoh1sAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName



                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAwardType", "SrAwardType", "AwardType")}


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.TempContoh1Id DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<TempContoh1>(sql);
        }

        public async Task CreateTempContoh1Async(TempContoh1 tempContoh1, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (TempContoh1Guid, CreatedById, StatusId, CreatedTime, PersonId, SrAwardType
                )
                VALUES
                (@TempContoh1Guid, @CreatedById, @StatusId, @CreatedTime, @PersonId, @SrAwardType
                ){dialect.ReturnIdentitySuffix("TempContoh1Id")}";
            tempContoh1.TempContoh1Id = await conn.QuerySingleAsync<long>(sql, tempContoh1, transaction);
        }

        public async Task UpdateTempContoh1Async(TempContoh1 tempContoh1, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     PersonId = @PersonId,
                    SrAwardType = @SrAwardType,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE TempContoh1Guid = @TempContoh1Guid";
            await conn.ExecuteAsync(sql, tempContoh1, transaction);
        }

        public async Task SoftDeleteTempContoh1Async(TempContoh1 tempContoh1, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE TempContoh1Guid = @TempContoh1Guid";

            await conn.ExecuteAsync(sql, tempContoh1, transaction);
        }

        public async Task DeleteTempContoh1Async(Guid tempContoh1Guid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE TempContoh1Guid = @tempContoh1Guid";
            await conn.ExecuteAsync(sql, new { tempContoh1Guid }, transaction);
        }

        public async Task<IEnumerable<TempContoh1>> SearchTempContoh1Async(

string? personName, string? personNameSearchType,
string? awardTypeName, string? awardTypeNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();


if (!string.IsNullOrWhiteSpace(personName)) { var param = SqlFilterHelper.BuildFilter("j_PersonId.PersonName", "@personName", personNameSearchType, parameters, "personName", personName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(awardTypeName)) { var param = SqlFilterHelper.BuildFilter("v_SrAwardType.StandardReferenceItemName", "@awardTypeName", awardTypeNameSearchType, parameters, "awardTypeName", awardTypeName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAwardType", "SrAwardType", "AwardType")}


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.TempContoh1Id DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<TempContoh1>(sql, parameters, transaction);
        }
    }
}
