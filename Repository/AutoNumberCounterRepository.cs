using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public class AutoNumberCounterRepository : IAutoNumberCounterRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _audit;

        public AutoNumberCounterRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _audit = auditService;
        }

        public async Task<AutoNumberCounter> GetAutoNumberCounterAsync(Guid autoNumberCounterGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberCounter] a
                WHERE a.AutoNumberCounterGuid = @autoNumberCounterGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<AutoNumberCounter>(sql, new { autoNumberCounterGuid });
        }

        public async Task<IEnumerable<AutoNumberCounter>> GetAllAutoNumberCountersAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberCounter] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AutoNumberCounterId DESC";
            return await connection.QueryAsync<AutoNumberCounter>(sql);
        }

        public async Task CreateAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(autoNumberCounter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[AutoNumberCounter]
                (AutoNumberCounterGuid, AutoNumberTemplateId,
                 CompanyId, CompanyOfficeId, OrganizationUnitId,
                 YearNo, MonthNo, DayNo,
                 LastNumber,
                 StatusId, CreatedById, CreatedTime)
                VALUES
                (@AutoNumberCounterGuid, @AutoNumberTemplateId,
                 @CompanyId, @CompanyOfficeId, @OrganizationUnitId,
                 @YearNo, @MonthNo, @DayNo,
                 @LastNumber,
                 @StatusId, @CreatedById, @CreatedTime)";
            await conn.ExecuteAsync(sql, autoNumberCounter, transaction);

            await _audit.LogAsync(
                actionType: "CREATE",
                tableName: "AutoNumberCounter",
                primaryKey: autoNumberCounter.AutoNumberCounterGuid.ToString(),
                userId: autoNumberCounter.CreatedById.ToString(),
                oldEntity: null,
                newEntity: autoNumberCounter);
        }

        public async Task UpdateAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(autoNumberCounter);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberCounterAsync(autoNumberCounter.AutoNumberCounterGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberCounter]
                SET AutoNumberTemplateId = @AutoNumberTemplateId,
                    CompanyId            = @CompanyId,
                    CompanyOfficeId      = @CompanyOfficeId,
                    OrganizationUnitId   = @OrganizationUnitId,
                    YearNo               = @YearNo,
                    MonthNo              = @MonthNo,
                    DayNo                = @DayNo,
                    LastNumber           = @LastNumber,
                    StatusId             = @StatusId,
                    UpdatedById          = @UpdatedById,
                    UpdatedTime          = @UpdatedTime
                WHERE AutoNumberCounterGuid = @AutoNumberCounterGuid";
            await conn.ExecuteAsync(sql, autoNumberCounter, transaction);

            await _audit.LogAsync(
                actionType: "UPDATE",
                tableName: "AutoNumberCounter",
                primaryKey: autoNumberCounter.AutoNumberCounterGuid.ToString(),
                userId: autoNumberCounter.UpdatedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: autoNumberCounter);
        }

        public async Task SoftDeleteAutoNumberCounterAsync(AutoNumberCounter autoNumberCounter, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(autoNumberCounter, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberCounterAsync(autoNumberCounter.AutoNumberCounterGuid, false);

            const string sql = @"
                UPDATE [app].[AutoNumberCounter]
                SET StatusId    = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AutoNumberCounterGuid = @AutoNumberCounterGuid";

            await conn.ExecuteAsync(sql, autoNumberCounter, transaction);

            await _audit.LogAsync(
                actionType: "DELETE",
                tableName: "AutoNumberCounter",
                primaryKey: autoNumberCounter.AutoNumberCounterGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task DeleteAutoNumberCounterAsync(Guid autoNumberCounterGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetAutoNumberCounterAsync(autoNumberCounterGuid, false);

            const string sql = @"DELETE FROM [app].[AutoNumberCounter] WHERE AutoNumberCounterGuid = @autoNumberCounterGuid";
            await conn.ExecuteAsync(sql, new { autoNumberCounterGuid }, transaction);

            await _audit.LogAsync(
                actionType: "DELETE_ADMIN",
                tableName: "AutoNumberCounter",
                primaryKey: autoNumberCounterGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<AutoNumberCounter>> SearchAutoNumberCounterAsync(
            long? autoNumberTemplateId,
            long? companyId,
            long? companyOfficeId,
            int? organizationUnitId,
            int? yearNo,
            int? monthNo,
            int? dayNo,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };
            var parameters = new DynamicParameters();

            if (autoNumberTemplateId.HasValue)
            {
                whereClauses.Add("a.AutoNumberTemplateId = @autoNumberTemplateId");
                parameters.Add("autoNumberTemplateId", autoNumberTemplateId.Value);
            }
            if (companyId.HasValue)
            {
                whereClauses.Add("a.CompanyId = @companyId");
                parameters.Add("companyId", companyId.Value);
            }
            if (companyOfficeId.HasValue)
            {
                whereClauses.Add("a.CompanyOfficeId = @companyOfficeId");
                parameters.Add("companyOfficeId", companyOfficeId.Value);
            }
            if (organizationUnitId.HasValue)
            {
                whereClauses.Add("a.OrganizationUnitId = @organizationUnitId");
                parameters.Add("organizationUnitId", organizationUnitId.Value);
            }
            if (yearNo.HasValue)
            {
                whereClauses.Add("a.YearNo = @yearNo");
                parameters.Add("yearNo", yearNo.Value);
            }
            if (monthNo.HasValue)
            {
                whereClauses.Add("a.MonthNo = @monthNo");
                parameters.Add("monthNo", monthNo.Value);
            }
            if (dayNo.HasValue)
            {
                whereClauses.Add("a.DayNo = @dayNo");
                parameters.Add("dayNo", dayNo.Value);
            }

            var sql = $@"
                SELECT a.*
                FROM [app].[AutoNumberCounter] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AutoNumberCounterId DESC";

            return await connection.QueryAsync<AutoNumberCounter>(sql, parameters, transaction);
        }

        // Detail helpers — scoped by parent AutoNumberTemplateGuid
        public async Task<IEnumerable<AutoNumberCounter>> GetAllByAutoNumberTemplateGuidAsync(Guid autoNumberTemplateGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
                FROM [app].[AutoNumberCounter] a
                INNER JOIN [app].[AutoNumberTemplate] t
                    ON a.AutoNumberTemplateId = t.AutoNumberTemplateId
                WHERE t.AutoNumberTemplateGuid = @autoNumberTemplateGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.YearNo, a.MonthNo, a.DayNo, a.AutoNumberCounterId ASC";
            return await connection.QueryAsync<AutoNumberCounter>(sql, new { autoNumberTemplateGuid });
        }
    }
}
