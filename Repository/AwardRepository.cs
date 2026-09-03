using BamboeUp.Audit.Contracts;
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
    public partial class AwardRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IAwardRepository
    {
        private readonly string _table = dialect.Qualify("app", "Award");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Award?> GetAwardAsync(Guid awardGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'AwardType')
    WHERE StandardReferenceItemId = a.SrAwardType
) v_SrAwardType

                WHERE a.AwardGuid = @awardGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Award>(sql, new { awardGuid });
        }

        public async Task<Award?> GetAwardByIdAsync(long awardId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'AwardType')
    WHERE StandardReferenceItemId = a.SrAwardType
) v_SrAwardType

                WHERE a.AwardId = @awardId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Award>(sql, new { awardId });
        }

        public async Task<IEnumerable<Award>> GetAllAwardsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'AwardType')
    WHERE StandardReferenceItemId = a.SrAwardType
) v_SrAwardType

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AwardId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Award>(sql);
        }

        public async Task CreateAwardAsync(Award award, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(award);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (AwardGuid, CreatedById, StatusId, CreatedTime, AwardCode, AwardName, SrAwardType, ValidFrom, Validto, AwardPrize, Note)
                VALUES
                (@AwardGuid, @CreatedById, @StatusId, @CreatedTime, @AwardCode, @AwardName, @SrAwardType, @ValidFrom, @Validto, @AwardPrize, @Note);
                {dialect.ReturnIdentitySuffix("AwardId")}";
            award.AwardId = await conn.QuerySingleAsync<long>(sql, award, transaction);
        }

        public async Task UpdateAwardAsync(Award award, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(award);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     AwardCode = @AwardCode,
                    AwardName = @AwardName,
                    SrAwardType = @SrAwardType,
                    ValidFrom = @ValidFrom,
                    Validto = @Validto,
                    AwardPrize = @AwardPrize,
                    Note = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE AwardGuid = @AwardGuid";
            await conn.ExecuteAsync(sql, award, transaction);
        }

        public async Task SoftDeleteAwardAsync(Award award, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(award, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AwardGuid = @AwardGuid";

            await conn.ExecuteAsync(sql, award, transaction);
        }

        public async Task DeleteAwardAsync(Guid awardGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE AwardGuid = @awardGuid";
            await conn.ExecuteAsync(sql, new { awardGuid }, transaction);
        }

        public async Task<IEnumerable<Award>> SearchAwardAsync(
            string? awardCode,
            string? awardCodeSearchType,
            string? awardName,
            string? awardNameSearchType,
            string? validFrom,
            string? validFromSearchType,
            string? validto,
            string? validtoSearchType,
            string? awardPrize,
            string? awardPrizeSearchType,
            string? note,
            string? noteSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(awardCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardCode", "@awardCode", awardCodeSearchType, parameters, "awardCode", awardCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(awardName))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardName", "@awardName", awardNameSearchType, parameters, "awardName", awardName, dialect);
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

            if (!string.IsNullOrWhiteSpace(awardPrize))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardPrize", "@awardPrize", awardPrizeSearchType, parameters, "awardPrize", awardPrize, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAwardType.StandardReferenceItemName")} AS AwardTypeName


                FROM {_table} a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'AwardType')
    WHERE StandardReferenceItemId = a.SrAwardType
) v_SrAwardType

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AwardId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Award>(sql, parameters, transaction);
        }
    }
}
