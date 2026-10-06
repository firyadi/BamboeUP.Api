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
    public partial class PersonAddressRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IPersonAddressRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonAddress");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonAddress?> GetPersonAddressAsync(Guid personAddressGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("core", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("core", "Country")} j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Province")} j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAddressType", "SrAddressType", "AddressType")}

                WHERE a.PersonAddressGuid = @personAddressGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonAddress>(sql, new { personAddressGuid });
        }

        public async Task<PersonAddress?> GetByPersonGuidAndPersonAddressGuidAsync(Guid personGuid, Guid personAddressGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("core", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("core", "Country")} j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Province")} j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAddressType", "SrAddressType", "AddressType")}

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
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("core", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("core", "Country")} j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Province")} j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAddressType", "SrAddressType", "AddressType")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonAddressId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonAddress>(sql, new { personGuid });
        }

        public async Task CreatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personAddress);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonAddressGuid, PersonId, CreatedById, StatusId, CreatedTime, [SrAddressType], [Address], [CountryId], [ProvinceId], [CityId])
                VALUES
                (@PersonAddressGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrAddressType, @Address, @CountryId, @ProvinceId, @CityId){dialect.ReturnIdentitySuffix("PersonAddressId")}";
            personAddress.PersonAddressId = await conn.QuerySingleAsync<long>(sql, personAddress, transaction);
        }

        public async Task UpdatePersonAddressAsync(PersonAddress personAddress, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personAddress);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [SrAddressType] = @SrAddressType,
                    [Address] = @Address,
                    [CountryId] = @CountryId,
                    [ProvinceId] = @ProvinceId,
                    [CityId] = @CityId,
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonAddressGuid = @PersonAddressGuid";

            await conn.ExecuteAsync(sql, personAddress, transaction);
        }

        public async Task DeletePersonAddressAsync(Guid personAddressGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonAddressGuid = @personAddressGuid";
            await conn.ExecuteAsync(sql, new { personAddressGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonAddress>> SearchPersonAddressAsync(
            string? address,
            string? addressSearchType,
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

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CountryId.CountryName AS CountryName
, j_PersonId.PersonName AS PersonName
, j_ProvinceId.ProvinceName AS ProvinceName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("core", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("core", "Country")} j_CountryId ON a.CountryId = j_CountryId.CountryId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Province")} j_ProvinceId ON a.ProvinceId = j_ProvinceId.ProvinceId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAddressType", "SrAddressType", "AddressType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonAddressId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonAddress>(sql, parameters, transaction);
        }
    }
}
