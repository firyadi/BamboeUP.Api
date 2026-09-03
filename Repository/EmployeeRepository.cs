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
    public partial class EmployeeRepository(RepositoryContext context, IAuditService audit, ISqlDialect dialect) : IEmployeeRepository
    {
        private readonly string _table = dialect.Qualify("app", "Employee");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<Employee?> GetEmployeeAsync(Guid employeeGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRemunerationType.StandardReferenceItemName")} AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRemunerationType", "SrRemunerationType", "RemunerationType")}

                WHERE a.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { employeeGuid });
        }

        public async Task<Employee?> GetEmployeeByIdAsync(long employeeId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRemunerationType.StandardReferenceItemName")} AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRemunerationType", "SrRemunerationType", "RemunerationType")}

                WHERE a.EmployeeId = @employeeId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { employeeId });
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRemunerationType.StandardReferenceItemName")} AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRemunerationType", "SrRemunerationType", "RemunerationType")}

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task CreateEmployeeAsync(Employee employee, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employee);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (EmployeeGuid, CreatedById, StatusId, CreatedTime, PersonId, EmployeeNo, HireDate, ProbationEndDate, ConfirmationDate, TerminationDate, SrEmploymentStatus, SrEmploymentType, SrRemunerationType)
                VALUES
                (@EmployeeGuid, @CreatedById, @StatusId, @CreatedTime, @PersonId, @EmployeeNo, @HireDate, @ProbationEndDate, @ConfirmationDate, @TerminationDate, @SrEmploymentStatus, @SrEmploymentType, @SrRemunerationType){dialect.ReturnIdentitySuffix("EmployeeId")}";
            employee.EmployeeId = await conn.QuerySingleAsync<long>(sql, employee, transaction);
        }

        public async Task UpdateEmployeeAsync(Employee employee, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employee);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     PersonId = @PersonId,
                    EmployeeNo = @EmployeeNo,
                    HireDate = @HireDate,
                    ProbationEndDate = @ProbationEndDate,
                    ConfirmationDate = @ConfirmationDate,
                    TerminationDate = @TerminationDate,
                    SrEmploymentStatus = @SrEmploymentStatus,
                    SrEmploymentType = @SrEmploymentType,
                    SrRemunerationType = @SrRemunerationType,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeGuid = @EmployeeGuid";
            await conn.ExecuteAsync(sql, employee, transaction);
        }

        public async Task SoftDeleteEmployeeAsync(Employee employee, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employee, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeGuid = @EmployeeGuid";

            await conn.ExecuteAsync(sql, employee, transaction);
        }

        public async Task DeleteEmployeeAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE EmployeeGuid = @employeeGuid";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<Employee>> SearchEmployeeAsync(
            string? employeeNo,
            string? employeeNoSearchType,
            string? hireDate,
            string? hireDateSearchType,
            string? probationEndDate,
            string? probationEndDateSearchType,
            string? confirmationDate,
            string? confirmationDateSearchType,
            string? terminationDate,
            string? terminationDateSearchType,
            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(employeeNo))
            {
                var param = SqlFilterHelper.BuildFilter("a.EmployeeNo", "@employeeNo", employeeNoSearchType, parameters, "employeeNo", employeeNo, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hireDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.HireDate", "@hireDate", hireDateSearchType, parameters, "hireDate", hireDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(probationEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProbationEndDate", "@probationEndDate", probationEndDateSearchType, parameters, "probationEndDate", probationEndDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(confirmationDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ConfirmationDate", "@confirmationDate", confirmationDateSearchType, parameters, "confirmationDate", confirmationDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(terminationDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.TerminationDate", "@terminationDate", terminationDateSearchType, parameters, "terminationDate", terminationDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentStatus.StandardReferenceItemName")} AS EmploymentStatusName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrEmploymentType.StandardReferenceItemName")} AS EmploymentTypeName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrRemunerationType.StandardReferenceItemName")} AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentStatus", "SrEmploymentStatus", "EmploymentStatus")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrEmploymentType", "SrEmploymentType", "EmploymentType")}
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrRemunerationType", "SrRemunerationType", "RemunerationType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Employee>(sql, parameters, transaction);
        }
    }
}
