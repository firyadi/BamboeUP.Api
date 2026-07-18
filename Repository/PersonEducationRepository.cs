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
    public partial class PersonEducationRepository(RepositoryContext context, IAuditService audit) : IPersonEducationRepository
    {

        public async Task<PersonEducation?> GetPersonEducationAsync(Guid personEducationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonEducation] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonEducationGuid = @personEducationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEducation>(sql, new { personEducationGuid });
        }

        public async Task<PersonEducation?> GetByPersonGuidAndPersonEducationGuidAsync(Guid personGuid, Guid personEducationGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonEducation] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonEducationGuid = @personEducationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEducation>(sql, new { personGuid, personEducationGuid });
        }

        public async Task<IEnumerable<PersonEducation>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonEducation] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonEducationId DESC";
            return await connection.QueryAsync<PersonEducation>(sql, new { personGuid });
        }

        public async Task CreatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personEducation);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonEducation]
                (PersonEducationGuid, PersonId, CreatedById, StatusId, CreatedTime, SrEducationLevel, InstitutionName)
                VALUES
                (@PersonEducationGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrEducationLevel, @InstitutionName);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personEducation.PersonEducationId = await conn.QuerySingleAsync<long>(sql, personEducation, transaction);
        }

        public async Task UpdatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personEducation);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonEducation]
                SET                     PersonId = @PersonId,
                    SrEducationLevel = @SrEducationLevel,
                    InstitutionName = @InstitutionName,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonEducationGuid = @PersonEducationGuid";
            await conn.ExecuteAsync(sql, personEducation, transaction);
        }

        public async Task SoftDeletePersonEducationAsync(PersonEducation personEducation, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personEducation, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonEducation]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonEducationGuid = @PersonEducationGuid";

            await conn.ExecuteAsync(sql, personEducation, transaction);
        }

        public async Task DeletePersonEducationAsync(Guid personEducationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonEducation] WHERE PersonEducationGuid = @personEducationGuid";
            await conn.ExecuteAsync(sql, new { personEducationGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonEducation] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonEducation>> SearchPersonEducationAsync(
            string? srEducationLevel,
            string? srEducationLevelSearchType,
            string? institutionName,
            string? institutionNameSearchType,
            Guid personGuid, Guid personEducationGuid,
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
            if (personEducationGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonEducationGuid = @personEducationGuid");
                parameters.Add("@personEducationGuid", personEducationGuid);
            }

            if (!string.IsNullOrWhiteSpace(srEducationLevel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrEducationLevel", "@srEducationLevel", srEducationLevelSearchType, parameters, "srEducationLevel", srEducationLevel);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(institutionName))
            {
                var param = SqlFilterHelper.BuildFilter("a.InstitutionName", "@institutionName", institutionNameSearchType, parameters, "institutionName", institutionName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonEducation] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonEducationId DESC";

            return await connection.QueryAsync<PersonEducation>(sql, parameters, transaction);
        }
    }
}
