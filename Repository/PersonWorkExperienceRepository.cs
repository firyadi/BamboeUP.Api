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
    public partial class PersonWorkExperienceRepository(RepositoryContext context, IAuditService audit) : IPersonWorkExperienceRepository
    {

        public async Task<PersonWorkExperience?> GetPersonWorkExperienceAsync(Guid personWorkExperienceGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonWorkExperience] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonWorkExperienceGuid = @personWorkExperienceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonWorkExperience>(sql, new { personWorkExperienceGuid });
        }

        public async Task<PersonWorkExperience?> GetByPersonGuidAndPersonWorkExperienceGuidAsync(Guid personGuid, Guid personWorkExperienceGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonWorkExperience] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonWorkExperienceGuid = @personWorkExperienceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonWorkExperience>(sql, new { personGuid, personWorkExperienceGuid });
        }

        public async Task<IEnumerable<PersonWorkExperience>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonWorkExperience] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonWorkExperienceId DESC";
            return await connection.QueryAsync<PersonWorkExperience>(sql, new { personGuid });
        }

        public async Task CreatePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personWorkExperience);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonWorkExperience]
                (PersonWorkExperienceGuid, PersonId, CreatedById, StatusId, CreatedTime, SrIndustry, SrEmploymentType, CompanyName, JobTitle, Department, Location, Supervisor, JobDescription, StartDate, EndDate, IsCurrentEmployment, LastSalary, ReasonforLeaving, Remarks)
                VALUES
                (@PersonWorkExperienceGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrIndustry, @SrEmploymentType, @CompanyName, @JobTitle, @Department, @Location, @Supervisor, @JobDescription, @StartDate, @EndDate, @IsCurrentEmployment, @LastSalary, @ReasonforLeaving, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personWorkExperience.PersonWorkExperienceId = await conn.QuerySingleAsync<long>(sql, personWorkExperience, transaction);
        }

        public async Task UpdatePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personWorkExperience);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonWorkExperience]
                SET                     PersonId = @PersonId,
                    SrIndustry = @SrIndustry,
                    SrEmploymentType = @SrEmploymentType,
                    CompanyName = @CompanyName,
                    JobTitle = @JobTitle,
                    Department = @Department,
                    Location = @Location,
                    Supervisor = @Supervisor,
                    JobDescription = @JobDescription,
                    StartDate = @StartDate,
                    EndDate = @EndDate,
                    IsCurrentEmployment = @IsCurrentEmployment,
                    LastSalary = @LastSalary,
                    ReasonforLeaving = @ReasonforLeaving,
                    Remarks = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonWorkExperienceGuid = @PersonWorkExperienceGuid";
            await conn.ExecuteAsync(sql, personWorkExperience, transaction);
        }

        public async Task SoftDeletePersonWorkExperienceAsync(PersonWorkExperience personWorkExperience, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personWorkExperience, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonWorkExperience]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonWorkExperienceGuid = @PersonWorkExperienceGuid";

            await conn.ExecuteAsync(sql, personWorkExperience, transaction);
        }

        public async Task DeletePersonWorkExperienceAsync(Guid personWorkExperienceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonWorkExperience] WHERE PersonWorkExperienceGuid = @personWorkExperienceGuid";
            await conn.ExecuteAsync(sql, new { personWorkExperienceGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonWorkExperience] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonWorkExperience>> SearchPersonWorkExperienceAsync(
            string? srIndustry,
            string? srIndustrySearchType,
            string? srEmploymentType,
            string? srEmploymentTypeSearchType,
            string? companyName,
            string? companyNameSearchType,
            string? jobTitle,
            string? jobTitleSearchType,
            string? department,
            string? departmentSearchType,
            string? location,
            string? locationSearchType,
            string? supervisor,
            string? supervisorSearchType,
            string? jobDescription,
            string? jobDescriptionSearchType,
            string? startDate,
            string? startDateSearchType,
            string? endDate,
            string? endDateSearchType,
            string? isCurrentEmployment,
            string? isCurrentEmploymentSearchType,
            string? lastSalary,
            string? lastSalarySearchType,
            string? reasonforLeaving,
            string? reasonforLeavingSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid personGuid, Guid personWorkExperienceGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_PersonId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (personWorkExperienceGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonWorkExperienceGuid = @personWorkExperienceGuid");
                parameters.Add("@personWorkExperienceGuid", personWorkExperienceGuid);
            }

            if (!string.IsNullOrWhiteSpace(srIndustry))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrIndustry", "@srIndustry", srIndustrySearchType, parameters, "srIndustry", srIndustry);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srEmploymentType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrEmploymentType", "@srEmploymentType", srEmploymentTypeSearchType, parameters, "srEmploymentType", srEmploymentType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(companyName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyName", "@companyName", companyNameSearchType, parameters, "companyName", companyName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(jobTitle))
            {
                var param = SqlFilterHelper.BuildFilter("a.JobTitle", "@jobTitle", jobTitleSearchType, parameters, "jobTitle", jobTitle);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                var param = SqlFilterHelper.BuildFilter("a.Department", "@department", departmentSearchType, parameters, "department", department);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                var param = SqlFilterHelper.BuildFilter("a.Location", "@location", locationSearchType, parameters, "location", location);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(supervisor))
            {
                var param = SqlFilterHelper.BuildFilter("a.Supervisor", "@supervisor", supervisorSearchType, parameters, "supervisor", supervisor);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(jobDescription))
            {
                var param = SqlFilterHelper.BuildFilter("a.JobDescription", "@jobDescription", jobDescriptionSearchType, parameters, "jobDescription", jobDescription);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
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

            if (!string.IsNullOrWhiteSpace(isCurrentEmployment))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCurrentEmployment", "@isCurrentEmployment", isCurrentEmploymentSearchType, parameters, "isCurrentEmployment", isCurrentEmployment);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastSalary))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastSalary", "@lastSalary", lastSalarySearchType, parameters, "lastSalary", lastSalary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(reasonforLeaving))
            {
                var param = SqlFilterHelper.BuildFilter("a.ReasonforLeaving", "@reasonforLeaving", reasonforLeavingSearchType, parameters, "reasonforLeaving", reasonforLeaving);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonWorkExperience] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonWorkExperienceId DESC";

            return await connection.QueryAsync<PersonWorkExperience>(sql, parameters, transaction);
        }
    }
}
