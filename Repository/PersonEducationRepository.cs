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
    public partial class PersonEducationRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IPersonEducationRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonEducation");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonEducation?> GetPersonEducationAsync(Guid personEducationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}

                WHERE a.PersonEducationGuid = @personEducationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEducation>(sql, new { personEducationGuid });
        }

        public async Task<PersonEducation?> GetByPersonGuidAndPersonEducationGuidAsync(Guid personGuid, Guid personEducationGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonEducationGuid = @personEducationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonEducation>(sql, new { personGuid, personEducationGuid });
        }

        public async Task<IEnumerable<PersonEducation>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonEducationId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonEducation>(sql, new { personGuid });
        }

        public async Task CreatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personEducation);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonEducationGuid, PersonId, CreatedById, StatusId, CreatedTime, [SrEducationLevel], [InstitutionName])
                VALUES
                (@PersonEducationGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrEducationLevel, @InstitutionName){dialect.ReturnIdentitySuffix("PersonEducationId")}";
            personEducation.PersonEducationId = await conn.QuerySingleAsync<long>(sql, personEducation, transaction);
        }

        public async Task UpdatePersonEducationAsync(PersonEducation personEducation, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personEducation);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [SrEducationLevel] = @SrEducationLevel,
                    [InstitutionName] = @InstitutionName,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonEducationGuid = @PersonEducationGuid";
            await conn.ExecuteAsync(sql, personEducation, transaction);
        }

        public async Task SoftDeletePersonEducationAsync(PersonEducation personEducation, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personEducation, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonEducationGuid = @PersonEducationGuid";

            await conn.ExecuteAsync(sql, personEducation, transaction);
        }

        public async Task DeletePersonEducationAsync(Guid personEducationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonEducationGuid = @personEducationGuid";
            await conn.ExecuteAsync(sql, new { personEducationGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonEducation>> SearchPersonEducationAsync(
            string? institutionName,
            string? institutionNameSearchType,
            Guid personGuid, Guid personEducationGuid,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (personGuid != Guid.Empty)
            {
                whereClauses.Add("j_PersonId.PersonGuid = @personGuid");
                parameters.Add("@personGuid", personGuid);
            }
            if (personEducationGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonEducationGuid = @personEducationGuid");
                parameters.Add("@personEducationGuid", personEducationGuid);
            }

            if (!string.IsNullOrWhiteSpace(institutionName))
            {
                var param = SqlFilterHelper.BuildFilter("a.InstitutionName", "@institutionName", institutionNameSearchType, parameters, "institutionName", institutionName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEducationLevel.StandardReferenceItemName")} AS EducationLevelName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEducationLevel", "SrEducationLevel", "EducationLevel")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonEducationId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonEducation>(sql, parameters, transaction);
        }
    }
}
