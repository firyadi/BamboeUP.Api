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
    public partial class CompanyOfficeRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : ICompanyOfficeRepository
    {
        private readonly string _table = dialect.Qualify("app", "CompanyOffice");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<CompanyOffice?> GetCompanyOfficeAsync(Guid companyOfficeGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CompanyId.CompanyName AS CompanyName
, j_CountryId.CountryName AS CountryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
    WHERE StandardReferenceItemId = a.SrAddressType
) v_SrAddressType

                WHERE a.CompanyOfficeGuid = @companyOfficeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new { companyOfficeGuid });
        }

        public async Task<CompanyOffice?> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CompanyId.CompanyName AS CompanyName
, j_CountryId.CountryName AS CountryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
    WHERE StandardReferenceItemId = a.SrAddressType
) v_SrAddressType

                WHERE j_CompanyId.CompanyGuid = @companyGuid
                  AND a.CompanyOfficeGuid = @companyOfficeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new { companyGuid, companyOfficeGuid });
        }

        public async Task<IEnumerable<CompanyOffice>> GetAllByCompanyGuidAsync(Guid companyGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CompanyId.CompanyName AS CompanyName
, j_CountryId.CountryName AS CountryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
    WHERE StandardReferenceItemId = a.SrAddressType
) v_SrAddressType

                WHERE j_CompanyId.CompanyGuid = @companyGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CompanyOfficeId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<CompanyOffice>(sql, new { companyGuid });
        }

        public async Task CreateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(companyOffice);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (CompanyOfficeGuid, CompanyId, CompanyGuid, CreatedById, StatusId, CreatedTime, [CompanyOfficeName], [SrAddressType], [CountryId], [StateId], [CityId], [PostalCodeId], [Address], [Latitude], [Longitude], [RadiusMeters])
                VALUES
                (@CompanyOfficeGuid, @CompanyId, @CompanyGuid, @CreatedById, @StatusId, @CreatedTime, @CompanyOfficeName, @SrAddressType, @CountryId, @StateId, @CityId, @PostalCodeId, @Address, @Latitude, @Longitude, @RadiusMeters);
                {dialect.ReturnIdentitySuffix("CompanyOfficeId")}";
            companyOffice.CompanyOfficeId = await conn.QuerySingleAsync<long>(sql, companyOffice, transaction);
        }

        public async Task UpdateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(companyOffice);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [CompanyId] = @CompanyId,
                    [CompanyOfficeName] = @CompanyOfficeName,
                    [SrAddressType] = @SrAddressType,
                    [CountryId] = @CountryId,
                    [StateId] = @StateId,
                    [CityId] = @CityId,
                    [PostalCodeId] = @PostalCodeId,
                    [Address] = @Address,
                    [Latitude] = @Latitude,
                    [Longitude] = @Longitude,
                    [RadiusMeters] = @RadiusMeters,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE CompanyOfficeGuid = @CompanyOfficeGuid";
            await conn.ExecuteAsync(sql, companyOffice, transaction);
        }

        public async Task SoftDeleteCompanyOfficeAsync(CompanyOffice companyOffice, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(companyOffice, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CompanyOfficeGuid = @CompanyOfficeGuid";

            await conn.ExecuteAsync(sql, companyOffice, transaction);
        }

        public async Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CompanyOfficeGuid = @companyOfficeGuid";
            await conn.ExecuteAsync(sql, new { companyOfficeGuid }, transaction);
        }

        public async Task DeleteByCompanyGuidAsync(Guid companyGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE CompanyId IN (SELECT CompanyId FROM [app].[Company] WHERE CompanyGuid = @companyGuid)";
            await conn.ExecuteAsync(sql, new { companyGuid }, transaction);
        }

        public async Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName,
            string? companyOfficeNameSearchType,
            string? address,
            string? addressSearchType,
            Guid companyGuid, Guid companyOfficeGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (companyGuid != Guid.Empty)
            {
                whereClauses.Add("j_CompanyId.CompanyGuid = @companyGuid");
                parameters.Add("@companyGuid", companyGuid);
            }
            if (companyOfficeGuid != Guid.Empty)
            {
                whereClauses.Add("a.CompanyOfficeGuid = @companyOfficeGuid");
                parameters.Add("@companyOfficeGuid", companyOfficeGuid);
            }

            if (!string.IsNullOrWhiteSpace(companyOfficeName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeName", "@companyOfficeName", companyOfficeNameSearchType, parameters, "companyOfficeName", companyOfficeName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CompanyId.CompanyName AS CompanyName
, j_CountryId.CountryName AS CountryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName


                FROM {_table} a
LEFT JOIN [core].[City] j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN [app].[Company] j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN [core].[Country] j_CountryId ON a.CountryId = j_CountryId.CountryId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
    WHERE StandardReferenceItemId = a.SrAddressType
) v_SrAddressType

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CompanyOfficeId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<CompanyOffice>(sql, parameters, transaction);
        }
    }
}
