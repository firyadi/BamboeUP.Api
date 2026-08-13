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
    public partial class EmployeeLanguageRepository(RepositoryContext context, IAuditService audit) : IEmployeeLanguageRepository
    {

        public async Task<EmployeeLanguage?> GetEmployeeLanguageAsync(Guid employeeLanguageGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrLanguage.StandardReferenceItemName, '') AS LanguageName
, ISNULL(v_SrListeningLevel.StandardReferenceItemName, '') AS ListeningLevelName
, ISNULL(v_SrReadingLevel.StandardReferenceItemName, '') AS ReadingLevelName
, ISNULL(v_SrSpeakingLevel.StandardReferenceItemName, '') AS SpeakingLevelName
, ISNULL(v_SrWritingLevel.StandardReferenceItemName, '') AS WritingLevelName


                FROM [app].[EmployeeLanguage] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Language')
    WHERE StandardReferenceItemId = a.SrLanguage
) v_SrLanguage
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrListeningLevel
) v_SrListeningLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrReadingLevel
) v_SrReadingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSpeakingLevel
) v_SrSpeakingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrWritingLevel
) v_SrWritingLevel

                WHERE a.EmployeeLanguageGuid = @employeeLanguageGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeLanguage>(sql, new { employeeLanguageGuid });
        }

        public async Task<EmployeeLanguage?> GetByEmployeeGuidAndEmployeeLanguageGuidAsync(Guid employeeGuid, Guid employeeLanguageGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrLanguage.StandardReferenceItemName, '') AS LanguageName
, ISNULL(v_SrListeningLevel.StandardReferenceItemName, '') AS ListeningLevelName
, ISNULL(v_SrReadingLevel.StandardReferenceItemName, '') AS ReadingLevelName
, ISNULL(v_SrSpeakingLevel.StandardReferenceItemName, '') AS SpeakingLevelName
, ISNULL(v_SrWritingLevel.StandardReferenceItemName, '') AS WritingLevelName


                FROM [app].[EmployeeLanguage] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Language')
    WHERE StandardReferenceItemId = a.SrLanguage
) v_SrLanguage
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrListeningLevel
) v_SrListeningLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrReadingLevel
) v_SrReadingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSpeakingLevel
) v_SrSpeakingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrWritingLevel
) v_SrWritingLevel

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeLanguageGuid = @employeeLanguageGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeLanguage>(sql, new { employeeGuid, employeeLanguageGuid });
        }

        public async Task<IEnumerable<EmployeeLanguage>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrLanguage.StandardReferenceItemName, '') AS LanguageName
, ISNULL(v_SrListeningLevel.StandardReferenceItemName, '') AS ListeningLevelName
, ISNULL(v_SrReadingLevel.StandardReferenceItemName, '') AS ReadingLevelName
, ISNULL(v_SrSpeakingLevel.StandardReferenceItemName, '') AS SpeakingLevelName
, ISNULL(v_SrWritingLevel.StandardReferenceItemName, '') AS WritingLevelName


                FROM [app].[EmployeeLanguage] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Language')
    WHERE StandardReferenceItemId = a.SrLanguage
) v_SrLanguage
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrListeningLevel
) v_SrListeningLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrReadingLevel
) v_SrReadingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSpeakingLevel
) v_SrSpeakingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrWritingLevel
) v_SrWritingLevel

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeLanguageId DESC";
            return await connection.QueryAsync<EmployeeLanguage>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeLanguage);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeLanguage]
                (EmployeeLanguageGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [SrLanguage], [SrReadingLevel], [SrWritingLevel], [SrListeningLevel], [SrSpeakingLevel], [CertificationName], [Score], [IssuedDate], [ExpiredDate], [FileStorageId], [IsPrimary], [Remarks])
                VALUES
                (@EmployeeLanguageGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @SrLanguage, @SrReadingLevel, @SrWritingLevel, @SrListeningLevel, @SrSpeakingLevel, @CertificationName, @Score, @IssuedDate, @ExpiredDate, @FileStorageId, @IsPrimary, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeLanguage.EmployeeLanguageId = await conn.QuerySingleAsync<long>(sql, employeeLanguage, transaction);
        }

        public async Task UpdateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeLanguage);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeLanguage]
                SET                     [EmployeeId] = @EmployeeId,
                    [SrLanguage] = @SrLanguage,
                    [SrReadingLevel] = @SrReadingLevel,
                    [SrWritingLevel] = @SrWritingLevel,
                    [SrListeningLevel] = @SrListeningLevel,
                    [SrSpeakingLevel] = @SrSpeakingLevel,
                    [CertificationName] = @CertificationName,
                    [Score] = @Score,
                    [IssuedDate] = @IssuedDate,
                    [ExpiredDate] = @ExpiredDate,
                    [FileStorageId] = @FileStorageId,
                    [IsPrimary] = @IsPrimary,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeLanguageGuid = @EmployeeLanguageGuid";
            await conn.ExecuteAsync(sql, employeeLanguage, transaction);
        }

        public async Task SoftDeleteEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeLanguage, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeLanguage]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeLanguageGuid = @EmployeeLanguageGuid";

            await conn.ExecuteAsync(sql, employeeLanguage, transaction);
        }

        public async Task DeleteEmployeeLanguageAsync(Guid employeeLanguageGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeLanguage] WHERE EmployeeLanguageGuid = @employeeLanguageGuid";
            await conn.ExecuteAsync(sql, new { employeeLanguageGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeLanguage] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeLanguage>> SearchEmployeeLanguageAsync(
            string? certificationName,
            string? certificationNameSearchType,
            string? score,
            string? scoreSearchType,
            string? issuedDate,
            string? issuedDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeLanguageGuid,
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
            if (employeeLanguageGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeLanguageGuid = @employeeLanguageGuid");
                parameters.Add("@employeeLanguageGuid", employeeLanguageGuid);
            }

            if (!string.IsNullOrWhiteSpace(certificationName))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificationName", "@certificationName", certificationNameSearchType, parameters, "certificationName", certificationName);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(score))
            {
                var param = SqlFilterHelper.BuildFilter("a.Score", "@score", scoreSearchType, parameters, "score", score);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(issuedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.IssuedDate", "@issuedDate", issuedDateSearchType, parameters, "issuedDate", issuedDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(expiredDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExpiredDate", "@expiredDate", expiredDateSearchType, parameters, "expiredDate", expiredDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, ISNULL(v_SrLanguage.StandardReferenceItemName, '') AS LanguageName
, ISNULL(v_SrListeningLevel.StandardReferenceItemName, '') AS ListeningLevelName
, ISNULL(v_SrReadingLevel.StandardReferenceItemName, '') AS ReadingLevelName
, ISNULL(v_SrSpeakingLevel.StandardReferenceItemName, '') AS SpeakingLevelName
, ISNULL(v_SrWritingLevel.StandardReferenceItemName, '') AS WritingLevelName


                FROM [app].[EmployeeLanguage] a
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'Language')
    WHERE StandardReferenceItemId = a.SrLanguage
) v_SrLanguage
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrListeningLevel
) v_SrListeningLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrReadingLevel
) v_SrReadingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrSpeakingLevel
) v_SrSpeakingLevel
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'LanguageLevel')
    WHERE StandardReferenceItemId = a.SrWritingLevel
) v_SrWritingLevel

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeLanguageId DESC";

            return await connection.QueryAsync<EmployeeLanguage>(sql, parameters, transaction);
        }
    }
}
