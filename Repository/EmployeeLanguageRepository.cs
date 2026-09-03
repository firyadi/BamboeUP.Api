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
    public partial class EmployeeLanguageRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IEmployeeLanguageRepository
    {
        private readonly string _table = dialect.Qualify("app", "EmployeeLanguage");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<EmployeeLanguage?> GetEmployeeLanguageAsync(Guid employeeLanguageGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrLanguage.StandardReferenceItemName")} AS LanguageName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrListeningLevel.StandardReferenceItemName")} AS ListeningLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReadingLevel.StandardReferenceItemName")} AS ReadingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSpeakingLevel.StandardReferenceItemName")} AS SpeakingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrWritingLevel.StandardReferenceItemName")} AS WritingLevelName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrLanguage", "SrLanguage", "Language")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrListeningLevel", "SrListeningLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReadingLevel", "SrReadingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSpeakingLevel", "SrSpeakingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrWritingLevel", "SrWritingLevel", "LanguageLevel")}

                WHERE a.EmployeeLanguageGuid = @employeeLanguageGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeLanguage>(sql, new { employeeLanguageGuid });
        }

        public async Task<EmployeeLanguage?> GetByEmployeeGuidAndEmployeeLanguageGuidAsync(Guid employeeGuid, Guid employeeLanguageGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrLanguage.StandardReferenceItemName")} AS LanguageName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrListeningLevel.StandardReferenceItemName")} AS ListeningLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReadingLevel.StandardReferenceItemName")} AS ReadingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSpeakingLevel.StandardReferenceItemName")} AS SpeakingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrWritingLevel.StandardReferenceItemName")} AS WritingLevelName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrLanguage", "SrLanguage", "Language")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrListeningLevel", "SrListeningLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReadingLevel", "SrReadingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSpeakingLevel", "SrSpeakingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrWritingLevel", "SrWritingLevel", "LanguageLevel")}

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
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrLanguage.StandardReferenceItemName")} AS LanguageName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrListeningLevel.StandardReferenceItemName")} AS ListeningLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReadingLevel.StandardReferenceItemName")} AS ReadingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSpeakingLevel.StandardReferenceItemName")} AS SpeakingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrWritingLevel.StandardReferenceItemName")} AS WritingLevelName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrLanguage", "SrLanguage", "Language")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrListeningLevel", "SrListeningLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReadingLevel", "SrReadingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSpeakingLevel", "SrSpeakingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrWritingLevel", "SrWritingLevel", "LanguageLevel")}

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeLanguageId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<EmployeeLanguage>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeLanguage);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (EmployeeLanguageGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [SrLanguage], [SrReadingLevel], [SrWritingLevel], [SrListeningLevel], [SrSpeakingLevel], [CertificationName], [Score], [IssuedDate], [ExpiredDate], [FileStorageId], [IsPrimary], [Remarks])
                VALUES
                (@EmployeeLanguageGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @SrLanguage, @SrReadingLevel, @SrWritingLevel, @SrListeningLevel, @SrSpeakingLevel, @CertificationName, @Score, @IssuedDate, @ExpiredDate, @FileStorageId, @IsPrimary, @Remarks){dialect.ReturnIdentitySuffix("EmployeeLanguageId")}";
            employeeLanguage.EmployeeLanguageId = await conn.QuerySingleAsync<long>(sql, employeeLanguage, transaction);
        }

        public async Task UpdateEmployeeLanguageAsync(EmployeeLanguage employeeLanguage, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeLanguage);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
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
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeLanguageGuid = @EmployeeLanguageGuid";

            await conn.ExecuteAsync(sql, employeeLanguage, transaction);
        }

        public async Task DeleteEmployeeLanguageAsync(Guid employeeLanguageGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeLanguageGuid = @employeeLanguageGuid";
            await conn.ExecuteAsync(sql, new { employeeLanguageGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeId IN (SELECT EmployeeId FROM {dialect.Qualify("app", "Employee")} WHERE EmployeeGuid = @employeeGuid)";
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
                var param = SqlFilterHelper.BuildFilter("a.CertificationName", "@certificationName", certificationNameSearchType, parameters, "certificationName", certificationName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(score))
            {
                var param = SqlFilterHelper.BuildFilter("a.Score", "@score", scoreSearchType, parameters, "score", score, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(issuedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.IssuedDate", "@issuedDate", issuedDateSearchType, parameters, "issuedDate", issuedDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(expiredDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ExpiredDate", "@expiredDate", expiredDateSearchType, parameters, "expiredDate", expiredDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrLanguage.StandardReferenceItemName")} AS LanguageName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrListeningLevel.StandardReferenceItemName")} AS ListeningLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrReadingLevel.StandardReferenceItemName")} AS ReadingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrSpeakingLevel.StandardReferenceItemName")} AS SpeakingLevelName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrWritingLevel.StandardReferenceItemName")} AS WritingLevelName


                FROM {_table} a
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrLanguage", "SrLanguage", "Language")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrListeningLevel", "SrListeningLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrReadingLevel", "SrReadingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrSpeakingLevel", "SrSpeakingLevel", "LanguageLevel")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrWritingLevel", "SrWritingLevel", "LanguageLevel")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeLanguageId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<EmployeeLanguage>(sql, parameters, transaction);
        }
    }
}
