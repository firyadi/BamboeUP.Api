using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IPersonBankAccountRepository
    {
        Task<PersonBankAccount?> GetPersonBankAccountAsync(Guid personBankAccountGuid, bool trackChanges);
        Task<PersonBankAccount?> GetByPersonGuidAndPersonBankAccountGuidAsync(Guid personGuid, Guid personBankAccountGuid);
        Task<IEnumerable<PersonBankAccount>> GetAllByPersonGuidAsync(Guid personGuid);

        Task CreatePersonBankAccountAsync(PersonBankAccount personBankAccount, IDbTransaction? transaction = null);
        Task UpdatePersonBankAccountAsync(PersonBankAccount personBankAccount, IDbTransaction? transaction = null);
        Task SoftDeletePersonBankAccountAsync(PersonBankAccount personBankAccount, long deletedBy, IDbTransaction? transaction = null);
        Task DeletePersonBankAccountAsync(Guid personBankAccountGuid, IDbTransaction? transaction = null);
        Task DeleteByPersonGuidAsync(Guid personGuid, IDbTransaction? transaction = null);

        Task<IEnumerable<PersonBankAccount>> SearchPersonBankAccountAsync(
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
            IDbTransaction? transaction = null);
    }
}
