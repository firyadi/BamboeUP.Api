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
    public partial class PersonFamilyRepository(RepositoryContext context, IAuditService audit) : IPersonFamilyRepository
    {

        public async Task<PersonFamily?> GetPersonFamilyAsync(Guid personFamilyGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonFamily] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonFamilyGuid = @personFamilyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonFamily>(sql, new { personFamilyGuid });
        }

        public async Task<PersonFamily?> GetByPersonGuidAndPersonFamilyGuidAsync(Guid personGuid, Guid personFamilyGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonFamily] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

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
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonFamily] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonFamilyId DESC";
            return await connection.QueryAsync<PersonFamily>(sql, new { personGuid });
        }

        public async Task CreatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personFamily);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonFamily]
                (PersonFamilyGuid, PersonId, CreatedById, StatusId, CreatedTime, SrFamilyRelation, FamilyName, DateBirth, SrEducationLevel, Address, StateId, CityId, ZipCode, Phone, SrMaritalStatus, SrGender)
                VALUES
                (@PersonFamilyGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrFamilyRelation, @FamilyName, @DateBirth, @SrEducationLevel, @Address, @StateId, @CityId, @ZipCode, @Phone, @SrMaritalStatus, @SrGender);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personFamily.PersonFamilyId = await conn.QuerySingleAsync<long>(sql, personFamily, transaction);
        }

        public async Task UpdatePersonFamilyAsync(PersonFamily personFamily, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personFamily);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonFamily]
                SET                     PersonId = @PersonId,
                    SrFamilyRelation = @SrFamilyRelation,
                    FamilyName = @FamilyName,
                    DateBirth = @DateBirth,
                    SrEducationLevel = @SrEducationLevel,
                    Address = @Address,
                    StateId = @StateId,
                    CityId = @CityId,
                    ZipCode = @ZipCode,
                    Phone = @Phone,
                    SrMaritalStatus = @SrMaritalStatus,
                    SrGender = @SrGender,
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
            const string sql = @"
                UPDATE [app].[PersonFamily]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonFamilyGuid = @PersonFamilyGuid";

            await conn.ExecuteAsync(sql, personFamily, transaction);
        }

        public async Task DeletePersonFamilyAsync(Guid personFamilyGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonFamily] WHERE PersonFamilyGuid = @personFamilyGuid";
            await conn.ExecuteAsync(sql, new { personFamilyGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonFamily] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonFamily>> SearchPersonFamilyAsync(
            string? srFamilyRelation,
            string? srFamilyRelationSearchType,
            string? familyName,
            string? familyNameSearchType,
            string? dateBirth,
            string? dateBirthSearchType,
            string? srEducationLevel,
            string? srEducationLevelSearchType,
            string? address,
            string? addressSearchType,
            string? stateId,
            string? stateIdSearchType,
            string? cityId,
            string? cityIdSearchType,
            string? zipCode,
            string? zipCodeSearchType,
            string? phone,
            string? phoneSearchType,
            string? srMaritalStatus,
            string? srMaritalStatusSearchType,
            string? srGender,
            string? srGenderSearchType,
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

            if (!string.IsNullOrWhiteSpace(srFamilyRelation))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrFamilyRelation", "@srFamilyRelation", srFamilyRelationSearchType, parameters, "srFamilyRelation", srFamilyRelation);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(familyName))
            {
                var param = SqlFilterHelper.BuildFilter("a.FamilyName", "@familyName", familyNameSearchType, parameters, "familyName", familyName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(dateBirth))
            {
                var param = SqlFilterHelper.BuildFilter("a.DateBirth", "@dateBirth", dateBirthSearchType, parameters, "dateBirth", dateBirth);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srEducationLevel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrEducationLevel", "@srEducationLevel", srEducationLevelSearchType, parameters, "srEducationLevel", srEducationLevel);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(stateId))
            {
                var param = SqlFilterHelper.BuildFilter("a.StateId", "@stateId", stateIdSearchType, parameters, "stateId", stateId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(cityId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CityId", "@cityId", cityIdSearchType, parameters, "cityId", cityId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(zipCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.ZipCode", "@zipCode", zipCodeSearchType, parameters, "zipCode", zipCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var param = SqlFilterHelper.BuildFilter("a.Phone", "@phone", phoneSearchType, parameters, "phone", phone);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srMaritalStatus))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrMaritalStatus", "@srMaritalStatus", srMaritalStatusSearchType, parameters, "srMaritalStatus", srMaritalStatus);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srGender))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrGender", "@srGender", srGenderSearchType, parameters, "srGender", srGender);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonFamily] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonFamilyId DESC";

            return await connection.QueryAsync<PersonFamily>(sql, parameters, transaction);
        }
    }
}
