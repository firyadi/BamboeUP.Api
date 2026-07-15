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
    public partial class PersonAddressRepository(RepositoryContext context, IAuditService audit) : IPersonAddressRepository
    {

        public async Task<PersonAddress?> GetPersonAddressAsync(Guid personAddressGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName

                FROM [app].[PersonAddress] a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN [core].[Province] j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId

                WHERE a.PersonAddressGuid = @personAddressGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonAddress>(sql, new { personAddressGuid });
        }

        public async Task<PersonAddress?> GetByPersonGuidAndPersonAddressGuidAsync(Guid personGuid, Guid personAddressGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName

                FROM [app].[PersonAddress] a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN [core].[Province] j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonAddressGuid = @personAddressGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonAddress>(sql, new { personGuid, personAddressGuid });
        }

        public async Task<IEnumerable<PersonAddress>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName

                FROM [app].[PersonAddress] a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN [core].[Province] j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonAddressId DESC";
            return await connection.QueryAsync<PersonAddress>(sql, new { personGuid });
        }

        public async Task CreatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personAddress);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonAddress]
                (PersonAddressGuid, PersonId, CreatedById, StatusId, CreatedTime, SrAddressType, Address, CountryId, ProvinceId, CityId)
                VALUES
                (@PersonAddressGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrAddressType, @Address, @CountryId, @ProvinceId, @CityId);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personAddress.PersonAddressId = await conn.QuerySingleAsync<long>(sql, personAddress, transaction);
        }

        public async Task UpdatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personAddress);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonAddress]
                SET                     PersonId = @PersonId,
                    SrAddressType = @SrAddressType,
                    Address = @Address,
                    CountryId = @CountryId,
                    ProvinceId = @ProvinceId,
                    CityId = @CityId,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonAddressGuid = @PersonAddressGuid";
            await conn.ExecuteAsync(sql, personAddress, transaction);
        }

        public async Task SoftDeletePersonAddressAsync(PersonAddress personAddress, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personAddress, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonAddress]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonAddressGuid = @PersonAddressGuid";

            await conn.ExecuteAsync(sql, personAddress, transaction);
        }

        public async Task DeletePersonAddressAsync(Guid personAddressGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonAddress] WHERE PersonAddressGuid = @personAddressGuid";
            await conn.ExecuteAsync(sql, new { personAddressGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonAddress] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonAddress>> SearchPersonAddressAsync(
            string? srAddressType,
            string? srAddressTypeSearchType,
            string? address,
            string? addressSearchType,
            string? countryId,
            string? countryIdSearchType,
            string? provinceId,
            string? provinceIdSearchType,
            string? cityId,
            string? cityIdSearchType,
            Guid personGuid, Guid personAddressGuid,
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
            if (personAddressGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonAddressGuid = @personAddressGuid");
                parameters.Add("@personAddressGuid", personAddressGuid);
            }

            if (!string.IsNullOrWhiteSpace(srAddressType))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrAddressType", "@srAddressType", srAddressTypeSearchType, parameters, "srAddressType", srAddressType);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(countryId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CountryId", "@countryId", countryIdSearchType, parameters, "countryId", countryId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(provinceId))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProvinceId", "@provinceId", provinceIdSearchType, parameters, "provinceId", provinceId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(cityId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CityId", "@cityId", cityIdSearchType, parameters, "cityId", cityId);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName

                FROM [app].[PersonAddress] a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN [core].[Province] j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonAddressId DESC";

            return await connection.QueryAsync<PersonAddress>(sql, parameters, transaction);
        }
    }
}
