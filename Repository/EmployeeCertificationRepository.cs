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
    public partial class EmployeeCertificationRepository(RepositoryContext context, IAuditService audit) : IEmployeeCertificationRepository
    {

        public async Task<EmployeeCertification?> GetEmployeeCertificationAsync(Guid employeeCertificationGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CertificationId.CertificationName AS CertificationName


                FROM [app].[EmployeeCertification] a
LEFT JOIN [app].[Certification] j_CertificationId ON a.CertificationId = j_CertificationId.CertificationId

                WHERE a.EmployeeCertificationGuid = @employeeCertificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeCertification>(sql, new { employeeCertificationGuid });
        }

        public async Task<EmployeeCertification?> GetByEmployeeGuidAndEmployeeCertificationGuidAsync(Guid employeeGuid, Guid employeeCertificationGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CertificationId.CertificationName AS CertificationName


                FROM [app].[EmployeeCertification] a
LEFT JOIN [app].[Certification] j_CertificationId ON a.CertificationId = j_CertificationId.CertificationId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.EmployeeCertificationGuid = @employeeCertificationGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<EmployeeCertification>(sql, new { employeeGuid, employeeCertificationGuid });
        }

        public async Task<IEnumerable<EmployeeCertification>> GetAllByEmployeeGuidAsync(Guid employeeGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CertificationId.CertificationName AS CertificationName


                FROM [app].[EmployeeCertification] a
LEFT JOIN [app].[Certification] j_CertificationId ON a.CertificationId = j_CertificationId.CertificationId

                WHERE j_EmployeeId.EmployeeGuid = @employeeGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.EmployeeCertificationId DESC";
            return await connection.QueryAsync<EmployeeCertification>(sql, new { employeeGuid });
        }

        public async Task CreateEmployeeCertificationAsync(EmployeeCertification employeeCertification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(employeeCertification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                INSERT INTO [app].[EmployeeCertification]
                (EmployeeCertificationGuid, EmployeeId, CreatedById, StatusId, CreatedTime, [CertificationId], [CertificateNumber], [IssuedDate], [ExpiredDate], [Issuedby], [Score], [FileStorageId], [IsLifetime], [IsCurrent], [Remarks])
                VALUES
                (@EmployeeCertificationGuid, @EmployeeId, @CreatedById, @StatusId, @CreatedTime, @CertificationId, @CertificateNumber, @IssuedDate, @ExpiredDate, @Issuedby, @Score, @FileStorageId, @IsLifetime, @IsCurrent, @Remarks);
                SELECT CAST(SCOPE_IDENTITY() as bigint);";
            employeeCertification.EmployeeCertificationId = await conn.QuerySingleAsync<long>(sql, employeeCertification, transaction);
        }

        public async Task UpdateEmployeeCertificationAsync(EmployeeCertification employeeCertification, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(employeeCertification);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeCertification]
                SET                     [EmployeeId] = @EmployeeId,
                    [CertificationId] = @CertificationId,
                    [CertificateNumber] = @CertificateNumber,
                    [IssuedDate] = @IssuedDate,
                    [ExpiredDate] = @ExpiredDate,
                    [Issuedby] = @Issuedby,
                    [Score] = @Score,
                    [FileStorageId] = @FileStorageId,
                    [IsLifetime] = @IsLifetime,
                    [IsCurrent] = @IsCurrent,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE EmployeeCertificationGuid = @EmployeeCertificationGuid";
            await conn.ExecuteAsync(sql, employeeCertification, transaction);
        }

        public async Task SoftDeleteEmployeeCertificationAsync(EmployeeCertification employeeCertification, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(employeeCertification, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"
                UPDATE [app].[EmployeeCertification]
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE EmployeeCertificationGuid = @EmployeeCertificationGuid";

            await conn.ExecuteAsync(sql, employeeCertification, transaction);
        }

        public async Task DeleteEmployeeCertificationAsync(Guid employeeCertificationGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeCertification] WHERE EmployeeCertificationGuid = @employeeCertificationGuid";
            await conn.ExecuteAsync(sql, new { employeeCertificationGuid }, transaction);
        }

        public async Task DeleteByEmployeeGuidAsync(Guid employeeGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            const string sql = @"DELETE FROM [app].[EmployeeCertification] WHERE EmployeeId IN (SELECT EmployeeId FROM [app].[Employee] WHERE EmployeeGuid = @employeeGuid)";
            await conn.ExecuteAsync(sql, new { employeeGuid }, transaction);
        }

        public async Task<IEnumerable<EmployeeCertification>> SearchEmployeeCertificationAsync(
            string? certificateNumber,
            string? certificateNumberSearchType,
            string? issuedDate,
            string? issuedDateSearchType,
            string? expiredDate,
            string? expiredDateSearchType,
            string? issuedby,
            string? issuedbySearchType,
            string? score,
            string? scoreSearchType,
            string? isLifetime,
            string? isLifetimeSearchType,
            string? isCurrent,
            string? isCurrentSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid employeeGuid, Guid employeeCertificationGuid,
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
            if (employeeCertificationGuid != Guid.Empty)
            {
                whereClauses.Add("a.EmployeeCertificationGuid = @employeeCertificationGuid");
                parameters.Add("@employeeCertificationGuid", employeeCertificationGuid);
            }

            if (!string.IsNullOrWhiteSpace(certificateNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.CertificateNumber", "@certificateNumber", certificateNumberSearchType, parameters, "certificateNumber", certificateNumber);
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

            if (!string.IsNullOrWhiteSpace(issuedby))
            {
                var param = SqlFilterHelper.BuildFilter("a.Issuedby", "@issuedby", issuedbySearchType, parameters, "issuedby", issuedby);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(score))
            {
                var param = SqlFilterHelper.BuildFilter("a.Score", "@score", scoreSearchType, parameters, "score", score);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isLifetime))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsLifetime", "@isLifetime", isLifetimeSearchType, parameters, "isLifetime", isLifetime);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isCurrent))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsCurrent", "@isCurrent", isCurrentSearchType, parameters, "isCurrent", isCurrent);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                SELECT TOP ({Contracts.ParameterContext.MaxResultRecord}) a.*
, j_CertificationId.CertificationName AS CertificationName


                FROM [app].[EmployeeCertification] a
LEFT JOIN [app].[Certification] j_CertificationId ON a.CertificationId = j_CertificationId.CertificationId

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.EmployeeCertificationId DESC";

            return await connection.QueryAsync<EmployeeCertification>(sql, parameters, transaction);
        }
    }
}
