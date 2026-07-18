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
    public partial class PersonContactRepository(RepositoryContext context, IAuditService audit) : IPersonContactRepository
    {

        public async Task<PersonContact?> GetPersonContactAsync(Guid personContactGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonContact] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonContactGuid = @personContactGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonContact>(sql, new { personContactGuid });
        }

        public async Task<PersonContact?> GetByPersonGuidAndPersonContactGuidAsync(Guid personGuid, Guid personContactGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonContact] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonContactGuid = @personContactGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonContact>(sql, new { personGuid, personContactGuid });
        }

        public async Task<IEnumerable<PersonContact>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonContact] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonContactId DESC";
            return await connection.QueryAsync<PersonContact>(sql, new { personGuid });
        }

        public async Task CreatePersonContactAsync(PersonContact personContact, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personContact);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonContact]
                (PersonContactGuid, PersonId, CreatedById, StatusId, CreatedTime, SrContactType, ContactValue, IsPrimary, Remark)
                VALUES
                (@PersonContactGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrContactType, @ContactValue, @IsPrimary, @Remark);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personContact.PersonContactId = await conn.QuerySingleAsync<long>(sql, personContact, transaction);
        }

        public async Task UpdatePersonContactAsync(PersonContact personContact, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personContact);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonContact]
                SET                     PersonId = @PersonId,
                    SrContactType = @SrContactType,
                    ContactValue = @ContactValue,
                    IsPrimary = @IsPrimary,
                    Remark = @Remark,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonContactGuid = @PersonContactGuid";
            await conn.ExecuteAsync(sql, personContact, transaction);
        }

        public async Task SoftDeletePersonContactAsync(PersonContact personContact, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personContact, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonContact]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonContactGuid = @PersonContactGuid";

            await conn.ExecuteAsync(sql, personContact, transaction);
        }

        public async Task DeletePersonContactAsync(Guid personContactGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonContact] WHERE PersonContactGuid = @personContactGuid";
            await conn.ExecuteAsync(sql, new { personContactGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonContact] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonContact>> SearchPersonContactAsync(
            string? srContactType,
            string? srContactTypeSearchType,
            string? contactValue,
            string? contactValueSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? remark,
            string? remarkSearchType,
            Guid personGuid, Guid personContactGuid,
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
            if (personContactGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonContactGuid = @personContactGuid");
                parameters.Add("@personContactGuid", personContactGuid);
            }

            if (!string.IsNullOrWhiteSpace(srContactType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrContactType", "@srContactType", srContactTypeSearchType, parameters, "srContactType", srContactType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(contactValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContactValue", "@contactValue", contactValueSearchType, parameters, "contactValue", contactValue);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remark))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remark", "@remark", remarkSearchType, parameters, "remark", remark);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonContact] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonContactId DESC";

            return await connection.QueryAsync<PersonContact>(sql, parameters, transaction);
        }
    }
}
