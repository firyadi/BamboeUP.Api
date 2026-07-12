using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    // Detail version (child-aware): unified style with whereClauses + parameters
    public partial class CompanyOfficeRepository : ICompanyOfficeRepository
    {
        private readonly RepositoryContext _context;
        private readonly IUserContext _userContext;

        public CompanyOfficeRepository(RepositoryContext context, IUserContext userContext)
        {
            _context = context;
            _userContext = userContext;
        }

        public async Task<CompanyOffice?> GetCompanyOfficeAsync(Guid companyOfficeGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                , ISNULL(c.CityName, '') AS CityName
                , ISNULL(p.ProvinceName, '') AS StateName
                , ISNULL(co.CountryName, '') AS CountryName
                FROM [app].[CompanyOffice] a
                OUTER APPLY (
                    SELECT StandardReferenceItemName
                    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                    WHERE StandardReferenceItemId = a.SrAddressType
                ) sri0
                LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                WHERE a.CompanyOfficeGuid = @companyOfficeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new { companyOfficeGuid });
        }

        public async Task<CompanyOffice?> GetCompanyOfficeByIdAsync(long companyOfficeId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                , ISNULL(c.CityName, '') AS CityName
                , ISNULL(p.ProvinceName, '') AS StateName
                , ISNULL(co.CountryName, '') AS CountryName
                FROM [app].[CompanyOffice] a
                OUTER APPLY (
                    SELECT StandardReferenceItemName
                    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                    WHERE StandardReferenceItemId = a.SrAddressType
                ) sri0
                LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                WHERE a.CompanyOfficeId = @companyOfficeId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new { companyOfficeId });
        }

        public async Task<IEnumerable<CompanyOffice>> GetAllCompanyOfficesAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT a.*
                , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                , ISNULL(c.CityName, '') AS CityName
                , ISNULL(p.ProvinceName, '') AS StateName
                , ISNULL(co.CountryName, '') AS CountryName
                FROM [app].[CompanyOffice] a
                OUTER APPLY (
                    SELECT StandardReferenceItemName
                    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                    WHERE StandardReferenceItemId = a.SrAddressType
                ) sri0
                LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.CompanyOfficeId DESC";
            return await connection.QueryAsync<CompanyOffice>(sql);
        }

        public async Task CreateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(companyOffice);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[CompanyOffice]
                (CompanyOfficeGuid, CreatedById, StatusId, CreatedTime, CompanyId, CompanyGuid, CompanyOfficeName, SrAddressType, CountryId, StateId, CityId, PostalCodeId, Address)
                VALUES
                (@CompanyOfficeGuid, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyGuid, @CompanyOfficeName, @SrAddressType, @CountryId, @StateId, @CityId, @PostalCodeId, @Address);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            companyOffice.CompanyOfficeId = await conn.QuerySingleAsync<long>(sql, companyOffice, transaction);
        }

        public async Task UpdateCompanyOfficeAsync(CompanyOffice companyOffice, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(companyOffice);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                UPDATE [app].[CompanyOffice]
                SET CompanyId = @CompanyId,
                                      CompanyGuid = @CompanyGuid,
                                      CompanyOfficeName = @CompanyOfficeName,
                                      SrAddressType = @SrAddressType,
                                      CountryId = @CountryId,
                                      StateId = @StateId,
                                      CityId = @CityId,
                                      PostalCodeId = @PostalCodeId,
                                      Address = @Address,
                                      UpdatedById = @UpdatedById,
                                      UpdatedTime = @UpdatedTime,
                                      StatusId = @StatusId
                WHERE CompanyOfficeGuid = @CompanyOfficeGuid";
            await conn.ExecuteAsync(sql, companyOffice, transaction);
        }

        public async Task SoftDeleteCompanyOfficeAsync(CompanyOffice companyOffice, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(companyOffice, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                UPDATE [app].[CompanyOffice]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE CompanyOfficeGuid = @CompanyOfficeGuid";

            await conn.ExecuteAsync(sql, companyOffice, transaction);
        }

        public async Task DeleteCompanyOfficeAsync(Guid companyOfficeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();

            const string sql = @"
                DELETE FROM [app].[CompanyOffice]
                WHERE CompanyOfficeGuid = @companyOfficeGuid";
            await conn.ExecuteAsync(sql, new { companyOfficeGuid }, transaction);
        }

        public async Task<IEnumerable<CompanyOffice>> SearchCompanyOfficeAsync(
            string? companyOfficeName, string? companyOfficeNameSearchType,
            long? srAddressType,
            long? countryId, 
            long? stateId, 
            long? cityId, 
            string? postalCodeId, string? postalCodeIdSearchType,
            string? address, string? addressSearchType, 
            Guid companyGuid, Guid companyOfficeGuid,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            // 🔍 CompanyOfficeName
if (!string.IsNullOrWhiteSpace(companyOfficeName))
{
    var param = SqlFilterHelper.BuildFilter("a.CompanyOfficeName", "@companyOfficeName", companyOfficeNameSearchType, parameters, "companyOfficeName", companyOfficeName);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 Address
if (!string.IsNullOrWhiteSpace(address))
{
    var param = SqlFilterHelper.BuildFilter("a.Address", "@address", addressSearchType, parameters, "address", address);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 PostalCodeId
if (!string.IsNullOrWhiteSpace(postalCodeId))
{
    var param = SqlFilterHelper.BuildFilter("a.PostalCodeId", "@postalCodeId", postalCodeIdSearchType, parameters, "postalCodeId", postalCodeId);
    if (!string.IsNullOrWhiteSpace(param))
        whereClauses.Add(param);
}

// 🔍 Exact matches
if (srAddressType.HasValue) { whereClauses.Add("a.SrAddressType = @srAddressType"); parameters.Add("srAddressType", srAddressType.Value); }
if (countryId.HasValue) { whereClauses.Add("a.CountryId = @countryId"); parameters.Add("countryId", countryId.Value); }
if (stateId.HasValue) { whereClauses.Add("a.StateId = @stateId"); parameters.Add("stateId", stateId.Value); }
if (cityId.HasValue) { whereClauses.Add("a.CityId = @cityId"); parameters.Add("cityId", cityId.Value); }

            // 🔒 Scope: Company/Office/Global
            if (_userContext.IsAdmin)
            {
                whereClauses.Add(@"(
                    (a.CompanyGuid = @companyGuid AND a.CompanyOfficeGuid = @companyOfficeGuid)
                    OR (a.CompanyGuid = @companyGuid AND a.CompanyOfficeGuid = '00000000-0000-0000-0000-000000000000')
                    OR (a.CompanyGuid = '00000000-0000-0000-0000-000000000000' AND a.CompanyOfficeGuid = '00000000-0000-0000-0000-000000000000'))");
                parameters.Add("companyGuid", companyGuid);
                parameters.Add("companyOfficeGuid", companyOfficeGuid);
            }
            else if (_userContext.CompanyId.HasValue)
            {
                whereClauses.Add("a.CompanyId = @contextCompanyId");
                parameters.Add("contextCompanyId", _userContext.CompanyId.Value);

                if (_userContext.OfficeId.HasValue)
                {
                    whereClauses.Add("(a.CompanyOfficeId = @contextOfficeId OR a.CompanyOfficeId = 0)");
                    parameters.Add("contextOfficeId", _userContext.OfficeId.Value);
                }
            }
            else
            {
                // Non-admin without any company scope selected = should see nothing or only global
                whereClauses.Add("a.CompanyId = -1"); 
            }

            var sql = $@"
                SELECT a.*
                , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                , ISNULL(c.CityName, '') AS CityName
                , ISNULL(p.ProvinceName, '') AS StateName
                , ISNULL(co.CountryName, '') AS CountryName
                FROM [app].[CompanyOffice] a
                OUTER APPLY (
                    SELECT StandardReferenceItemName
                    FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                    WHERE StandardReferenceItemId = a.SrAddressType
                ) sri0
                LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.CompanyOfficeId DESC";

            return await connection.QueryAsync<CompanyOffice>(sql, parameters, transaction);
        }

        // Detail helpers (only emitted if entity has a parent)
        public async Task<IEnumerable<CompanyOffice>> GetAllByCompanyGuidAsync(Guid companyGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT a.*
                        , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                        , ISNULL(c.CityName, '') AS CityName
                        , ISNULL(p.ProvinceName, '') AS StateName
                        , ISNULL(co.CountryName, '') AS CountryName
                        FROM [app].[CompanyOffice] a
                        OUTER APPLY (
                            SELECT StandardReferenceItemName
                            FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                            WHERE StandardReferenceItemId = a.SrAddressType
                        ) sri0
                        LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                        LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                        LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                        WHERE a.CompanyGuid = @companyGuid AND a.StatusId > 0 AND a.DeletedTime IS NULL
                        ORDER BY a.CompanyOfficeId DESC";
            return await connection.QueryAsync<CompanyOffice>(sql, new { companyGuid });
        }

        public async Task<CompanyOffice?> GetByCompanyGuidAndCompanyOfficeGuidAsync(Guid companyGuid, Guid companyOfficeGuid)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT a.*
                        , ISNULL(sri0.StandardReferenceItemName, '') AS AddressTypeName
                        , ISNULL(c.CityName, '') AS CityName
                        , ISNULL(p.ProvinceName, '') AS StateName
                        , ISNULL(co.CountryName, '') AS CountryName
                        FROM [app].[CompanyOffice] a
                        OUTER APPLY (
                            SELECT StandardReferenceItemName
                            FROM [app].[fn_GetStandardReferenceItems](a.CompanyId, a.CompanyOfficeId, NULL, 'AddressType')
                            WHERE StandardReferenceItemId = a.SrAddressType
                        ) sri0
                        LEFT JOIN [core].[City] c ON a.CityId = c.CityId
                        LEFT JOIN [core].[Province] p ON a.StateId = p.ProvinceId
                        LEFT JOIN [core].[Country] co ON a.CountryId = co.CountryId
                        WHERE a.CompanyGuid = @companyGuid
                          AND a.CompanyOfficeGuid = @companyOfficeGuid
                          AND a.StatusId > 0 AND a.DeletedTime IS NULL";

            return await connection.QuerySingleOrDefaultAsync<CompanyOffice>(sql, new {
                companyGuid,
                companyOfficeGuid
            });
        }
    }
}


