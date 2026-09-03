using Dapper;
using Entities.Models;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public partial class CompanyOfficeRepository
    {
        private string BuildSelectColumns(int maxRows) => $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, maxRows, "a.*")}
, j_CityId.CityName AS CityName
, j_CompanyId.CompanyName AS CompanyName
, j_CountryId.CountryName AS CountryName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrAddressType.StandardReferenceItemName")} AS AddressTypeName";

        private string BuildFromClause() => $@"
                FROM {_table} a
LEFT JOIN {dialect.Qualify("core", "City")} j_CityId ON a.CityId = j_CityId.CityId
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("core", "Country")} j_CountryId ON a.CountryId = j_CountryId.CountryId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrAddressType", "SrAddressType", "AddressType")}";

        public async Task<IEnumerable<CompanyOffice>> GetAllCompanyOfficesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = BuildSelectColumns(_maxRows)
                + BuildFromClause() + $@"
                WHERE a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.CompanyOfficeId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<CompanyOffice>(sql);
        }

        public async Task<CompanyOffice?> GetCompanyOfficeByIdAsync(long companyOfficeId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = BuildSelectColumns(1)
                + BuildFromClause() + @"
                WHERE a.CompanyOfficeId = @companyOfficeId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new { companyOfficeId });
        }

        public async Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName,
            string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId,
            long? stateId,
            long? cityId,
            string? postalCodeId,
            string? postalCodeIdSearchType,
            string? address,
            string? addressSearchType,
            Guid companyGuid,
            Guid companyOfficeGuid)
        {
            using var connection = context.CreateConnection();
            List<string> whereClauses = ["a.StatusId > 0", "a.DeletedTime IS NULL"];
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
                var param = SqlFilterHelper.BuildFilter(
                    "a.CompanyOfficeName", "@companyOfficeName", companyOfficeNameSearchType,
                    parameters, "companyOfficeName", companyOfficeName, dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (srAddressType is > 0)
            {
                whereClauses.Add("a.SrAddressType = @srAddressType");
                parameters.Add("@srAddressType", srAddressType.Value);
            }

            if (countryId is > 0)
            {
                whereClauses.Add("a.CountryId = @countryId");
                parameters.Add("@countryId", countryId.Value);
            }

            if (stateId is > 0)
            {
                whereClauses.Add("a.StateId = @stateId");
                parameters.Add("@stateId", stateId.Value);
            }

            if (cityId is > 0)
            {
                whereClauses.Add("a.CityId = @cityId");
                parameters.Add("@cityId", cityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(postalCodeId))
            {
                var param = SqlFilterHelper.BuildFilter(
                    "a.PostalCodeId", "@postalCodeId", postalCodeIdSearchType,
                    parameters, "postalCodeId", postalCodeId, dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                var param = SqlFilterHelper.BuildFilter(
                    "a.Address", "@address", addressSearchType,
                    parameters, "address", address, dialect);
                if (!string.IsNullOrWhiteSpace(param))
                    whereClauses.Add(param);
            }

            var sql = BuildSelectColumns(_maxRows)
                + BuildFromClause() + $@"
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CompanyOfficeId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<CompanyOffice>(sql, parameters);
        }
    }
}
