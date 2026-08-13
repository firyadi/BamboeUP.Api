using BamboeUp.Audit.Contracts;
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
    public partial class AwardCriteriaRepository(RepositoryContext context, IAuditService audit) : IAwardCriteriaRepository
    {

        public async Task<AwardCriteria?> GetAwardCriteriaAsync(Guid awardCriteriaGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AwardId.AwardName AS AwardName


                FROM [app].[AwardCriteria] a
LEFT JOIN [app].[Award] j_AwardId ON a.AwardId = j_AwardId.AwardId

                WHERE a.AwardCriteriaGuid = @awardCriteriaGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AwardCriteria>(sql, new { awardCriteriaGuid });
        }

        public async Task<AwardCriteria?> GetByAwardGuidAndAwardCriteriaGuidAsync(Guid awardGuid, Guid awardCriteriaGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AwardId.AwardName AS AwardName


                FROM [app].[AwardCriteria] a
LEFT JOIN [app].[Award] j_AwardId ON a.AwardId = j_AwardId.AwardId

                WHERE j_AwardId.AwardGuid = @awardGuid
                  AND a.AwardCriteriaGuid = @awardCriteriaGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AwardCriteria>(sql, new { awardGuid, awardCriteriaGuid });
        }

        public async Task<IEnumerable<AwardCriteria>> GetAllByAwardGuidAsync(Guid awardGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AwardId.AwardName AS AwardName


                FROM [app].[AwardCriteria] a
LEFT JOIN [app].[Award] j_AwardId ON a.AwardId = j_AwardId.AwardId

                WHERE j_AwardId.AwardGuid = @awardGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.AwardCriteriaId DESC";
            return await connection.QueryAsync<AwardCriteria>(sql, new { awardGuid });
        }

        public async Task<IEnumerable<AwardCriteria>> GetAllByAwardIdAsync(long awardId)
        {
            using var connection = context.CreateConnection();
            const string sql = @"
                SELECT *
                FROM [app].[AwardCriteria]
                WHERE AwardId = @awardId
                  AND StatusId > 0
                  AND DeletedTime IS NULL
                ORDER BY AwardCriteriaId DESC";
            return await connection.QueryAsync<AwardCriteria>(sql, new { awardId });
        }

        public async Task CreateAwardCriteriaAsync(AwardCriteria awardCriteria, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(awardCriteria);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[AwardCriteria]
                ([AwardCriteriaGuid], [AwardId], [CreatedById], [StatusId], [CreatedTime], [LineNo], [CriteriaCode], [CriteriaName], [MinimumScore], [MaximumScore], [Weight], [Note])
                VALUES
                (@AwardCriteriaGuid, @AwardId, @CreatedById, @StatusId, @CreatedTime, @LineNo, @CriteriaCode, @CriteriaName, @MinimumScore, @MaximumScore, @Weight, @Note);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            awardCriteria.AwardCriteriaId = await conn.QuerySingleAsync<long>(sql, awardCriteria, transaction);
        }

        public async Task UpdateAwardCriteriaAsync(AwardCriteria awardCriteria, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(awardCriteria);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[AwardCriteria]
                SET [AwardId] = @AwardId,
                    [LineNo] = @LineNo,
                    [CriteriaCode] = @CriteriaCode,
                    [CriteriaName] = @CriteriaName,
                    [MinimumScore] = @MinimumScore,
                    [MaximumScore] = @MaximumScore,
                    [Weight] = @Weight,
                    [Note] = @Note,
                    [StatusId] = @StatusId,
                    [UpdatedById] = @UpdatedById,
                    [UpdatedTime] = @UpdatedTime
                WHERE [AwardCriteriaGuid] = @AwardCriteriaGuid";
            await conn.ExecuteAsync(sql, awardCriteria, transaction);
        }

        public async Task SoftDeleteAwardCriteriaAsync(AwardCriteria awardCriteria, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(awardCriteria, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[AwardCriteria]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AwardCriteriaGuid = @AwardCriteriaGuid";

            await conn.ExecuteAsync(sql, awardCriteria, transaction);
        }

        public async Task DeleteAwardCriteriaAsync(Guid awardCriteriaGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[AwardCriteria] WHERE AwardCriteriaGuid = @awardCriteriaGuid";
            await conn.ExecuteAsync(sql, new { awardCriteriaGuid }, transaction);
        }

        public async Task DeleteByAwardGuidAsync(Guid awardGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[AwardCriteria] WHERE AwardId IN (SELECT AwardId FROM [app].[Award] WHERE AwardGuid = @awardGuid)";
            await conn.ExecuteAsync(sql, new { awardGuid }, transaction);
        }

        public async Task<IEnumerable<AwardCriteria>> SearchAwardCriteriaAsync(
            string? lineNo,
            string? lineNoSearchType,
            string? criteriaCode,
            string? criteriaCodeSearchType,
            string? criteriaName,
            string? criteriaNameSearchType,
            string? minimumScore,
            string? minimumScoreSearchType,
            string? maximumScore,
            string? maximumScoreSearchType,
            string? weight,
            string? weightSearchType,
            string? note,
            string? noteSearchType,
            Guid awardGuid, Guid awardCriteriaGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (awardGuid != Guid.Empty)
            {
                whereClauses.Add("j_AwardId.AwardGuid = @awardGuid");
                parameters.Add("@awardGuid", awardGuid);
            }
            if (awardCriteriaGuid != Guid.Empty)
            {
                whereClauses.Add("a.AwardCriteriaGuid = @awardCriteriaGuid");
                parameters.Add("@awardCriteriaGuid", awardCriteriaGuid);
            }

            if (!string.IsNullOrWhiteSpace(lineNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.LineNo", "@lineNo", lineNoSearchType, parameters, "lineNo", lineNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(criteriaCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.CriteriaCode", "@criteriaCode", criteriaCodeSearchType, parameters, "criteriaCode", criteriaCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(criteriaName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CriteriaName", "@criteriaName", criteriaNameSearchType, parameters, "criteriaName", criteriaName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(minimumScore))
            {
                var param = SqlFilterHelper.BuildFilter("a.MinimumScore", "@minimumScore", minimumScoreSearchType, parameters, "minimumScore", minimumScore);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(maximumScore))
            {
                var param = SqlFilterHelper.BuildFilter("a.MaximumScore", "@maximumScore", maximumScoreSearchType, parameters, "maximumScore", maximumScore);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(weight))
            {
                var param = SqlFilterHelper.BuildFilter("a.Weight", "@weight", weightSearchType, parameters, "weight", weight);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_AwardId.AwardName AS AwardName


                FROM [app].[AwardCriteria] a
LEFT JOIN [app].[Award] j_AwardId ON a.AwardId = j_AwardId.AwardId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AwardCriteriaId DESC";

            return await connection.QueryAsync<AwardCriteria>(sql, parameters, transaction);
        }
    }
}
