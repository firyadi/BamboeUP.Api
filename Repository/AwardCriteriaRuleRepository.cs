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
    public partial class AwardCriteriaRuleRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IAwardCriteriaRuleRepository
    {
        private readonly string _table = dialect.Qualify("app", "AwardCriteriaRule");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<AwardCriteriaRule?> GetAwardCriteriaRuleAsync(Guid awardCriteriaRuleGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_AwardCriteriaId.AwardCriteriaName AS AwardCriteriaName


                FROM {_table} a
LEFT JOIN [app].[AwardCriteria] j_AwardCriteriaId ON a.AwardCriteriaId = j_AwardCriteriaId.AwardCriteriaId

                WHERE a.AwardCriteriaRuleGuid = @awardCriteriaRuleGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AwardCriteriaRule>(sql, new { awardCriteriaRuleGuid });
        }

        public async Task<AwardCriteriaRule?> GetByAwardCriteriaGuidAndAwardCriteriaRuleGuidAsync(Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_AwardCriteriaId.AwardCriteriaName AS AwardCriteriaName


                FROM {_table} a
LEFT JOIN [app].[AwardCriteria] j_AwardCriteriaId ON a.AwardCriteriaId = j_AwardCriteriaId.AwardCriteriaId

                WHERE j_AwardCriteriaId.AwardCriteriaGuid = @awardCriteriaGuid
                  AND a.AwardCriteriaRuleGuid = @awardCriteriaRuleGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AwardCriteriaRule>(sql, new { awardCriteriaGuid, awardCriteriaRuleGuid });
        }

        public async Task<IEnumerable<AwardCriteriaRule>> GetAllByAwardCriteriaGuidAsync(Guid awardCriteriaGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_AwardCriteriaId.AwardCriteriaName AS AwardCriteriaName


                FROM {_table} a
LEFT JOIN [app].[AwardCriteria] j_AwardCriteriaId ON a.AwardCriteriaId = j_AwardCriteriaId.AwardCriteriaId

                WHERE j_AwardCriteriaId.AwardCriteriaGuid = @awardCriteriaGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.AwardCriteriaRuleId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<AwardCriteriaRule>(sql, new { awardCriteriaGuid });
        }

        public async Task<IEnumerable<AwardCriteriaRule>> GetAllByAwardCriteriaIdAsync(long awardCriteriaId)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT *
                FROM {_table}
                WHERE AwardCriteriaId = @awardCriteriaId
                  AND StatusId > 0
                  AND DeletedTime IS NULL
                ORDER BY AwardCriteriaRuleId DESC";
            return await connection.QueryAsync<AwardCriteriaRule>(sql, new { awardCriteriaId });
        }

        public async Task CreateAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(awardCriteriaRule);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (AwardCriteriaRuleGuid, AwardCriteriaId, CreatedById, StatusId, CreatedTime, [RuleCode], [RuleName], [Operator], [ValueFrom], [Valueto], [Score], [Note])
                VALUES
                (@AwardCriteriaRuleGuid, @AwardCriteriaId, @CreatedById, @StatusId, @CreatedTime, @RuleCode, @RuleName, @Operator, @ValueFrom, @Valueto, @Score, @Note);
                {dialect.ReturnIdentitySuffix("AwardCriteriaRuleId")}";
            awardCriteriaRule.AwardCriteriaRuleId = await conn.QuerySingleAsync<long>(sql, awardCriteriaRule, transaction);
        }

        public async Task UpdateAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(awardCriteriaRule);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [AwardCriteriaId] = @AwardCriteriaId,
                    [RuleCode] = @RuleCode,
                    [RuleName] = @RuleName,
                    [Operator] = @Operator,
                    [ValueFrom] = @ValueFrom,
                    [Valueto] = @Valueto,
                    [Score] = @Score,
                    [Note] = @Note,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE AwardCriteriaRuleGuid = @AwardCriteriaRuleGuid";
            await conn.ExecuteAsync(sql, awardCriteriaRule, transaction);
        }

        public async Task SoftDeleteAwardCriteriaRuleAsync(AwardCriteriaRule awardCriteriaRule, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(awardCriteriaRule, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AwardCriteriaRuleGuid = @AwardCriteriaRuleGuid";

            await conn.ExecuteAsync(sql, awardCriteriaRule, transaction);
        }

        public async Task DeleteAwardCriteriaRuleAsync(Guid awardCriteriaRuleGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE AwardCriteriaRuleGuid = @awardCriteriaRuleGuid";
            await conn.ExecuteAsync(sql, new { awardCriteriaRuleGuid }, transaction);
        }

        public async Task DeleteByAwardCriteriaGuidAsync(Guid awardCriteriaGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE AwardCriteriaId IN (SELECT AwardCriteriaId FROM [app].[AwardCriteria] WHERE AwardCriteriaGuid = @awardCriteriaGuid)";
            await conn.ExecuteAsync(sql, new { awardCriteriaGuid }, transaction);
        }

        public async Task<IEnumerable<AwardCriteriaRule>> SearchAwardCriteriaRuleAsync(
            string? ruleCode,
            string? ruleCodeSearchType,
            string? ruleName,
            string? ruleNameSearchType,
            string? @operator,
            string? @operatorSearchType,
            string? valueFrom,
            string? valueFromSearchType,
            string? valueto,
            string? valuetoSearchType,
            string? score,
            string? scoreSearchType,
            string? note,
            string? noteSearchType,
            Guid awardCriteriaGuid, Guid awardCriteriaRuleGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (awardCriteriaGuid != Guid.Empty)
            {
                whereClauses.Add("j_AwardCriteriaId.AwardCriteriaGuid = @awardCriteriaGuid");
                parameters.Add("@awardCriteriaGuid", awardCriteriaGuid);
            }
            if (awardCriteriaRuleGuid != Guid.Empty)
            {
                whereClauses.Add("a.AwardCriteriaRuleGuid = @awardCriteriaRuleGuid");
                parameters.Add("@awardCriteriaRuleGuid", awardCriteriaRuleGuid);
            }

            if (!string.IsNullOrWhiteSpace(ruleCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.RuleCode", "@ruleCode", ruleCodeSearchType, parameters, "ruleCode", ruleCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(ruleName))
            {
                var param = SqlFilterHelper.BuildFilter("a.RuleName", "@ruleName", ruleNameSearchType, parameters, "ruleName", ruleName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(@operator))
            {
                var param = SqlFilterHelper.BuildFilter("a.Operator", "@operator", @operatorSearchType, parameters, "operator", @operator);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(valueFrom))
            {
                var param = SqlFilterHelper.BuildFilter("a.ValueFrom", "@valueFrom", valueFromSearchType, parameters, "valueFrom", valueFrom, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(valueto))
            {
                var param = SqlFilterHelper.BuildFilter("a.Valueto", "@valueto", valuetoSearchType, parameters, "valueto", valueto, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(score))
            {
                var param = SqlFilterHelper.BuildFilter("a.Score", "@score", scoreSearchType, parameters, "score", score, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_AwardCriteriaId.AwardCriteriaName AS AwardCriteriaName


                FROM {_table} a
LEFT JOIN [app].[AwardCriteria] j_AwardCriteriaId ON a.AwardCriteriaId = j_AwardCriteriaId.AwardCriteriaId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AwardCriteriaRuleId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<AwardCriteriaRule>(sql, parameters, transaction);
        }
    }
}
