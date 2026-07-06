using Contracts;
using Dapper;
using Entities.Models;

namespace Repository
{
    public class AdministrativeAreaDisplayRepository : IAdministrativeAreaDisplayRepository
    {
        private readonly RepositoryContext _context;

        public AdministrativeAreaDisplayRepository(RepositoryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdministrativeAreaDisplay>> GetAllAsync(Shared.RequestFeatures.AdministrativeAreaParameters parameters, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                SELECT CountryId, CountryGuid, CountryIso, CountryIso3, CountryName, PhoneCode, CurrencyCode,
                       ProvinceId, ProvinceGuid, ProvinceName, 
                       CityId, CityGuid, CityName, 
                       DistrictId, DistrictGuid, DistrictName, 
                       SubDistrictId, SubDistrictGuid, SubDistrictName, 
                       PostalCodeId, PostalCodeGuid, PostalCode
                FROM [core].[vw_AdministrativeArea]
                WHERE 1 = 1 ";

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                sql += @" AND (
                    CountryName LIKE @SearchTerm OR
                    ProvinceName LIKE @SearchTerm OR
                    CityName LIKE @SearchTerm OR
                    DistrictName LIKE @SearchTerm OR
                    SubDistrictName LIKE @SearchTerm OR
                    PostalCode LIKE @SearchTerm
                )";
            }

            if (!string.IsNullOrWhiteSpace(parameters.CountryName)) sql += " AND CountryName LIKE @CountryName";
            if (!string.IsNullOrWhiteSpace(parameters.ProvinceName)) sql += " AND ProvinceName LIKE @ProvinceName";
            if (!string.IsNullOrWhiteSpace(parameters.CityName)) sql += " AND CityName LIKE @CityName";
            if (!string.IsNullOrWhiteSpace(parameters.DistrictName)) sql += " AND DistrictName LIKE @DistrictName";
            if (!string.IsNullOrWhiteSpace(parameters.SubDistrictName)) sql += " AND SubDistrictName LIKE @SubDistrictName";
            if (!string.IsNullOrWhiteSpace(parameters.PostalCode)) sql += " AND PostalCode = @PostalCode";

            // A default limit might be wise, but keeping it simple for now, ordering by Country, Province, etc.
            sql += " ORDER BY CountryName, ProvinceName, CityName, DistrictName, SubDistrictName, PostalCode";

            var dapperParams = new
            {
                SearchTerm = string.IsNullOrWhiteSpace(parameters.SearchTerm) ? null : $"%{parameters.SearchTerm}%",
                CountryName = string.IsNullOrWhiteSpace(parameters.CountryName) ? null : $"%{parameters.CountryName}%",
                ProvinceName = string.IsNullOrWhiteSpace(parameters.ProvinceName) ? null : $"%{parameters.ProvinceName}%",
                CityName = string.IsNullOrWhiteSpace(parameters.CityName) ? null : $"%{parameters.CityName}%",
                DistrictName = string.IsNullOrWhiteSpace(parameters.DistrictName) ? null : $"%{parameters.DistrictName}%",
                SubDistrictName = string.IsNullOrWhiteSpace(parameters.SubDistrictName) ? null : $"%{parameters.SubDistrictName}%",
                PostalCode = parameters.PostalCode
            };

            return await connection.QueryAsync<AdministrativeAreaDisplay>(sql, dapperParams);
        }

        public async Task<AdministrativeAreaDisplay> GetOneAsync(string id, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            string sql = @"
                SELECT CountryId, CountryGuid, CountryIso, CountryIso3, CountryName, PhoneCode, CurrencyCode,
                       ProvinceId, ProvinceGuid, ProvinceName, 
                       CityId, CityGuid, CityName, 
                       DistrictId, DistrictGuid, DistrictName, 
                       SubDistrictId, SubDistrictGuid, SubDistrictName, 
                       PostalCodeId, PostalCodeGuid, PostalCode
                FROM [core].[vw_AdministrativeArea]
                WHERE PostalCode = @Id";

            return await connection.QueryFirstOrDefaultAsync<AdministrativeAreaDisplay>(sql, new { Id = id });
        }
    }
}
