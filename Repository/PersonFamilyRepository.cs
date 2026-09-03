using BamboeUp.Audit.Contracts;
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
    public partial class PersonFamilyRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IPersonFamilyRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonFamily");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonFamily?> GetPersonFamilyAsync(Guid personFamilyGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_CityId.CityName AS CityName
, j_StateId.StateName AS StateName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFamilyRelation.StandardReferenceItemName")} AS FamilyRelationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("app", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("app", "State")} j_StateId ON a.StateId = j_StateId.StateId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrFamilyRelation", "SrFamilyRelation", "FamilyRelation")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}

                WHERE a.PersonFamilyGuid = @personFamilyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonFamily>(sql, new { personFamilyGuid });
        }

        public async Task<PersonFamily?> GetByPersonGuidAndPersonFamilyGuidAsync(Guid personGuid, Guid personFamilyGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_CityId.CityName AS CityName
, j_StateId.StateName AS StateName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFamilyRelation.StandardReferenceItemName")} AS FamilyRelationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("app", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("app", "State")} j_StateId ON a.StateId = j_StateId.StateId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrFamilyRelation", "SrFamilyRelation", "FamilyRelation")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonFamilyGuid = @personFamilyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonFamily>(sql, new { personGuid, personFamilyGuid });
        }

        public async Task<IEnumerable<PersonFamily>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_CityId.CityName AS CityName
, j_StateId.StateName AS StateName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFamilyRelation.StandardReferenceItemName")} AS FamilyRelationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("app", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("app", "State")} j_StateId ON a.StateId = j_StateId.StateId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrFamilyRelation", "SrFamilyRelation", "FamilyRelation")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonFamilyId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonFamily>(sql, new { personGuid });
        }

        public async Task CreatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personFamily);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonFamilyGuid, PersonId, CreatedById, StatusId, CreatedTime, [SrFamilyRelation], [FamilyName], [DateBirth], [SrEducationLevel], [Address], [StateId], [CityId], [ZipCode], [Phone], [SrMaritalStatus], [SrGender])
                VALUES
                (@PersonFamilyGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrFamilyRelation, @FamilyName, @DateBirth, @SrEducationLevel, @Address, @StateId, @CityId, @ZipCode, @Phone, @SrMaritalStatus, @SrGender){dialect.ReturnIdentitySuffix("PersonFamilyId")}";
            personFamily.PersonFamilyId = await conn.QuerySingleAsync<long>(sql, personFamily, transaction);
        }

        public async Task UpdatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personFamily);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [SrFamilyRelation] = @SrFamilyRelation,
                    [FamilyName] = @FamilyName,
                    [DateBirth] = @DateBirth,
                    [SrEducationLevel] = @SrEducationLevel,
                    [Address] = @Address,
                    [StateId] = @StateId,
                    [CityId] = @CityId,
                    [ZipCode] = @ZipCode,
                    [Phone] = @Phone,
                    [SrMaritalStatus] = @SrMaritalStatus,
                    [SrGender] = @SrGender,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonFamilyGuid = @PersonFamilyGuid";
            await conn.ExecuteAsync(sql, personFamily, transaction);
        }

        public async Task SoftDeletePersonFamilyAsync(PersonFamily personFamily, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personFamily, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonFamilyGuid = @PersonFamilyGuid";

            await conn.ExecuteAsync(sql, personFamily, transaction);
        }

        public async Task DeletePersonFamilyAsync(Guid personFamilyGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonFamilyGuid = @personFamilyGuid";
            await conn.ExecuteAsync(sql, new { personFamilyGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonFamily>> SearchPersonFamilyAsync(
            string? familyName,
            string? familyNameSearchType,
            string? dateBirth,
            string? dateBirthSearchType,
            string? address,
            string? addressSearchType,
            string? zipCode,
            string? zipCodeSearchType,
            string? phone,
            string? phoneSearchType,
            Guid personGuid, Guid personFamilyGuid,
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
            if (personFamilyGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonFamilyGuid = @personFamilyGuid");
                parameters.Add("@personFamilyGuid", personFamilyGuid);
            }

            if (!string.IsNullOrWhiteSpace(familyName))
            {
                var param = SqlFilterHelper.BuildFilter("a.FamilyName", "@familyName", familyNameSearchType, parameters, "familyName", familyName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(dateBirth))
            {
                var param = SqlFilterHelper.BuildFilter("a.DateBirth", "@dateBirth", dateBirthSearchType, parameters, "dateBirth", dateBirth, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.ZipCode", "@zipCode", zipCodeSearchType, parameters, "zipCode", zipCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var param = SqlFilterHelper.BuildFilter("a.Phone", "@phone", phoneSearchType, parameters, "phone", phone, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_CityId.CityName AS CityName
, j_StateId.StateName AS StateName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrFamilyRelation.StandardReferenceItemName")} AS FamilyRelationName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("app", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("app", "State")} j_StateId ON a.StateId = j_StateId.StateId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrFamilyRelation", "SrFamilyRelation", "FamilyRelation")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonFamilyId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonFamily>(sql, parameters, transaction);
        }
    }
}
