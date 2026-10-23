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
    public partial class PersonRepository(RepositoryContext context, ISqlDialect dialect) : IPersonRepository
    {
        private readonly string _table = dialect.Qualify("app", "Person");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<Person?> GetPersonAsync(Guid personGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBloodType.StandardReferenceItemName")} AS BloodTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReligion.StandardReferenceItemName")} AS ReligionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSalutation.StandardReferenceItemName")} AS SalutationName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBloodType", "SrBloodType", "BloodType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReligion", "SrReligion", "Religion")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSalutation", "SrSalutation", "Salutation")}

                WHERE a.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { personGuid });
        }

        public async Task<Person?> GetPersonByIdAsync(long personId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBloodType.StandardReferenceItemName")} AS BloodTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReligion.StandardReferenceItemName")} AS ReligionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSalutation.StandardReferenceItemName")} AS SalutationName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBloodType", "SrBloodType", "BloodType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReligion", "SrReligion", "Religion")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSalutation", "SrSalutation", "Salutation")}

                WHERE a.PersonId = @personId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { personId });
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBloodType.StandardReferenceItemName")} AS BloodTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReligion.StandardReferenceItemName")} AS ReligionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSalutation.StandardReferenceItemName")} AS SalutationName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBloodType", "SrBloodType", "BloodType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReligion", "SrReligion", "Religion")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSalutation", "SrSalutation", "Salutation")}

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.PersonId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Person>(sql);
        }

        public async Task CreatePersonAsync(Person person, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(person);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonGuid, CreatedById, StatusId, CreatedTime, FirstName, MiddleName, LastName, PreTitle, PostTitle, BirthName, PlaceofBirth, BirthDate, NationalIdNo, SrGender, SrReligion, SrSalutation, SrBloodType, SrMaritalStatus, FileStorageId)
                VALUES
                (@PersonGuid, @CreatedById, @StatusId, @CreatedTime, @FirstName, @MiddleName, @LastName, @PreTitle, @PostTitle, @BirthName, @PlaceofBirth, @BirthDate, @NationalIdNo, @SrGender, @SrReligion, @SrSalutation, @SrBloodType, @SrMaritalStatus, @FileStorageId){dialect.ReturnIdentitySuffix("PersonId")}";
            person.PersonId = await conn.QuerySingleAsync<long>(sql, person, transaction);
        }

        public async Task UpdatePersonAsync(Person person, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(person);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     FirstName = @FirstName,
                    MiddleName = @MiddleName,
                    LastName = @LastName,
                    PreTitle = @PreTitle,
                    PostTitle = @PostTitle,
                    BirthName = @BirthName,
                    PlaceofBirth = @PlaceofBirth,
                    BirthDate = @BirthDate,
                    NationalIdNo = @NationalIdNo,
                    SrGender = @SrGender,
                    SrReligion = @SrReligion,
                    SrSalutation = @SrSalutation,
                    SrBloodType = @SrBloodType,
                    SrMaritalStatus = @SrMaritalStatus,
                    FileStorageId = @FileStorageId,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonGuid = @PersonGuid";
            await conn.ExecuteAsync(sql, person, transaction);
        }

        public async Task SoftDeletePersonAsync(Person person, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(person, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonGuid = @PersonGuid";

            await conn.ExecuteAsync(sql, person, transaction);
        }

        public async Task DeletePersonAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonGuid = @personGuid";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<Person>> SearchPersonAsync(
            string? firstName,
            string? firstNameSearchType,
            string? middleName,
            string? middleNameSearchType,
            string? lastName,
            string? lastNameSearchType,
            string? preTitle,
            string? preTitleSearchType,
            string? postTitle,
            string? postTitleSearchType,
            string? personName,
            string? personNameSearchType,
            string? birthName,
            string? birthNameSearchType,
            string? placeofBirth,
            string? placeofBirthSearchType,
            string? birthDate,
            string? birthDateSearchType,
            string? nationalIdNo,
            string? nationalIdNoSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                var param = SqlFilterHelper.BuildFilter("a.FirstName", "@firstName", firstNameSearchType, parameters, "firstName", firstName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                var param = SqlFilterHelper.BuildFilter("a.MiddleName", "@middleName", middleNameSearchType, parameters, "middleName", middleName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastName", "@lastName", lastNameSearchType, parameters, "lastName", lastName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(preTitle))
            {
                var param = SqlFilterHelper.BuildFilter("a.PreTitle", "@preTitle", preTitleSearchType, parameters, "preTitle", preTitle, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(postTitle))
            {
                var param = SqlFilterHelper.BuildFilter("a.PostTitle", "@postTitle", postTitleSearchType, parameters, "postTitle", postTitle, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(personName))
            {
                var param = SqlFilterHelper.BuildFilter("a.PersonName", "@personName", personNameSearchType, parameters, "personName", personName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(birthName))
            {
                var param = SqlFilterHelper.BuildFilter("a.BirthName", "@birthName", birthNameSearchType, parameters, "birthName", birthName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(placeofBirth))
            {
                var param = SqlFilterHelper.BuildFilter("a.PlaceofBirth", "@placeofBirth", placeofBirthSearchType, parameters, "placeofBirth", placeofBirth, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(birthDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.BirthDate", "@birthDate", birthDateSearchType, parameters, "birthDate", birthDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(nationalIdNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.NationalIdNo", "@nationalIdNo", nationalIdNoSearchType, parameters, "nationalIdNo", nationalIdNo, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBloodType.StandardReferenceItemName")} AS BloodTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrGender.StandardReferenceItemName")} AS GenderName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrMaritalStatus.StandardReferenceItemName")} AS MaritalStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReligion.StandardReferenceItemName")} AS ReligionName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSalutation.StandardReferenceItemName")} AS SalutationName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBloodType", "SrBloodType", "BloodType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrGender", "SrGender", "Gender")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrMaritalStatus", "SrMaritalStatus", "MaritalStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReligion", "SrReligion", "Religion")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSalutation", "SrSalutation", "Salutation")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Person>(sql, parameters, transaction);
        }
    }
}
