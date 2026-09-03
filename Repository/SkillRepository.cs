using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class SkillRepository(RepositoryContext context, ISqlDialect dialect) : ISkillRepository
    {
        private readonly string _table = dialect.Qualify("app", "Skill");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Skill?> GetSkillAsync(Guid skillGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE a.SkillGuid = @skillGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Skill>(sql, new { skillGuid });
        }

        public async Task<IEnumerable<Skill>> GetAllSkillsAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.SkillId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Skill>(sql);
        }

        public async Task CreateSkillAsync(Skill skill, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (SkillGuid, CreatedById, StatusId, CreatedTime, SkillCode, SkillName, SkillShortName, SrSkillCategory, SrSkillType, SrDefaultSkillLevel, IsCertificationRequired, IsMandatory, FileStorageId, Description, IsActive
                )
                VALUES
                (@SkillGuid, @CreatedById, @StatusId, @CreatedTime, @SkillCode, @SkillName, @SkillShortName, @SrSkillCategory, @SrSkillType, @SrDefaultSkillLevel, @IsCertificationRequired, @IsMandatory, @FileStorageId, @Description, @IsActive
                ){dialect.ReturnIdentitySuffix("SkillId")}";
            skill.SkillId = await conn.QuerySingleAsync<long>(sql, skill, transaction);
        }

        public async Task UpdateSkillAsync(Skill skill, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     SkillCode = @SkillCode,
                    SkillName = @SkillName,
                    SkillShortName = @SkillShortName,
                    SrSkillCategory = @SrSkillCategory,
                    SrSkillType = @SrSkillType,
                    SrDefaultSkillLevel = @SrDefaultSkillLevel,
                    IsCertificationRequired = @IsCertificationRequired,
                    IsMandatory = @IsMandatory,
                    FileStorageId = @FileStorageId,
                    Description = @Description,
                    IsActive = @IsActive,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE SkillGuid = @SkillGuid";
            await conn.ExecuteAsync(sql, skill, transaction);
        }

        public async Task SoftDeleteSkillAsync(Skill skill, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE SkillGuid = @SkillGuid";

            await conn.ExecuteAsync(sql, skill, transaction);
        }

        public async Task DeleteSkillAsync(Guid skillGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE SkillGuid = @skillGuid";
            await conn.ExecuteAsync(sql, new { skillGuid }, transaction);
        }

        public async Task<IEnumerable<Skill>> SearchSkillAsync(
            string? skillCode,
            string? skillCodeSearchType,
            string? skillName,
            string? skillNameSearchType,
            string? skillShortName,
            string? skillShortNameSearchType,
            string? srSkillCategory,
            string? srSkillCategorySearchType,
            string? srSkillType,
            string? srSkillTypeSearchType,
            string? srDefaultSkillLevel,
            string? srDefaultSkillLevelSearchType,
            string? isCertificationRequired,
            string? isCertificationRequiredSearchType,
            string? isMandatory,
            string? isMandatorySearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(skillCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.SkillCode", "@skillCode", skillCodeSearchType, parameters, "skillCode", skillCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(skillName))
            {
                var param = SqlFilterHelper.BuildFilter("a.SkillName", "@skillName", skillNameSearchType, parameters, "skillName", skillName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(skillShortName))
            {
                var param = SqlFilterHelper.BuildFilter("a.SkillShortName", "@skillShortName", skillShortNameSearchType, parameters, "skillShortName", skillShortName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srSkillCategory))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrSkillCategory", "@srSkillCategory", srSkillCategorySearchType, parameters, "srSkillCategory", srSkillCategory, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srSkillType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrSkillType", "@srSkillType", srSkillTypeSearchType, parameters, "srSkillType", srSkillType, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srDefaultSkillLevel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrDefaultSkillLevel", "@srDefaultSkillLevel", srDefaultSkillLevelSearchType, parameters, "srDefaultSkillLevel", srDefaultSkillLevel, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCertificationRequired))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCertificationRequired", "@isCertificationRequired", isCertificationRequiredSearchType, parameters, "isCertificationRequired", isCertificationRequired, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isMandatory))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsMandatory", "@isMandatory", isMandatorySearchType, parameters, "isMandatory", isMandatory, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isActive))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsActive", "@isActive", isActiveSearchType, parameters, "isActive", isActive, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
                FROM {_table} a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.SkillId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Skill>(sql, parameters, transaction);
        }
    }
}
