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
    public partial class PersonEmergencyContactRepository(RepositoryContext context, ISqlDialect dialect) : IPersonEmergencyContactRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonEmergencyContact");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonEmergencyContact?> GetPersonEmergencyContactAsync(Guid personEmergencyContactGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRelationship.StandardReferenceItemName")} AS RelationshipName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRelationship", "SrRelationship", "Relationship")}

                WHERE a.PersonEmergencyContactGuid = @personEmergencyContactGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEmergencyContact>(sql, new { personEmergencyContactGuid });
        }

        public async Task<PersonEmergencyContact?> GetByPersonGuidAndPersonEmergencyContactGuidAsync(Guid personGuid, Guid personEmergencyContactGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRelationship.StandardReferenceItemName")} AS RelationshipName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRelationship", "SrRelationship", "Relationship")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonEmergencyContactGuid = @personEmergencyContactGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEmergencyContact>(sql, new { personGuid, personEmergencyContactGuid });
        }

        public async Task<IEnumerable<PersonEmergencyContact>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRelationship.StandardReferenceItemName")} AS RelationshipName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRelationship", "SrRelationship", "Relationship")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonEmergencyContactId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonEmergencyContact>(sql, new { personGuid });
        }

        public async Task CreatePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personEmergencyContact);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonEmergencyContactGuid, PersonId, CreatedById, StatusId, CreatedTime, [ContactName], [SrRelationship], [Phone], [IsPrimary])
                VALUES
                (@PersonEmergencyContactGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @ContactName, @SrRelationship, @Phone, @IsPrimary){dialect.ReturnIdentitySuffix("PersonEmergencyContactId")}";
            personEmergencyContact.PersonEmergencyContactId = await conn.QuerySingleAsync<long>(sql, personEmergencyContact, transaction);
        }

        public async Task UpdatePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personEmergencyContact);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [ContactName] = @ContactName,
                    [SrRelationship] = @SrRelationship,
                    [Phone] = @Phone,
                    [IsPrimary] = @IsPrimary,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonEmergencyContactGuid = @PersonEmergencyContactGuid";
            await conn.ExecuteAsync(sql, personEmergencyContact, transaction);
        }

        public async Task SoftDeletePersonEmergencyContactAsync(PersonEmergencyContact personEmergencyContact, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personEmergencyContact, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonEmergencyContactGuid = @PersonEmergencyContactGuid";

            await conn.ExecuteAsync(sql, personEmergencyContact, transaction);
        }

        public async Task DeletePersonEmergencyContactAsync(Guid personEmergencyContactGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonEmergencyContactGuid = @personEmergencyContactGuid";
            await conn.ExecuteAsync(sql, new { personEmergencyContactGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonEmergencyContact>> SearchPersonEmergencyContactAsync(
            string? contactName,
            string? contactNameSearchType,
            string? phone,
            string? phoneSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            Guid personGuid, Guid personEmergencyContactGuid,
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
            if (personEmergencyContactGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonEmergencyContactGuid = @personEmergencyContactGuid");
                parameters.Add("@personEmergencyContactGuid", personEmergencyContactGuid);
            }

            if (!string.IsNullOrWhiteSpace(contactName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ContactName", "@contactName", contactNameSearchType, parameters, "contactName", contactName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var param = SqlFilterHelper.BuildFilter("a.Phone", "@phone", phoneSearchType, parameters, "phone", phone, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRelationship.StandardReferenceItemName")} AS RelationshipName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRelationship", "SrRelationship", "Relationship")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonEmergencyContactId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonEmergencyContact>(sql, parameters, transaction);
        }
    }
}
