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
    public partial class EmployeeRepository(RepositoryContext context, IAuditService audit) : IEmployeeRepository
    {

        public async Task<Employee?> GetEmployeeAsync(Guid employeeGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName
, ISNULL(v_SrRemunerationType.StandardReferenceItemName, '') AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM [app].[Employee] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'RemunerationType')
    WHERE StandardReferenceItemId = a.SrRemunerationType
) v_SrRemunerationType

                WHERE a.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { employeeGuid });
        }

        public async Task<Employee?> GetEmployeeByIdAsync(long employeeId, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName
, ISNULL(v_SrRemunerationType.StandardReferenceItemName, '') AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM [app].[Employee] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'RemunerationType')
    WHERE StandardReferenceItemId = a.SrRemunerationType
) v_SrRemunerationType

                WHERE a.EmployeeId = @employeeId
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { employeeId });
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName
, ISNULL(v_SrRemunerationType.StandardReferenceItemName, '') AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM [app].[Employee] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'RemunerationType')
    WHERE StandardReferenceItemId = a.SrRemunerationType
) v_SrRemunerationType

                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeId DESC";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task CreateEmployeeAsync(Employee employee, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employee);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[Employee]
                (EmployeeGuid, CreatedById, StatusId, CreatedTime, PersonId, EmployeeNo, HireDate, ProbationEndDate, ConfirmationDate, TerminationDate, SrEmploymentStatus, SrEmploymentType, SrRemunerationType)
                VALUES
                (@EmployeeGuid, @CreatedById, @StatusId, @CreatedTime, @PersonId, @EmployeeNo, @HireDate, @ProbationEndDate, @ConfirmationDate, @TerminationDate, @SrEmploymentStatus, @SrEmploymentType, @SrRemunerationType);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employee.EmployeeId = await conn.QuerySingleAsync<long>(sql, employee, transaction);
        }

        public async Task UpdateEmployeeAsync(Employee employee, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employee);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[Employee]
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
            const string sql = @"
                UPDATE [app].[Employee]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeGuid = @EmployeeGuid";

            await conn.ExecuteAsync(sql, employee, transaction);
        }

        public async Task DeleteEmployeeAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid";
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
                var param = SqlFilterHelper.BuildFilter("a.EmployeeNo", "@employeeNo", employeeNoSearchType, parameters, "employeeNo", employeeNo);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(hireDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.HireDate", "@hireDate", hireDateSearchType, parameters, "hireDate", hireDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(probationEndDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ProbationEndDate", "@probationEndDate", probationEndDateSearchType, parameters, "probationEndDate", probationEndDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(confirmationDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.ConfirmationDate", "@confirmationDate", confirmationDateSearchType, parameters, "confirmationDate", confirmationDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(terminationDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.TerminationDate", "@terminationDate", terminationDateSearchType, parameters, "terminationDate", terminationDate);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }



            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_PersonId.PersonName AS PersonName
, ISNULL(v_SrEmploymentStatus.StandardReferenceItemName, '') AS EmploymentStatusName
, ISNULL(v_SrEmploymentType.StandardReferenceItemName, '') AS EmploymentTypeName
, ISNULL(v_SrRemunerationType.StandardReferenceItemName, '') AS RemunerationTypeName

, j_PersonId.FileStorageId AS PersonFileStorageId
, j_PersonId.BirthDate AS BirthDate

                FROM [app].[Employee] a
LEFT JOIN [app].[Person] j_PersonId ON a.PersonId = j_PersonId.PersonId
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentStatus')
    WHERE StandardReferenceItemId = a.SrEmploymentStatus
) v_SrEmploymentStatus
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'EmploymentType')
    WHERE StandardReferenceItemId = a.SrEmploymentType
) v_SrEmploymentType
OUTER APPLY (
    SELECT StandardReferenceItemName
    FROM [app].[fn_GetStandardReferenceItems](NULL, NULL, NULL, 'RemunerationType')
    WHERE StandardReferenceItemId = a.SrRemunerationType
) v_SrRemunerationType

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeId DESC";

            return await connection.QueryAsync<Employee>(sql, parameters, transaction);
        }
    }
}
