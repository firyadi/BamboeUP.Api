using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonBankAccountService
    {
        Task<IEnumerable<PersonBankAccountDto>> GetAllPersonBankAccountsAsync(bool trackChanges);
        Task<PersonBankAccountDto?> GetPersonBankAccountByGuidAsync(Guid personBankAccountGuid, bool trackChanges);
        Task<PersonBankAccountDto> CreatePersonBankAccountAsync(Guid personGuid, PersonBankAccountForCreationDto input);
        Task UpdatePersonBankAccountAsync(Guid personGuid, Guid personBankAccountGuid, PersonBankAccountForUpdateDto input, bool trackChanges);
        Task DeletePersonBankAccountAsync(Guid personGuid, Guid personBankAccountGuid, PersonBankAccountForDeleteDto input, bool trackChanges);
        Task DeletePersonBankAccountByAdminAsync(Guid personBankAccountGuid, bool trackChanges);

        Task<IEnumerable<PersonBankAccountDto>> SearchPersonBankAccountAsync(
            string? bankId, string? bankIdSearchType, string? bankBranch, string? bankBranchSearchType, string? accountNumber, string? accountNumberSearchType, string? accountName, string? accountNameSearchType, string? swiftCode, string? swiftCodeSearchType, string? iBAN, string? iBANSearchType, string? currencyId, string? currencyIdSearchType, string? srBankAccountType, string? srBankAccountTypeSearchType, string? isPrimary, string? isPrimarySearchType, string? isPayrollAccount, string? isPayrollAccountSearchType, string? isActive, string? isActiveSearchType, string? remarks, string? remarksSearchType,
            Guid personGuid, Guid personBankAccountGuid);

        // Detail (child) helpers
        Task<IEnumerable<PersonBankAccountDto>> GetAllByPersonGuidAsync(Guid personGuid);
        Task<PersonBankAccountDto?> GetByPersonGuidAndPersonBankAccountGuidAsync(Guid personGuid, Guid personBankAccountGuid);
    }
}
