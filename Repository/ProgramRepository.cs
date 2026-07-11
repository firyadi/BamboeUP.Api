using BamboeUp.Audit.Contracts;
using Contracts;
using Dapper;
using Entities.Models;
using System.Data;
using Repository.Extensions;

namespace Repository
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly RepositoryContext _context;
        private readonly IAuditService _auditService;

        public ProgramRepository(RepositoryContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<Programs> GetProgramAsync(Guid programGuid, bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) * FROM [core].[Programs] 
                                 WHERE ProgramGuid = @programGuid AND StatusId > 0";
            return await connection.QuerySingleOrDefaultAsync<Programs>(sql, new { programGuid });
        }

        public async Task<IEnumerable<Programs>> GetAllProgramsAsync(bool trackChanges)
        {
            using var connection = _context.CreateConnection();
            var sql = $@"
                SELECT a.*
                FROM [core].[Programs] a
                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.ProgramCode, a.ProgramId ASC";

            return await connection.QueryAsync<Programs>(sql);
        }

        public async Task CreateProgramAsync(Programs program, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampCreate(program);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            const string sql = @"INSERT INTO [core].[Programs]
                                 (ProgramGuid, ProgramCode, ParentId, IconCode, ProgramName, TopLevelProgramId, RootLevel, RowIndex, Note, 
                                  IsParentProgram, IsProgram, IsBeginGroup, ProgramType, IsProgramAddAble, IsProgramEditAble, 
                                  IsProgramDeleteAble, IsProgramViewAble, IsProgramApprovalAble, IsProgramUnApprovalAble, 
                                  IsProgramVoidAble, IsProgramUnVoidAble, IsProgramDirectVoid, IsProgramPrintAble, 
                                  IsMenuAddVisible, IsMenuHomeVisible, IsVisible, NavigateUrl, HelpLinkId, AssemblyName, 
                                  AssemblyClassName, StoreProcedureName, AccessKey, IsActive, StatusId, CreatedById, CreatedTime)
                                 VALUES
                                 (@ProgramGuid, @ProgramCode, @ParentId, @IconCode, @ProgramName, @TopLevelProgramId, @RootLevel, @RowIndex, @Note, 
                                  @IsParentProgram, @IsProgram, @IsBeginGroup, @ProgramType, @IsProgramAddAble, @IsProgramEditAble, 
                                  @IsProgramDeleteAble, @IsProgramViewAble, @IsProgramApprovalAble, @IsProgramUnApprovalAble, 
                                  @IsProgramVoidAble, @IsProgramUnVoidAble, @IsProgramDirectVoid, @IsProgramPrintAble, 
                                  @IsMenuAddVisible, @IsMenuHomeVisible, @IsVisible, @NavigateUrl, @HelpLinkId, @AssemblyName, 
                                  @AssemblyClassName, @StoreProcedureName, @AccessKey, @IsActive, @StatusId, @CreatedById, @CreatedTime)";
            
            await conn.ExecuteAsync(sql, program, transaction);

            await _auditService.LogAsync(
                actionType: "CREATE",
                tableName: "Programs",
                primaryKey: program.ProgramGuid.ToString(),
                userId: program.CreatedById.ToString(),
                oldEntity: null,
                newEntity: program);
        }

        public async Task UpdateProgramAsync(Programs program, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampUpdate(program);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetProgramAsync(program.ProgramGuid, false);

            const string sql = @"UPDATE [core].[Programs]
                                 SET ProgramCode = @ProgramCode,
                                     ParentId = @ParentId,
                                     IconCode = @IconCode,
                                     ProgramName = @ProgramName,
                                     TopLevelProgramId = @TopLevelProgramId,
                                     RootLevel = @RootLevel,
                                     RowIndex = @RowIndex,
                                     Note = @Note,
                                     IsParentProgram = @IsParentProgram,
                                     IsProgram = @IsProgram,
                                     IsBeginGroup = @IsBeginGroup,
                                     ProgramType = @ProgramType,
                                     IsProgramAddAble = @IsProgramAddAble,
                                     IsProgramEditAble = @IsProgramEditAble,
                                     IsProgramDeleteAble = @IsProgramDeleteAble,
                                     IsProgramViewAble = @IsProgramViewAble,
                                     IsProgramApprovalAble = @IsProgramApprovalAble,
                                     IsProgramUnApprovalAble = @IsProgramUnApprovalAble,
                                     IsProgramVoidAble = @IsProgramVoidAble,
                                     IsProgramUnVoidAble = @IsProgramUnVoidAble,
                                     IsProgramDirectVoid = @IsProgramDirectVoid,
                                     IsProgramPrintAble = @IsProgramPrintAble,
                                     IsMenuAddVisible = @IsMenuAddVisible,
                                     IsMenuHomeVisible = @IsMenuHomeVisible,
                                     IsVisible = @IsVisible,
                                     NavigateUrl = @NavigateUrl,
                                     HelpLinkId = @HelpLinkId,
                                     AssemblyName = @AssemblyName,
                                     AssemblyClassName = @AssemblyClassName,
                                     StoreProcedureName = @StoreProcedureName,
                                     AccessKey = @AccessKey,
                                     IsActive = @IsActive,
                                     StatusId = @StatusId,
                                     UpdatedById = @UpdatedById,
                                     UpdatedTime = @UpdatedTime
                                 WHERE ProgramGuid = @ProgramGuid";
            
            await conn.ExecuteAsync(sql, program, transaction);

            await _auditService.LogAsync(
                actionType: "UPDATE",
                tableName: "Programs",
                primaryKey: program.ProgramGuid.ToString(),
                userId: program.UpdatedById?.ToString(),
                oldEntity: oldData,
                newEntity: program);
        }

        public async Task SoftDeleteProgramAsync(Programs program, long deletedBy, IDbTransaction transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(program, deletedBy);
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetProgramAsync(program.ProgramGuid, false);

            const string sql = @"UPDATE [core].[Programs]
                                 SET StatusId = 0,
                                     DeletedById = @DeletedById,
                                     DeletedTime = @DeletedTime
                                 WHERE ProgramGuid = @ProgramGuid";

            await conn.ExecuteAsync(sql, program, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Programs",
                primaryKey: program.ProgramGuid.ToString(),
                userId: deletedBy.ToString(),
                oldEntity: oldData,
                newEntity: program);
        }

        public async Task DeleteProgramAsync(Guid programGuid, IDbTransaction transaction = null)
        {
            var conn = transaction?.Connection ?? _context.CreateConnection();
            var oldData = await GetProgramAsync(programGuid, false);

            const string sql = @"DELETE FROM [core].[Programs]
                                 WHERE ProgramGuid = @programGuid";
            await conn.ExecuteAsync(sql, new { programGuid }, transaction);

            await _auditService.LogAsync(
                actionType: "DELETE",
                tableName: "Programs",
                primaryKey: programGuid.ToString(),
                userId: oldData?.DeletedById?.ToString() ?? "system",
                oldEntity: oldData,
                newEntity: null);
        }

        public async Task<IEnumerable<Programs>> SearchProgramAsync(
            string? programName, string? programNameSearchType,
            string? programCode, string? programCodeSearchType,
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var whereClauses = new List<string>
            {
                "a.StatusId > 0",
                "a.DeletedTime IS NULL"
            };

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(programName))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProgramName", "@programName", programNameSearchType, parameters, "programName", programName);
                whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(programCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProgramCode", "@programCode", programCodeSearchType, parameters, "programCode", programCode);
                whereClauses.Add(param);
            }

            var sql = $@"
                SELECT a.*
                FROM [core].[Programs] a
                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.RowIndex ASC";

            return await connection.QueryAsync<Programs>(sql, parameters, transaction);
        }

        public async Task<IEnumerable<Programs>> GetAllowedProgramsAsync(
            Guid userGuid, 
            string? companyId, 
            string? officeId, 
            IDbTransaction? transaction = null)
        {
            using var connection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("UserGuid", userGuid);

            var sqlBuilder = new System.Text.StringBuilder();
            sqlBuilder.Append(@"
                SELECT DISTINCT 
                    p.ProgramId, p.ProgramGuid, p.ProgramCode, p.ParentId, p.IconCode, p.ProgramName, 
                    p.TopLevelProgramId, p.RootLevel, p.RowIndex, p.Note, p.IsParentProgram, p.IsProgram, 
                    p.IsBeginGroup, p.ProgramType, p.IsMenuAddVisible, p.IsMenuHomeVisible, p.IsActive, 
                    p.StatusId, p.RowVersion, p.CreatedById, p.CreatedTime, p.UpdatedById, p.UpdatedTime, 
                    p.DeletedById, p.DeletedTime, p.NavigateUrl, p.HelpLinkId, p.AssemblyName, 
                    p.AssemblyClassName, p.StoreProcedureName, p.AccessKey, p.IsVisible,
                    p.IsProgramDirectVoid,
                    p.IsProgramViewAble,
                    p.IsProgramAddAble,
                    p.IsProgramEditAble,
                    p.IsProgramDeleteAble,
                    p.IsProgramApprovalAble,
                    p.IsProgramUnApprovalAble,
                    p.IsProgramVoidAble,
                    p.IsProgramUnVoidAble,
                    p.IsProgramPrintAble
                FROM [core].[Programs] p
                WHERE p.ProgramType = 'MNU'
                  AND p.StatusId > 0 AND p.DeletedTime IS NULL

                UNION

                SELECT DISTINCT 
                    p.ProgramId, p.ProgramGuid, p.ProgramCode, p.ParentId, p.IconCode, p.ProgramName, 
                    p.TopLevelProgramId, p.RootLevel, p.RowIndex, p.Note, p.IsParentProgram, p.IsProgram, 
                    p.IsBeginGroup, p.ProgramType, p.IsMenuAddVisible, p.IsMenuHomeVisible, p.IsActive, 
                    p.StatusId, p.RowVersion, p.CreatedById, p.CreatedTime, p.UpdatedById, p.UpdatedTime, 
                    p.DeletedById, p.DeletedTime, p.NavigateUrl, p.HelpLinkId, p.AssemblyName, 
                    p.AssemblyClassName, p.StoreProcedureName, p.AccessKey, p.IsVisible,
                    p.IsProgramDirectVoid,
                    ugp.IsUserGroupViewAble AS IsProgramViewAble,
                    ugp.IsUserGroupAddAble AS IsProgramAddAble,
                    ugp.IsUserGroupEditAble AS IsProgramEditAble,
                    ugp.IsUserGroupDeleteAble AS IsProgramDeleteAble,
                    ugp.IsUserGroupApprovalAble AS IsProgramApprovalAble,
                    ugp.IsUserGroupUnApprovalAble AS IsProgramUnApprovalAble,
                    ugp.IsUserGroupVoidAble AS IsProgramVoidAble,
                    ugp.IsUserGroupUnVoidAble AS IsProgramUnVoidAble,
                    ugp.IsUserGroupExportAble AS IsProgramPrintAble
                FROM [core].[Programs] p
                INNER JOIN [core].[UserGroupProgram] ugp ON ugp.ProgramsId = p.ProgramId
                INNER JOIN [core].[UserGroupScope] ugs ON ugs.UserGroupId = ugp.UserGroupId
                INNER JOIN [core].[Users] u ON u.UserId = ugs.UserId
                WHERE u.UserGuid = @UserGuid
                  AND ugs.StatusId > 0 AND ugs.DeletedTime IS NULL
                  AND ugp.StatusId > 0 AND ugp.DeletedTime IS NULL
                  AND p.StatusId > 0 AND p.DeletedTime IS NULL
                  AND ugp.IsUserGroupViewAble = 1");

            if (!string.IsNullOrEmpty(companyId))
            {
                if (Guid.TryParse(companyId, out var companyGuid))
                {
                    sqlBuilder.Append(" AND ugs.CompanyGuid = @CompanyGuid");
                    parameters.Add("CompanyGuid", companyGuid);
                }
                else if (long.TryParse(companyId, out var companyIdLong))
                {
                    sqlBuilder.Append(" AND ugs.CompanyId = @CompanyIdLong");
                    parameters.Add("CompanyIdLong", companyIdLong);
                }
            }

            if (!string.IsNullOrEmpty(officeId))
            {
                if (Guid.TryParse(officeId, out var officeGuid))
                {
                    sqlBuilder.Append(" AND (ugs.CompanyOfficeGuid = @OfficeGuid OR ugs.CompanyOfficeGuid IS NULL)");
                    parameters.Add("OfficeGuid", officeGuid);
                }
                else if (long.TryParse(officeId, out var officeIdLong))
                {
                    sqlBuilder.Append(" AND (ugs.CompanyOfficeId = @OfficeIdLong OR ugs.CompanyOfficeId IS NULL)");
                    parameters.Add("OfficeIdLong", officeIdLong);
                }
            }
            else
            {
                sqlBuilder.Append(" AND ugs.CompanyOfficeId IS NULL");
            }

            sqlBuilder.Append(" ORDER BY p.ProgramCode, p.ProgramId ASC");

            return await connection.QueryAsync<Programs>(sqlBuilder.ToString(), parameters, transaction);
        }
    }
}
