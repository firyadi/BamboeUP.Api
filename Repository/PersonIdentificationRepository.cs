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
    public partial class PersonIdentificationRepository(RepositoryContext context, ISqlDialect dialect) : IPersonIdentificationRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonIdentification");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonIdentification?> GetPersonIdentificationAsync(Guid personIdentificationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.FirstName AS FirstName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrIdentificationType.StandardReferenceItemName")} AS IdentificationTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrIdentificationType", "SrIdentificationType", "IdentificationType")}

                WHERE a.PersonIdentificationGuid = @personIdentificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonIdentification>(sql, new { personIdentificationGuid });
        }

        public async Task<PersonIdentification?> GetByPersonGuidAndPersonIdentificationGuidAsync(Guid personGuid, Guid personIdentificationGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.FirstName AS FirstName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrIdentificationType.StandardReferenceItemName")} AS IdentificationTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrIdentificationType", "SrIdentificationType", "IdentificationType")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonIdentificationGuid = @personIdentificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonIdentification>(sql, new { personGuid, personIdentificationGuid });
        }

        public async Task<IEnumerable<PersonIdentification>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.FirstName AS FirstName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrIdentificationType.StandardReferenceItemName")} AS IdentificationTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrIdentificationType", "SrIdentificationType", "IdentificationType")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonIdentificationId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonIdentification>(sql, new { personGuid });
        }

        public async Task CreatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personIdentification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonIdentificationGuid, PersonId, CreatedById, StatusId, CreatedTime, [SrIdentificationType], [IdentificationValue])
                VALUES
                (@PersonIdentificationGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrIdentificationType, @IdentificationValue){dialect.ReturnIdentitySuffix("PersonIdentificationId")}";
            personIdentification.PersonIdentificationId = await conn.QuerySingleAsync<long>(sql, personIdentification, transaction);
        }

        public async Task UpdatePersonIdentificationAsync(PersonIdentification personIdentification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personIdentification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [SrIdentificationType] = @SrIdentificationType,
                    [IdentificationValue] = @IdentificationValue,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonIdentificationGuid = @PersonIdentificationGuid";
            await conn.ExecuteAsync(sql, personIdentification, transaction);
        }

        public async Task SoftDeletePersonIdentificationAsync(PersonIdentification personIdentification, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personIdentification, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonIdentificationGuid = @PersonIdentificationGuid";

            await conn.ExecuteAsync(sql, personIdentification, transaction);
        }

        public async Task DeletePersonIdentificationAsync(Guid personIdentificationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonIdentificationGuid = @personIdentificationGuid";
            await conn.ExecuteAsync(sql, new { personIdentificationGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonIdentification>> SearchPersonIdentificationAsync(
            string? identificationValue,
            string? identificationValueSearchType,
            Guid personGuid, Guid personIdentificationGuid,
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
            if (personIdentificationGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonIdentificationGuid = @personIdentificationGuid");
                parameters.Add("@personIdentificationGuid", personIdentificationGuid);
            }

            if (!string.IsNullOrWhiteSpace(identificationValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.IdentificationValue", "@identificationValue", identificationValueSearchType, parameters, "identificationValue", identificationValue, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.FirstName AS FirstName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrIdentificationType.StandardReferenceItemName")} AS IdentificationTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrIdentificationType", "SrIdentificationType", "IdentificationType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonIdentificationId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonIdentification>(sql, parameters, transaction);
        }
    }
}
