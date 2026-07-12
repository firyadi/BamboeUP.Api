using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class AwardRepository : IAwardRepository
    {
        private readonly RepositoryContext _context;

        public AwardRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<Award?> GetAwardAsync(Guid awardGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Award] a


                WHERE a.AwardGuid = @awardGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Award>(sql, new { awardGuid });
        }

        public async Task<IEnumerable<Award>> GetAllAwardsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Award] a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AwardId DESC";
            return await connection.QueryAsync<Award>(sql);
        }

        public async Task CreateAwardAsync(Award award, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Award]
                (AwardGuid, CreatedById, StatusId, CreatedTime, AwardCode, AwardName, SrAwardCriteria, SrAwardType, ValidFrom, Validto, AwardPrize, Note
                )
                VALUES
                (@AwardGuid, @CreatedById, @StatusId, @CreatedTime, @AwardCode, @AwardName, @SrAwardCriteria, @SrAwardType, @ValidFrom, @Validto, @AwardPrize, @Note
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            award.AwardId = await conn.QuerySingleAsync<long>(sql, award, transaction);
        }

        public async Task UpdateAwardAsync(Award award, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Award]
                SET                     AwardCode = @AwardCode,
                    AwardName = @AwardName,
                    SrAwardCriteria = @SrAwardCriteria,
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
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Award]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AwardGuid = @AwardGuid";

            await conn.ExecuteAsync(sql, award, transaction);
        }

        public async Task DeleteAwardAsync(Guid awardGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Award] WHERE AwardGuid = @awardGuid";
            await conn.ExecuteAsync(sql, new { awardGuid }, transaction);
        }

        public async Task<IEnumerable<Award>> SearchAwardAsync(
            string? awardCode,
            string? awardCodeSearchType,
            string? awardName,
            string? awardNameSearchType,
            string? srAwardCriteria,
            string? srAwardCriteriaSearchType,
            string? srAwardType,
            string? srAwardTypeSearchType,
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
            var connection = transaction?.Connection ?? _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(awardCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardCode", "@awardCode", awardCodeSearchType, parameters, "awardCode", awardCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(awardName))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardName", "@awardName", awardNameSearchType, parameters, "awardName", awardName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAwardCriteria))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAwardCriteria", "@srAwardCriteria", srAwardCriteriaSearchType, parameters, "srAwardCriteria", srAwardCriteria);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srAwardType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAwardType", "@srAwardType", srAwardTypeSearchType, parameters, "srAwardType", srAwardType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(validFrom))
            {
                var param = SqlFilterHelper.BuildFilter("a.ValidFrom", "@validFrom", validFromSearchType, parameters, "validFrom", validFrom);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(validto))
            {
                var param = SqlFilterHelper.BuildFilter("a.Validto", "@validto", validtoSearchType, parameters, "validto", validto);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(awardPrize))
            {
                var param = SqlFilterHelper.BuildFilter("a.AwardPrize", "@awardPrize", awardPrizeSearchType, parameters, "awardPrize", awardPrize);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Award] a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AwardId DESC";

            return await connection.QueryAsync<Award>(sql, parameters, transaction);
        }
    }
}
