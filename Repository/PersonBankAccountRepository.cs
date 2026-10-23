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
    public partial class PersonBankAccountRepository(RepositoryContext context, ISqlDialect dialect) : IPersonBankAccountRepository
    {
        private readonly string _table = dialect.Qualify("app", "PersonBankAccount");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

public async Task<PersonBankAccount?> GetPersonBankAccountAsync(Guid personBankAccountGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_BankId.BankName AS BankName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBankAccountType.StandardReferenceItemName")} AS BankAccountTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Bank")} j_BankId ON a.BankId = j_BankId.BankId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBankAccountType", "SrBankAccountType", "BankAccountType")}

                WHERE a.PersonBankAccountGuid = @personBankAccountGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonBankAccount>(sql, new { personBankAccountGuid });
        }

        public async Task<PersonBankAccount?> GetByPersonGuidAndPersonBankAccountGuidAsync(Guid personGuid, Guid personBankAccountGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_BankId.BankName AS BankName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBankAccountType.StandardReferenceItemName")} AS BankAccountTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Bank")} j_BankId ON a.BankId = j_BankId.BankId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBankAccountType", "SrBankAccountType", "BankAccountType")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.PersonBankAccountGuid = @personBankAccountGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<PersonBankAccount>(sql, new { personGuid, personBankAccountGuid });
        }

        public async Task<IEnumerable<PersonBankAccount>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_BankId.BankName AS BankName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBankAccountType.StandardReferenceItemName")} AS BankAccountTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Bank")} j_BankId ON a.BankId = j_BankId.BankId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBankAccountType", "SrBankAccountType", "BankAccountType")}

                WHERE j_PersonId.PersonGuid = @personGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL
                ORDER BY a.PersonBankAccountId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<PersonBankAccount>(sql, new { personGuid });
        }

        public async Task CreatePersonBankAccountAsync(PersonBankAccount personBankAccount, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampCreate(personBankAccount);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (PersonBankAccountGuid, PersonId, CreatedById, StatusId, CreatedTime, [BankBranch], [AccountNumber], [AccountName], [SwiftCode], [IBAN], [CurrencyId], [SrBankAccountType], [IsPrimary], [IsPayrollAccount], [IsActive], [Remarks])
                VALUES
                (@PersonBankAccountGuid, @PersonId, @CreatedById, @StatusId, @CreatedTime, @BankBranch, @AccountNumber, @AccountName, @SwiftCode, @IBAN, @CurrencyId, @SrBankAccountType, @IsPrimary, @IsPayrollAccount, @IsActive, @Remarks){dialect.ReturnIdentitySuffix("PersonBankAccountId")}";
            personBankAccount.PersonBankAccountId = await conn.QuerySingleAsync<long>(sql, personBankAccount, transaction);
        }

        public async Task UpdatePersonBankAccountAsync(PersonBankAccount personBankAccount, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampUpdate(personBankAccount);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     [PersonId] = @PersonId,
                    [BankBranch] = @BankBranch,
                    [AccountNumber] = @AccountNumber,
                    [AccountName] = @AccountName,
                    [SwiftCode] = @SwiftCode,
                    [IBAN] = @IBAN,
                    [CurrencyId] = @CurrencyId,
                    [SrBankAccountType] = @SrBankAccountType,
                    [IsPrimary] = @IsPrimary,
                    [IsPayrollAccount] = @IsPayrollAccount,
                    [IsActive] = @IsActive,
                    [Remarks] = @Remarks,
                    StatusId = @StatusId,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE PersonBankAccountGuid = @PersonBankAccountGuid";
            await conn.ExecuteAsync(sql, personBankAccount, transaction);
        }

        public async Task SoftDeletePersonBankAccountAsync(PersonBankAccount personBankAccount, long deletedBy, IDbTransaction? transaction = null)
        {
            AuditTimestampHelper.StampSoftDelete(personBankAccount, deletedBy);
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE PersonBankAccountGuid = @PersonBankAccountGuid";

            await conn.ExecuteAsync(sql, personBankAccount, transaction);
        }

        public async Task DeletePersonBankAccountAsync(Guid personBankAccountGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonBankAccountGuid = @personBankAccountGuid";
            await conn.ExecuteAsync(sql, new { personBankAccountGuid }, transaction);
        }

        public async Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE PersonId IN (SELECT PersonId FROM {dialect.Qualify("app", "Person")} WHERE PersonGuid = @personGuid)";
            await conn.ExecuteAsync(sql, new { personGuid }, transaction);
        }

        public async Task<IEnumerable<PersonBankAccount>> SearchPersonBankAccountAsync(
            string? bankBranch,
            string? bankBranchSearchType,
            string? accountNumber,
            string? accountNumberSearchType,
            string? accountName,
            string? accountNameSearchType,
            string? swiftCode,
            string? swiftCodeSearchType,
            string? iBAN,
            string? iBANSearchType,
            string? currencyId,
            string? currencyIdSearchType,
            string? isPrimary,
            string? isPrimarySearchType,
            string? isPayrollAccount,
            string? isPayrollAccountSearchType,
            string? isActive,
            string? isActiveSearchType,
            string? remarks,
            string? remarksSearchType,
            Guid personGuid, Guid personBankAccountGuid,
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
            if (personBankAccountGuid != Guid.Empty)
            {
                whereClauses.Add("a.PersonBankAccountGuid = @personBankAccountGuid");
                parameters.Add("@personBankAccountGuid", personBankAccountGuid);
            }

            if (!string.IsNullOrWhiteSpace(bankBranch))
            {
                var param = SqlFilterHelper.BuildFilter("a.BankBranch", "@bankBranch", bankBranchSearchType, parameters, "bankBranch", bankBranch, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                var param = SqlFilterHelper.BuildFilter("a.AccountNumber", "@accountNumber", accountNumberSearchType, parameters, "accountNumber", accountNumber, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(accountName))
            {
                var param = SqlFilterHelper.BuildFilter("a.AccountName", "@accountName", accountNameSearchType, parameters, "accountName", accountName, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(swiftCode))
            {
                var param = SqlFilterHelper.BuildFilter("a.SwiftCode", "@swiftCode", swiftCodeSearchType, parameters, "swiftCode", swiftCode, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(iBAN))
            {
                var param = SqlFilterHelper.BuildFilter("a.IBAN", "@iBAN", iBANSearchType, parameters, "iBAN", iBAN, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(currencyId))
            {
                var param = SqlFilterHelper.BuildFilter("a.CurrencyId", "@currencyId", currencyIdSearchType, parameters, "currencyId", currencyId, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPrimary))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPrimary", "@isPrimary", isPrimarySearchType, parameters, "isPrimary", isPrimary, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isPayrollAccount))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsPayrollAccount", "@isPayrollAccount", isPayrollAccountSearchType, parameters, "isPayrollAccount", isPayrollAccount, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(isActive))
            {
                var param = SqlFilterHelper.BuildFilter("a.IsActive", "@isActive", isActiveSearchType, parameters, "isActive", isActive, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(remarks))
            {
                var param = SqlFilterHelper.BuildFilter("a.Remarks", "@remarks", remarksSearchType, parameters, "remarks", remarks, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_PersonId.PersonName AS PersonName
, j_BankId.BankName AS BankName
, {SqlDialectQueryHelper.CoalesceEmpty(dialect, "v_SrBankAccountType.StandardReferenceItemName")} AS BankAccountTypeName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId
LEFT JOIN {dialect.Qualify("core", "Bank")} j_BankId ON a.BankId = j_BankId.BankId
{SqlDialectQueryHelper.BuildGlobalStandardReferenceLookup(dialect, "v_SrBankAccountType", "SrBankAccountType", "BankAccountType")}

                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.PersonBankAccountId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<PersonBankAccount>(sql, parameters, transaction);
        }
    }
}
