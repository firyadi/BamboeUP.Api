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
    public partial class PersonIdentificationRepository(RepositoryContext context, IAuditService audit) : IPersonIdentificationRepository
    {

        public async Task<PersonIdentification?> GetPersonIdentificationAsync(Guid personIdentificationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName

                FROM [app].[PersonIdentification] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonIdentificationGuid = @personIdentificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonIdentification>(sql, new { personIdentificationGuid });
        }

        public async Task<PersonIdentification?> GetByPersonGuidAndPersonIdentificationGuidAsync(Guid personGuid, Guid personIdentificationGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName

                FROM [app].[PersonIdentification] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonIdentificationGuid = @personIdentificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonIdentification>(sql, new { personGuid, personIdentificationGuid });
        }

        public async Task<IEnumerable<PersonIdentification>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName

                FROM [app].[PersonIdentification] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonIdentificationId DESC";
            return await connection.QueryAsync<PersonIdentification>(sql, new { personGuid });
        }

        public async Task CreatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personIdentification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonIdentification]
                (PersonIdentificationGuid, PersonId, CreatedById, StatusId, CreatedTime, SrIdentificationTypeId, IdentificationValue)
                VALUES
                (@PersonIdentificationGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrIdentificationTypeId, @IdentificationValue);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personIdentification.PersonIdentificationId = await conn.QuerySingleAsync<long>(sql, personIdentification, transaction);
        }

        public async Task UpdatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personIdentification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonIdentification]
                SET                     PersonId = @PersonId,
                    SrIdentificationTypeId = @SrIdentificationTypeId,
                    IdentificationValue = @IdentificationValue,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonIdentificationGuid = @PersonIdentificationGuid";
            await conn.ExecuteAsync(sql, personIdentification, transaction);
        }

        public async Task SoftDeletePersonIdentificationAsync(PersonIdentification personIdentification, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personIdentification, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonIdentification]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonIdentificationGuid = @PersonIdentificationGuid";

            await conn.ExecuteAsync(sql, personIdentification, transaction);
        }

        public async Task DeletePersonIdentificationAsync(Guid personIdentificationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonIdentification] WHERE PersonIdentificationGuid = @personIdentificationGuid";
            await conn.ExecuteAsync(sql, new { personIdentificationGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonIdentification] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonIdentification>> SearchPersonIdentificationAsync(
            string? srIdentificationTypeId,
            string? srIdentificationTypeIdSearchType,
            string? identificationValue,
            string? identificationValueSearchType,
            Guid personGuid, Guid personIdentificationGuid,
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
            if (personIdentificationGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonIdentificationGuid = @personIdentificationGuid");
                parameters.Add("@personIdentificationGuid", personIdentificationGuid);
            }

            if (!string.IsNullOrWhiteSpace(srIdentificationTypeId))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrIdentificationTypeId", "@srIdentificationTypeId", srIdentificationTypeIdSearchType, parameters, "srIdentificationTypeId", srIdentificationTypeId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(identificationValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.IdentificationValue", "@identificationValue", identificationValueSearchType, parameters, "identificationValue", identificationValue);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName

                FROM [app].[PersonIdentification] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonIdentificationId DESC";

            return await connection.QueryAsync<PersonIdentification>(sql, parameters, transaction);
        }
    }
}
