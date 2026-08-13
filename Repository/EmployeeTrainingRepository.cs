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
    public partial class EmployeeTrainingRepository(RepositoryContext context, IAuditService audit) : IEmployeeTrainingRepository
    {

        public async Task<EmployeeTraining?> GetEmployeeTrainingAsync(Guid employeeTrainingGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_TrainingId.TrainingName AS TrainingName


                FROM [app].[EmployeeTraining] a
LEFT JOIN [app].[Training] j_TrainingId ON a.TrainingId = j_TrainingId.TrainingId

                WHERE a.EmployeeTrainingGuid = @employeeTrainingGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeTraining>(sql, new { employeeTrainingGuid });
        }

        public async Task<EmployeeTraining?> GetByEmployeeGuidAndEmployeeTrainingGuidAsync(Guid employeeGuid, Guid employeeTrainingGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_TrainingId.TrainingName AS TrainingName


                FROM [app].[EmployeeTraining] a
LEFT JOIN [app].[Training] j_TrainingId ON a.TrainingId = j_TrainingId.TrainingId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeTrainingGuid = @employeeTrainingGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeTraining>(sql, new { employeeGuid, employeeTrainingGuid });
        }

        public async Task<IEnumerable<EmployeeTraining>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_TrainingId.TrainingName AS TrainingName


                FROM [app].[EmployeeTraining] a
LEFT JOIN [app].[Training] j_TrainingId ON a.TrainingId = j_TrainingId.TrainingId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeTrainingId DESC";
            return await connection.QueryAsync<EmployeeTraining>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeTrainingAsync(EmployeeTraining employeeTraining, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeTraining);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeTraining]
                (EmployeeTrainingGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [TrainingId], [StartDate], [EndDate], [DurationHour], [Score], [Grade], [IsPassed], [CertificateNumber], [FileStorageId], [Remarks])
                VALUES
                (@EmployeeTrainingGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @TrainingId, @StartDate, @EndDate, @DurationHour, @Score, @Grade, @IsPassed, @CertificateNumber, @FileStorageId, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeTraining.EmployeeTrainingId = await conn.QuerySingleAsync<long>(sql, employeeTraining, transaction);
        }

        public async Task UpdateEmployeeTrainingAsync(EmployeeTraining employeeTraining, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeTraining);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeTraining]
                SET                     [EmployeeId] = @EmployeeId,
                    [TrainingId] = @TrainingId,
                    [StartDate] = @StartDate,
                    [EndDate] = @EndDate,
                    [DurationHour] = @DurationHour,
                    [Score] = @Score,
                    [Grade] = @Grade,
                    [IsPassed] = @IsPassed,
                    [CertificateNumber] = @CertificateNumber,
                    [FileStorageId] = @FileStorageId,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeTrainingGuid = @EmployeeTrainingGuid";
            await conn.ExecuteAsync(sql, employeeTraining, transaction);
        }

        public async Task SoftDeleteEmployeeTrainingAsync(EmployeeTraining employeeTraining, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeTraining, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeTraining]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeTrainingGuid = @EmployeeTrainingGuid";

            await conn.ExecuteAsync(sql, employeeTraining, transaction);
        }

        public async Task DeleteEmployeeTrainingAsync(Guid employeeTrainingGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeTraining] WHERE EmployeeTrainingGuid = @employeeTrainingGuid";
            await conn.ExecuteAsync(sql, new { employeeTrainingGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeTraining] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeTraining>> SearchEmployeeTrainingAsync(
            string? startDate,
            string? startDateSearchType,
            string? endDate,
            string? endDateSearchType,
            string? durationHour,
            string? durationHourSearchType,
            string? score,
            string? scoreSearchType,
            string? grade,
            string? gradeSearchType,
            string? isPassed,
            string? isPassedSearchType,
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeTrainingGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (employeeGuid != Guid.Empty)
            {
                whereClauses.Add("j_EmployeeId.EmployeeGuid = @employeeGuid");
                parameters.Add("@employeeGuid", employeeGuid);
            }
            if (employeeTrainingGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeTrainingGuid = @employeeTrainingGuid");
                parameters.Add("@employeeTrainingGuid", employeeTrainingGuid);
            }

            if (!string.IsNullOrWhiteSpace(startDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.StartDate", "@startDate", startDateSearchType, parameters, "startDate", startDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(endDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.EndDate", "@endDate", endDateSearchType, parameters, "endDate", endDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(durationHour))
            {
                var param = SqlFilterHelper.BuildFilter("a.DurationHour", "@durationHour", durationHourSearchType, parameters, "durationHour", durationHour);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(score))
            {
                var param = SqlFilterHelper.BuildFilter("a.Score", "@score", scoreSearchType, parameters, "score", score);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(grade))
            {
                var param = SqlFilterHelper.BuildFilter("a.Grade", "@grade", gradeSearchType, parameters, "grade", grade);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPassed))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPassed", "@isPassed", isPassedSearchType, parameters, "isPassed", isPassed);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificateNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificateNumber", "@certificateNumber", certificateNumberSearchType, parameters, "certificateNumber", certificateNumber);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_TrainingId.TrainingName AS TrainingName


                FROM [app].[EmployeeTraining] a
LEFT JOIN [app].[Training] j_TrainingId ON a.TrainingId = j_TrainingId.TrainingId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeTrainingId DESC";

            return await connection.QueryAsync<EmployeeTraining>(sql, parameters, transaction);
        }
    }
}
