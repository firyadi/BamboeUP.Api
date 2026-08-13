using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class TrainingRepository(RepositoryContext context) : ITrainingRepository
    {
        public async Task<Training?> GetTrainingAsync(Guid trainingGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Training] a


                WHERE a.TrainingGuid = @trainingGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Training>(sql, new { trainingGuid });
        }

        public async Task<IEnumerable<Training>> GetAllTrainingAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*



                FROM [app].[Training] a


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.TrainingId DESC";
            return await connection.QueryAsync<Training>(sql);
        }

        public async Task CreateTrainingAsync(Training training, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Training]
                (TrainingGuid, CreatedById, StatusId, CreatedTime, TrainingCode, TrainingName, TrainingShortName, SrTrainingCategory, SrTrainingType, SrTrainingLevel, DurationHour, ValidityMonth, SrTrainingProvider, IsMandatory, IsCertification, IsExternal, FileStorageId, Description, IsActive
                )
                VALUES
                (@TrainingGuid, @CreatedById, @StatusId, @CreatedTime, @TrainingCode, @TrainingName, @TrainingShortName, @SrTrainingCategory, @SrTrainingType, @SrTrainingLevel, @DurationHour, @ValidityMonth, @SrTrainingProvider, @IsMandatory, @IsCertification, @IsExternal, @FileStorageId, @Description, @IsActive
                );
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            training.TrainingId = await conn.QuerySingleAsync<long>(sql, training, transaction);
        }

        public async Task UpdateTrainingAsync(Training training, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Training]
                SET                     TrainingCode = @TrainingCode,
                    TrainingName = @TrainingName,
                    TrainingShortName = @TrainingShortName,
                    SrTrainingCategory = @SrTrainingCategory,
                    SrTrainingType = @SrTrainingType,
                    SrTrainingLevel = @SrTrainingLevel,
                    DurationHour = @DurationHour,
                    ValidityMonth = @ValidityMonth,
                    SrTrainingProvider = @SrTrainingProvider,
                    IsMandatory = @IsMandatory,
                    IsCertification = @IsCertification,
                    IsExternal = @IsExternal,
                    FileStorageId = @FileStorageId,
                    Description = @Description,
                    IsActive = @IsActive,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE TrainingGuid = @TrainingGuid";
            await conn.ExecuteAsync(sql, training, transaction);
        }

        public async Task SoftDeleteTrainingAsync(Training training, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Training]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE TrainingGuid = @TrainingGuid";

            await conn.ExecuteAsync(sql, training, transaction);
        }

        public async Task DeleteTrainingAsync(Guid trainingGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Training] WHERE TrainingGuid = @trainingGuid";
            await conn.ExecuteAsync(sql, new { trainingGuid }, transaction);
        }

        public async Task<IEnumerable<Training>> SearchTrainingAsync(
            string? trainingCode,
            string? trainingCodeSearchType,
            string? trainingName,
            string? trainingNameSearchType,
            string? trainingShortName,
            string? trainingShortNameSearchType,
            string? srTrainingCategory,
            string? srTrainingCategorySearchType,
            string? srTrainingType,
            string? srTrainingTypeSearchType,
            string? srTrainingLevel,
            string? srTrainingLevelSearchType,
            string? durationHour,
            string? durationHourSearchType,
            string? validityMonth,
            string? validityMonthSearchType,
            string? srTrainingProvider,
            string? srTrainingProviderSearchType,
            string? isMandatory,
            string? isMandatorySearchType,
            string? isCertification,
            string? isCertificationSearchType,
            string? isExternal,
            string? isExternalSearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(trainingCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.TrainingCode", "@trainingCode", trainingCodeSearchType, parameters, "trainingCode", trainingCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(trainingName))
            {
                var param = SqlFilterHelper.BuildFilter("a.TrainingName", "@trainingName", trainingNameSearchType, parameters, "trainingName", trainingName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(trainingShortName))
            {
                var param = SqlFilterHelper.BuildFilter("a.TrainingShortName", "@trainingShortName", trainingShortNameSearchType, parameters, "trainingShortName", trainingShortName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srTrainingCategory))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrTrainingCategory", "@srTrainingCategory", srTrainingCategorySearchType, parameters, "srTrainingCategory", srTrainingCategory);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srTrainingType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrTrainingType", "@srTrainingType", srTrainingTypeSearchType, parameters, "srTrainingType", srTrainingType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srTrainingLevel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrTrainingLevel", "@srTrainingLevel", srTrainingLevelSearchType, parameters, "srTrainingLevel", srTrainingLevel);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(durationHour))
            {
                var param = SqlFilterHelper.BuildFilter("a.DurationHour", "@durationHour", durationHourSearchType, parameters, "durationHour", durationHour);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(validityMonth))
            {
                var param = SqlFilterHelper.BuildFilter("a.ValidityMonth", "@validityMonth", validityMonthSearchType, parameters, "validityMonth", validityMonth);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srTrainingProvider))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrTrainingProvider", "@srTrainingProvider", srTrainingProviderSearchType, parameters, "srTrainingProvider", srTrainingProvider);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isMandatory))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsMandatory", "@isMandatory", isMandatorySearchType, parameters, "isMandatory", isMandatory);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCertification))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCertification", "@isCertification", isCertificationSearchType, parameters, "isCertification", isCertification);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isExternal))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsExternal", "@isExternal", isExternalSearchType, parameters, "isExternal", isExternal);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                var param = SqlFilterHelper.BuildFilter("a.Description", "@description", descriptionSearchType, parameters, "description", description);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isActive))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsActive", "@isActive", isActiveSearchType, parameters, "isActive", isActive);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*


                FROM [app].[Training] a


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.TrainingId DESC";

            return await connection.QueryAsync<Training>(sql, parameters, transaction);
        }
    }
}
