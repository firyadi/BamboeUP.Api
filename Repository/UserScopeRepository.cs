using Contracts;
using Dapper;
using Entities.Models;

namespace Repository;

public class UserScopeRepository : IUserScopeRepository
{
    private readonly RepositoryContext _context;

    public UserScopeRepository(RepositoryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Company>> GetAccessibleCompaniesAsync(long userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT DISTINCT c.*
            FROM [core].[UserGroupScope] ugs
            INNER JOIN [app].[Company] c ON c.CompanyId = ugs.CompanyId
            WHERE ugs.UserId = @userId
              AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
              AND c.StatusId > 0 AND c.DeletedTime IS NULL
            ORDER BY c.CompanyName
            """;
        return await connection.QueryAsync<Company>(sql, new { userId });
    }

    public async Task<IEnumerable<CompanyOffice>> GetAccessibleOfficesAsync(long userId, long companyId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT co.*
            FROM [app].[CompanyOffice] co
            WHERE co.CompanyId = @companyId
              AND co.StatusId > 0 AND co.DeletedTime IS NULL
              AND (
                EXISTS (
                    SELECT 1 FROM [core].[UserGroupScope] ugs
                    WHERE ugs.UserId = @userId AND ugs.CompanyId = @companyId
                      AND ugs.CompanyOfficeId IS NULL
                      AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                )
                OR EXISTS (
                    SELECT 1 FROM [core].[UserGroupScope] ugs
                    WHERE ugs.UserId = @userId AND ugs.CompanyId = @companyId
                      AND ugs.CompanyOfficeId = co.CompanyOfficeId
                      AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                )
              )
            ORDER BY co.CompanyOfficeName
            """;
        return await connection.QueryAsync<CompanyOffice>(sql, new { userId, companyId });
    }

    public async Task<IEnumerable<CompanyOffice>> GetAllAccessibleOfficesAsync(long userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT DISTINCT co.*
            FROM [app].[CompanyOffice] co
            WHERE co.StatusId > 0 AND co.DeletedTime IS NULL
              AND (
                EXISTS (
                    SELECT 1 FROM [core].[UserGroupScope] ugs
                    WHERE ugs.UserId = @userId AND ugs.CompanyId = co.CompanyId
                      AND ugs.CompanyOfficeId IS NULL
                      AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                )
                OR EXISTS (
                    SELECT 1 FROM [core].[UserGroupScope] ugs
                    WHERE ugs.UserId = @userId AND ugs.CompanyId = co.CompanyId
                      AND ugs.CompanyOfficeId = co.CompanyOfficeId
                      AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                )
              )
            ORDER BY co.CompanyOfficeName
            """;
        return await connection.QueryAsync<CompanyOffice>(sql, new { userId });
    }

    public async Task<bool> CanAccessCompanyAsync(long userId, long companyId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM [core].[UserGroupScope] ugs
                WHERE ugs.UserId = @userId AND ugs.CompanyId = @companyId
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
            ) THEN 1 ELSE 0 END
            """;
        return await connection.ExecuteScalarAsync<bool>(sql, new { userId, companyId });
    }

    public async Task<bool> CanAccessOfficeAsync(long userId, long companyId, long companyOfficeId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1 FROM [core].[UserGroupScope] ugs
                WHERE ugs.UserId = @userId AND ugs.CompanyId = @companyId
                  AND ugs.CompanyOfficeId IS NULL
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
            ) OR EXISTS (
                SELECT 1 FROM [core].[UserGroupScope] ugs
                WHERE ugs.UserId = @userId AND ugs.CompanyId = @companyId
                  AND ugs.CompanyOfficeId = @companyOfficeId
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
            ) THEN 1 ELSE 0 END
            """;
        return await connection.ExecuteScalarAsync<bool>(sql, new { userId, companyId, companyOfficeId });
    }

    public async Task<IEnumerable<OrganizationUnit>> GetAccessibleOrganizationUnitsAsync(long userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT DISTINCT ou.*
            FROM [app].[OrganizationUnit] ou
            WHERE ou.StatusId > 0 AND ou.DeletedTime IS NULL
              AND EXISTS (
                SELECT 1
                FROM [app].[OrganizationUnitScope] ous
                INNER JOIN [core].[UserGroupScope] ugs
                    ON ugs.UserId = @userId
                   AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                WHERE ous.OrganizationUnitId = ou.OrganizationUnitId
                  AND ous.StatusId > 0 AND ous.DeletedTime IS NULL
                  AND (
                    (ous.ScopeType = 'COMPANY' AND ous.CompanyId = ugs.CompanyId)
                    OR (
                        ous.ScopeType = 'OFFICE'
                        AND ous.CompanyId = ugs.CompanyId
                        AND (ugs.CompanyOfficeId IS NULL OR ous.CompanyOfficeId = ugs.CompanyOfficeId)
                    )
                  )
              )
            ORDER BY ou.OrganizationUnitId DESC
            """;
        return await connection.QueryAsync<OrganizationUnit>(sql, new { userId });
    }

    public async Task<bool> CanAccessOrganizationUnitAsync(long userId, long organizationUnitId)
    {
        using var connection = _context.CreateConnection();
        const string sql = """
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM [app].[OrganizationUnitScope] ous
                INNER JOIN [core].[UserGroupScope] ugs
                    ON ugs.UserId = @userId
                   AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                WHERE ous.OrganizationUnitId = @organizationUnitId
                  AND ous.StatusId > 0 AND ous.DeletedTime IS NULL
                  AND (
                    (ous.ScopeType = 'COMPANY' AND ous.CompanyId = ugs.CompanyId)
                    OR (
                        ous.ScopeType = 'OFFICE'
                        AND ous.CompanyId = ugs.CompanyId
                        AND (ugs.CompanyOfficeId IS NULL OR ous.CompanyOfficeId = ugs.CompanyOfficeId)
                    )
                  )
            ) THEN 1 ELSE 0 END
            """;
        return await connection.ExecuteScalarAsync<bool>(sql, new { userId, organizationUnitId });
    }
}
