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
    public partial class EmployeeSkillRepository(RepositoryContext context, IAuditService audit) : IEmployeeSkillRepository
    {

        public async Task<EmployeeSkill?> GetEmployeeSkillAsync(Guid employeeSkillGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_SkillId.SkillName AS SkillName
, ISNULL(v_SrSkillLevel.StandardReferenceItemName, '') AS SkillLevelName


                FROM [app].[EmployeeSkill] a
LEFT JOIN [app].[Skill] j_SkillId ON a.SkillId = j_SkillId.SkillId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSkillLevel
) v_SrSkillLevel

                WHERE a.EmployeeSkillGuid = @employeeSkillGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeSkill>(sql, new { employeeSkillGuid });
        }

        public async Task<EmployeeSkill?> GetByEmployeeGuidAndEmployeeSkillGuidAsync(Guid employeeGuid, Guid employeeSkillGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_SkillId.SkillName AS SkillName
, ISNULL(v_SrSkillLevel.StandardReferenceItemName, '') AS SkillLevelName


                FROM [app].[EmployeeSkill] a
LEFT JOIN [app].[Skill] j_SkillId ON a.SkillId = j_SkillId.SkillId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSkillLevel
) v_SrSkillLevel

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeSkillGuid = @employeeSkillGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeSkill>(sql, new { employeeGuid, employeeSkillGuid });
        }

        public async Task<IEnumerable<EmployeeSkill>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_SkillId.SkillName AS SkillName
, ISNULL(v_SrSkillLevel.StandardReferenceItemName, '') AS SkillLevelName


                FROM [app].[EmployeeSkill] a
LEFT JOIN [app].[Skill] j_SkillId ON a.SkillId = j_SkillId.SkillId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSkillLevel
) v_SrSkillLevel

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeSkillId DESC";
            return await connection.QueryAsync<EmployeeSkill>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeSkillAsync(EmployeeSkill employeeSkill, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeSkill);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeSkill]
                (EmployeeSkillGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [SkillId], [SrSkillLevel], [YearsofExperience], [LastUsedDate], [IsPrimary], [IsCertified], [CertificateNumber], [CertificateIssueDate], [CertificateExpiredDate], [FileStorageId], [Remarks])
                VALUES
                (@EmployeeSkillGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @SkillId, @SrSkillLevel, @YearsofExperience, @LastUsedDate, @IsPrimary, @IsCertified, @CertificateNumber, @CertificateIssueDate, @CertificateExpiredDate, @FileStorageId, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeSkill.EmployeeSkillId = await conn.QuerySingleAsync<long>(sql, employeeSkill, transaction);
        }

        public async Task UpdateEmployeeSkillAsync(EmployeeSkill employeeSkill, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeSkill);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeSkill]
                SET                     [EmployeeId] = @EmployeeId,
                    [SkillId] = @SkillId,
                    [SrSkillLevel] = @SrSkillLevel,
                    [YearsofExperience] = @YearsofExperience,
                    [LastUsedDate] = @LastUsedDate,
                    [IsPrimary] = @IsPrimary,
                    [IsCertified] = @IsCertified,
                    [CertificateNumber] = @CertificateNumber,
                    [CertificateIssueDate] = @CertificateIssueDate,
                    [CertificateExpiredDate] = @CertificateExpiredDate,
                    [FileStorageId] = @FileStorageId,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeSkillGuid = @EmployeeSkillGuid";
            await conn.ExecuteAsync(sql, employeeSkill, transaction);
        }

        public async Task SoftDeleteEmployeeSkillAsync(EmployeeSkill employeeSkill, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeSkill, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeSkill]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeSkillGuid = @EmployeeSkillGuid";

            await conn.ExecuteAsync(sql, employeeSkill, transaction);
        }

        public async Task DeleteEmployeeSkillAsync(Guid employeeSkillGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeSkill] WHERE EmployeeSkillGuid = @employeeSkillGuid";
            await conn.ExecuteAsync(sql, new { employeeSkillGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeSkill] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeSkill>> SearchEmployeeSkillAsync(
            string? yearsofExperience,
            string? yearsofExperienceSearchType,
            string? lastUsedDate,
            string? lastUsedDateSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? isCertified,
            string? isCertifiedSearchType,
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? certificateIssueDate,
            string? certificateIssueDateSearchType,
            string? certificateExpiredDate,
            string? certificateExpiredDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeSkillGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (employeeGuid != Guid.Empty)
            {
                whereClauses.Add("j_EmployeeId.EmployeeGuid = @employeeGuid");
                parameters.Add("@employeeGuid", employeeGuid);
            }
            if (employeeSkillGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeSkillGuid = @employeeSkillGuid");
                parameters.Add("@employeeSkillGuid", employeeSkillGuid);
            }

            if (!string.IsNullOrWhiteSpace(yearsofExperience))
            {
                var param = SqlFilterHelper.BuildFilter("a.YearsofExperience", "@yearsofExperience", yearsofExperienceSearchType, parameters, "yearsofExperience", yearsofExperience);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(lastUsedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.LastUsedDate", "@lastUsedDate", lastUsedDateSearchType, parameters, "lastUsedDate", lastUsedDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCertified))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCertified", "@isCertified", isCertifiedSearchType, parameters, "isCertified", isCertified);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificateNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificateNumber", "@certificateNumber", certificateNumberSearchType, parameters, "certificateNumber", certificateNumber);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificateIssueDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificateIssueDate", "@certificateIssueDate", certificateIssueDateSearchType, parameters, "certificateIssueDate", certificateIssueDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(certificateExpiredDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificateExpiredDate", "@certificateExpiredDate", certificateExpiredDateSearchType, parameters, "certificateExpiredDate", certificateExpiredDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_SkillId.SkillName AS SkillName
, ISNULL(v_SrSkillLevel.StandardReferenceItemName, '') AS SkillLevelName


                FROM [app].[EmployeeSkill] a
LEFT JOIN [app].[Skill] j_SkillId ON a.SkillId = j_SkillId.SkillId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSkillLevel
) v_SrSkillLevel

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeSkillId DESC";

            return await connection.QueryAsync<EmployeeSkill>(sql, parameters, transaction);
        }
    }
}
