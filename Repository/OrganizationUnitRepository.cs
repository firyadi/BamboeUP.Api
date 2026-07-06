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
    public partial class OrganizationUnitRepository : IOrganizationUnitRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public OrganizationUnitRepository(RepositoryContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<OrganizationUnit> GetOrganizationUnitAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM [app].[OrganizationUnit] a
LEFT JOIN [app].[OrganizationUnit] j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.OrganizationUnitGuid = @organizationUnitGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnit>(sql, new { organizationUnitGuid });
        }

        public async Task<OrganizationUnit?> GetOrganizationUnitByIdAsync(long organizationUnitId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM [app].[OrganizationUnit] a
LEFT JOIN [app].[OrganizationUnit] j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.OrganizationUnitId = @organizationUnitId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnit>(sql, new { organizationUnitId });
        }

        public async Task<IEnumerable<OrganizationUnit>> GetAllOrganizationUnitsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM [app].[OrganizationUnit] a
LEFT JOIN [app].[OrganizationUnit] j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.OrganizationUnitId DESC";
            return await connection.QueryAsync<OrganizationUnit>(sql);
        }

        public async Task CreateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(organizationUnit);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[OrganizationUnit]
                (OrganizationUnitGuid, CreatedById, StatusId, CreatedTime, OrganizationUnitCode, OrganizationUnitName,
                 ParentOrganizationUnitId, SrOrganizationLevel, LevelDepth, HierarchyPath)
                VALUES
                (@OrganizationUnitGuid, @CreatedById, @StatusId, @CreatedTime, @OrganizationUnitCode, @OrganizationUnitName,
                 @ParentOrganizationUnitId, @SrOrganizationLevel, @LevelDepth, @HierarchyPath);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            organizationUnit.OrganizationUnitId = await conn.QuerySingleAsync<long>(sql, organizationUnit, transaction);
        }

        public async Task UpdateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(organizationUnit);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[OrganizationUnit]
                SET OrganizationUnitCode = @OrganizationUnitCode,
                    OrganizationUnitName = @OrganizationUnitName,
                    ParentOrganizationUnitId = @ParentOrganizationUnitId,
                    SrOrganizationLevel = @SrOrganizationLevel,
                    LevelDepth = @LevelDepth,
                    HierarchyPath = @HierarchyPath,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE OrganizationUnitGuid = @OrganizationUnitGuid";
            await conn.ExecuteAsync(sql, organizationUnit, transaction);
        }

        public async Task SoftDeleteOrganizationUnitAsync(OrganizationUnit organizationUnit, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(organizationUnit, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                UPDATE [app].[OrganizationUnit]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE OrganizationUnitGuid = @OrganizationUnitGuid";

            await conn.ExecuteAsync(sql, organizationUnit, transaction);
        }

        public async Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"DELETE FROM [app].[OrganizationUnit] WHERE OrganizationUnitGuid = @organizationUnitGuid";
            await conn.ExecuteAsync(sql, new { organizationUnitGuid }, transaction);
        }

        public async Task<IEnumerable<OrganizationUnit>> SearchOrganizationUnitAsync(
            string? organizationUnitCode, string? organizationUnitCodeSearchType,
            string? organizationUnitName, string? organizationUnitNameSearchType,
string? parentOrganizationUnitName, string? parentOrganizationUnitNameSearchType,

            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(organizationUnitCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.OrganizationUnitCode", "@organizationUnitCode", organizationUnitCodeSearchType, parameters, "organizationUnitCode", organizationUnitCode);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(organizationUnitName))
            {
                var param = SqlFilterHelper.BuildFilter("a.OrganizationUnitName", "@organizationUnitName", organizationUnitNameSearchType, parameters, "organizationUnitName", organizationUnitName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

if (!string.IsNullOrWhiteSpace(parentOrganizationUnitName)) { var param = SqlFilterHelper.BuildFilter("j_ParentOrganizationUnitId.OrganizationUnitName", "@parentOrganizationUnitName", parentOrganizationUnitNameSearchType, parameters, "parentOrganizationUnitName", parentOrganizationUnitName); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM [app].[OrganizationUnit] a
LEFT JOIN [app].[OrganizationUnit] j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.OrganizationUnitId DESC";

            return await connection.QueryAsync<OrganizationUnit>(sql, parameters, transaction);
        }
    }
}
