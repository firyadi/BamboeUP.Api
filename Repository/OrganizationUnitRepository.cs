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
    public partial class OrganizationUnitRepository : IOrganizationUnitRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;
        private readonly ISqlDialect _dialect;
        private readonly string _table;
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public OrganizationUnitRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect)
        {
            _context = context;
            _audit = audit;
            _dialect = dialect;
            _table = dialect.Qualify("app", "OrganizationUnit");
        }

        public async Task<OrganizationUnit?> GetOrganizationUnitAsync(Guid organizationUnitGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM {_table} a
LEFT JOIN {_table} j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.OrganizationUnitGuid = @organizationUnitGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnit>(sql, new { organizationUnitGuid });
        }

        public async Task<OrganizationUnit?> GetOrganizationUnitByIdAsync(long organizationUnitId, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM {_table} a
LEFT JOIN {_table} j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.OrganizationUnitId = @organizationUnitId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<OrganizationUnit>(sql, new { organizationUnitId });
        }

        public async Task<IEnumerable<OrganizationUnit>> GetAllOrganizationUnitsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM {_table} a
LEFT JOIN {_table} j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.OrganizationUnitId DESC{_dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<OrganizationUnit>(sql);
        }

        public async Task CreateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(organizationUnit);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (OrganizationUnitGuid, CreatedById, StatusId, CreatedTime, OrganizationUnitCode, OrganizationUnitName, ParentOrganizationUnitId, SrOrganizationLevel, LevelDepth, HierarchyPath)
                VALUES
                (@OrganizationUnitGuid, @CreatedById, @StatusId, @CreatedTime, @OrganizationUnitCode, @OrganizationUnitName, @ParentOrganizationUnitId, @SrOrganizationLevel, @LevelDepth, @HierarchyPath);
                {_dialect.ReturnIdentitySuffix("OrganizationUnitId")}";
            organizationUnit.OrganizationUnitId = await conn.QuerySingleAsync<long>(sql, organizationUnit, transaction);
        }

        public async Task UpdateOrganizationUnitAsync(OrganizationUnit organizationUnit, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(organizationUnit);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     OrganizationUnitCode = @OrganizationUnitCode,
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE OrganizationUnitGuid = @OrganizationUnitGuid";

            await conn.ExecuteAsync(sql, organizationUnit, transaction);
        }

        public async Task DeleteOrganizationUnitAsync(Guid organizationUnitGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE OrganizationUnitGuid = @organizationUnitGuid";
            await conn.ExecuteAsync(sql, new { organizationUnitGuid }, transaction);
        }

        public async Task<IEnumerable<OrganizationUnit>> SearchOrganizationUnitAsync(
            string? organizationUnitCode,
            string? organizationUnitCodeSearchType,
            string? organizationUnitName,
            string? organizationUnitNameSearchType,
            string? srOrganizationLevel,
            string? srOrganizationLevelSearchType,
            string? levelDepth,
            string? levelDepthSearchType,
            string? hierarchyPath,
            string? hierarchyPathSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? _context.CreateConnection();
            var whereClauses = new List<string> { "a.StatusId > 0", "a.DeletedTime IS NULL" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(organizationUnitCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.OrganizationUnitCode", "@organizationUnitCode", organizationUnitCodeSearchType, parameters, "organizationUnitCode", organizationUnitCode, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(organizationUnitName))
            {
                var param = SqlFilterHelper.BuildFilter("a.OrganizationUnitName", "@organizationUnitName", organizationUnitNameSearchType, parameters, "organizationUnitName", organizationUnitName, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srOrganizationLevel))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrOrganizationLevel", "@srOrganizationLevel", srOrganizationLevelSearchType, parameters, "srOrganizationLevel", srOrganizationLevel, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(levelDepth))
            {
                var param = SqlFilterHelper.BuildFilter("a.LevelDepth", "@levelDepth", levelDepthSearchType, parameters, "levelDepth", levelDepth, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hierarchyPath))
            {
                var param = SqlFilterHelper.BuildFilter("a.HierarchyPath", "@hierarchyPath", hierarchyPathSearchType, parameters, "hierarchyPath", hierarchyPath, _dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(_dialect, _maxRows, "a.*")}
, j_ParentOrganizationUnitId.OrganizationUnitName AS ParentOrganizationUnitName

                FROM {_table} a
LEFT JOIN {_table} j_ParentOrganizationUnitId ON a.ParentOrganizationUnitId = j_ParentOrganizationUnitId.OrganizationUnitId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.OrganizationUnitId DESC{_dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<OrganizationUnit>(sql, parameters, transaction);
        }
    }
}
