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
    public partial class PersonRepository(RepositoryContext context, IAuditService audit) : IPersonRepository
    {

        public async Task<Person?> GetPersonAsync(Guid personGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrBloodType.StandardReferenceItemName, '') AS BloodTypeName
, ISNULL(v_SrGender.StandardReferenceItemName, '') AS GenderName
, ISNULL(v_SrMaritalStatus.StandardReferenceItemName, '') AS MaritalStatusName
, ISNULL(v_SrReligion.StandardReferenceItemName, '') AS ReligionName
, ISNULL(v_SrSalutation.StandardReferenceItemName, '') AS SalutationName


                FROM [app].[Person] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'BloodType')
    WHERE StandardReferenceItemId = a.SrBloodType
) v_SrBloodType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Gender')
    WHERE StandardReferenceItemId = a.SrGender
) v_SrGender
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'MaritalStatus')
    WHERE StandardReferenceItemId = a.SrMaritalStatus
) v_SrMaritalStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Religion')
    WHERE StandardReferenceItemId = a.SrReligion
) v_SrReligion
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Salutation')
    WHERE StandardReferenceItemId = a.SrSalutation
) v_SrSalutation

                WHERE a.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { personGuid });
        }

        public async Task<Person?> GetPersonByIdAsync(long personId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrBloodType.StandardReferenceItemName, '') AS BloodTypeName
, ISNULL(v_SrGender.StandardReferenceItemName, '') AS GenderName
, ISNULL(v_SrMaritalStatus.StandardReferenceItemName, '') AS MaritalStatusName
, ISNULL(v_SrReligion.StandardReferenceItemName, '') AS ReligionName
, ISNULL(v_SrSalutation.StandardReferenceItemName, '') AS SalutationName


                FROM [app].[Person] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'BloodType')
    WHERE StandardReferenceItemId = a.SrBloodType
) v_SrBloodType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Gender')
    WHERE StandardReferenceItemId = a.SrGender
) v_SrGender
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'MaritalStatus')
    WHERE StandardReferenceItemId = a.SrMaritalStatus
) v_SrMaritalStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Religion')
    WHERE StandardReferenceItemId = a.SrReligion
) v_SrReligion
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Salutation')
    WHERE StandardReferenceItemId = a.SrSalutation
) v_SrSalutation

                WHERE a.PersonId = @personId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { personId });
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrBloodType.StandardReferenceItemName, '') AS BloodTypeName
, ISNULL(v_SrGender.StandardReferenceItemName, '') AS GenderName
, ISNULL(v_SrMaritalStatus.StandardReferenceItemName, '') AS MaritalStatusName
, ISNULL(v_SrReligion.StandardReferenceItemName, '') AS ReligionName
, ISNULL(v_SrSalutation.StandardReferenceItemName, '') AS SalutationName


                FROM [app].[Person] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'BloodType')
    WHERE StandardReferenceItemId = a.SrBloodType
) v_SrBloodType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Gender')
    WHERE StandardReferenceItemId = a.SrGender
) v_SrGender
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'MaritalStatus')
    WHERE StandardReferenceItemId = a.SrMaritalStatus
) v_SrMaritalStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Religion')
    WHERE StandardReferenceItemId = a.SrReligion
) v_SrReligion
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Salutation')
    WHERE StandardReferenceItemId = a.SrSalutation
) v_SrSalutation

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.PersonId DESC";
            return await connection.QueryAsync<Person>(sql);
        }

        public async Task CreatePersonAsync(Person person, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(person);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Person]
                (PersonGuid, CreatedById, StatusId, CreatedTime, FirstName, MiddleName, LastName, PreTitle, PostTitle, BirthName, PlaceofBirth, BirthDate, NationalIdNo, SrGender, SrReligion, SrSalutation, SrBloodType, SrMaritalStatus, FileStorageId)
                VALUES
                (@PersonGuid, @CreatedById, @StatusId, @CreatedTime, @FirstName, @MiddleName, @LastName, @PreTitle, @PostTitle, @BirthName, @PlaceofBirth, @BirthDate, @NationalIdNo, @SrGender, @SrReligion, @SrSalutation, @SrBloodType, @SrMaritalStatus, @FileStorageId);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            person.PersonId = await conn.QuerySingleAsync<long>(sql, person, transaction);
        }

        public async Task UpdatePersonAsync(Person person, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(person);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Person]
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
            const string sql = @"
                UPDATE [app].[Person]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonGuid = @PersonGuid";

            await conn.ExecuteAsync(sql, person, transaction);
        }

        public async Task DeletePersonAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Person] WHERE PersonGuid = @personGuid";
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
                var param = SqlFilterHelper.BuildFilter("a.FirstName", "@firstName", firstNameSearchType, parameters, "firstName", firstName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(middleName))
            {
                var param = SqlFilterHelper.BuildFilter("a.MiddleName", "@middleName", middleNameSearchType, parameters, "middleName", middleName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastName", "@lastName", lastNameSearchType, parameters, "lastName", lastName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(preTitle))
            {
                var param = SqlFilterHelper.BuildFilter("a.PreTitle", "@preTitle", preTitleSearchType, parameters, "preTitle", preTitle);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(postTitle))
            {
                var param = SqlFilterHelper.BuildFilter("a.PostTitle", "@postTitle", postTitleSearchType, parameters, "postTitle", postTitle);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(personName))
            {
                var param = SqlFilterHelper.BuildFilter("a.PersonName", "@personName", personNameSearchType, parameters, "personName", personName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(birthName))
            {
                var param = SqlFilterHelper.BuildFilter("a.BirthName", "@birthName", birthNameSearchType, parameters, "birthName", birthName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(placeofBirth))
            {
                var param = SqlFilterHelper.BuildFilter("a.PlaceofBirth", "@placeofBirth", placeofBirthSearchType, parameters, "placeofBirth", placeofBirth);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(birthDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.BirthDate", "@birthDate", birthDateSearchType, parameters, "birthDate", birthDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(nationalIdNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.NationalIdNo", "@nationalIdNo", nationalIdNoSearchType, parameters, "nationalIdNo", nationalIdNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrBloodType.StandardReferenceItemName, '') AS BloodTypeName
, ISNULL(v_SrGender.StandardReferenceItemName, '') AS GenderName
, ISNULL(v_SrMaritalStatus.StandardReferenceItemName, '') AS MaritalStatusName
, ISNULL(v_SrReligion.StandardReferenceItemName, '') AS ReligionName
, ISNULL(v_SrSalutation.StandardReferenceItemName, '') AS SalutationName


                FROM [app].[Person] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'BloodType')
    WHERE StandardReferenceItemId = a.SrBloodType
) v_SrBloodType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Gender')
    WHERE StandardReferenceItemId = a.SrGender
) v_SrGender
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'MaritalStatus')
    WHERE StandardReferenceItemId = a.SrMaritalStatus
) v_SrMaritalStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Religion')
    WHERE StandardReferenceItemId = a.SrReligion
) v_SrReligion
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Salutation')
    WHERE StandardReferenceItemId = a.SrSalutation
) v_SrSalutation

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonId DESC";

            return await connection.QueryAsync<Person>(sql, parameters, transaction);
        }
    }
}
