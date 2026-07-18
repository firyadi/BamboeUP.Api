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
    public partial class PersonPhysicalCharacteristicRepository(RepositoryContext context, IAuditService audit) : IPersonPhysicalCharacteristicRepository
    {

        public async Task<PersonPhysicalCharacteristic?> GetPersonPhysicalCharacteristicAsync(Guid personPhysicalCharacteristicGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonPhysicalCharacteristic] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE a.PersonPhysicalCharacteristicGuid = @personPhysicalCharacteristicGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonPhysicalCharacteristic>(sql, new { personPhysicalCharacteristicGuid });
        }

        public async Task<PersonPhysicalCharacteristic?> GetByPersonGuidAndPersonPhysicalCharacteristicGuidAsync(Guid personGuid, Guid personPhysicalCharacteristicGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonPhysicalCharacteristic] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonPhysicalCharacteristicGuid = @personPhysicalCharacteristicGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonPhysicalCharacteristic>(sql, new { personGuid, personPhysicalCharacteristicGuid });
        }

        public async Task<IEnumerable<PersonPhysicalCharacteristic>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonPhysicalCharacteristic] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonPhysicalCharacteristicId DESC";
            return await connection.QueryAsync<PersonPhysicalCharacteristic>(sql, new { personGuid });
        }

        public async Task CreatePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personPhysicalCharacteristic);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[PersonPhysicalCharacteristic]
                (PersonPhysicalCharacteristicGuid, PersonId, CreatedById, StatusId, CreatedTime, SrPhysicalCharacteristic, PhysicalValue, SrMeasurementUnit, RecordedDate, Remarks)
                VALUES
                (@PersonPhysicalCharacteristicGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @SrPhysicalCharacteristic, @PhysicalValue, @SrMeasurementUnit, @RecordedDate, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            personPhysicalCharacteristic.PersonPhysicalCharacteristicId = await conn.QuerySingleAsync<long>(sql, personPhysicalCharacteristic, transaction);
        }

        public async Task UpdatePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personPhysicalCharacteristic);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonPhysicalCharacteristic]
                SET                     PersonId = @PersonId,
                    SrPhysicalCharacteristic = @SrPhysicalCharacteristic,
                    PhysicalValue = @PhysicalValue,
                    SrMeasurementUnit = @SrMeasurementUnit,
                    RecordedDate = @RecordedDate,
                    Remarks = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonPhysicalCharacteristicGuid = @PersonPhysicalCharacteristicGuid";
            await conn.ExecuteAsync(sql, personPhysicalCharacteristic, transaction);
        }

        public async Task SoftDeletePersonPhysicalCharacteristicAsync(PersonPhysicalCharacteristic personPhysicalCharacteristic, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personPhysicalCharacteristic, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[PersonPhysicalCharacteristic]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonPhysicalCharacteristicGuid = @PersonPhysicalCharacteristicGuid";

            await conn.ExecuteAsync(sql, personPhysicalCharacteristic, transaction);
        }

        public async Task DeletePersonPhysicalCharacteristicAsync(Guid personPhysicalCharacteristicGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonPhysicalCharacteristic] WHERE PersonPhysicalCharacteristicGuid = @personPhysicalCharacteristicGuid";
            await conn.ExecuteAsync(sql, new { personPhysicalCharacteristicGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[PersonPhysicalCharacteristic] WHERE PersonId IN (SELECT PersonId FROM [app].[Person] WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonPhysicalCharacteristic>> SearchPersonPhysicalCharacteristicAsync(
            string? srPhysicalCharacteristic,
            string? srPhysicalCharacteristicSearchType,
            string? physicalValue,
            string? physicalValueSearchType,
            string? srMeasurementUnit,
            string? srMeasurementUnitSearchType,
            string? recordedDate,
            string? recordedDateSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid personGuid, Guid personPhysicalCharacteristicGuid,
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
            if (personPhysicalCharacteristicGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonPhysicalCharacteristicGuid = @personPhysicalCharacteristicGuid");
                parameters.Add("@personPhysicalCharacteristicGuid", personPhysicalCharacteristicGuid);
            }

            if (!string.IsNullOrWhiteSpace(srPhysicalCharacteristic))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrPhysicalCharacteristic", "@srPhysicalCharacteristic", srPhysicalCharacteristicSearchType, parameters, "srPhysicalCharacteristic", srPhysicalCharacteristic);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(physicalValue))
            {
                var param = SqlFilterHelper.BuildFilter("a.PhysicalValue", "@physicalValue", physicalValueSearchType, parameters, "physicalValue", physicalValue);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(srMeasurementUnit))
            {
                var param = SqlFilterHelper.BuildFilter("a.SrMeasurementUnit", "@srMeasurementUnit", srMeasurementUnitSearchType, parameters, "srMeasurementUnit", srMeasurementUnit);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(recordedDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.RecordedDate", "@recordedDate", recordedDateSearchType, parameters, "recordedDate", recordedDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.FirstName AS PersonName

                FROM [app].[PersonPhysicalCharacteristic] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonPhysicalCharacteristicId DESC";

            return await connection.QueryAsync<PersonPhysicalCharacteristic>(sql, parameters, transaction);
        }
    }
}
