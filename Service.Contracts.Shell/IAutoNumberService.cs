using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public interface IAutoNumberService
    {
        Task<IEnumerable<AutoNumberDto>> GetAllAutoNumbersAsync(bool trackChanges);
        Task<AutoNumberDto> GetAutoNumberByGuidAsync(Guid autoNumberGuid, bool trackChanges);
        Task<AutoNumberDto> CreateAutoNumberAsync(AutoNumberForCreationDto input);
        Task UpdateAutoNumberAsync(Guid autoNumberGuid, AutoNumberForUpdateDto input, bool trackChanges);
        Task DeleteAutoNumberAsync(Guid autoNumberGuid, AutoNumberForDeleteDto input, bool trackChanges);
        Task DeleteAutoNumberByAdminAsync(Guid autoNumberGuid, bool trackChanges);

        Task<IEnumerable<AutoNumberDto>> SearchAutoNumberAsync(
            string? prefik, string? prefikSearchType, string? separatorAfterPrefik, string? separatorAfterPrefikSearchType, string? separatorAfterDept, string? separatorAfterDeptSearchType, string? separatorAfterYear, string? separatorAfterYearSearchType, string? separatorAfterMonth, string? separatorAfterMonthSearchType, string? separatorAfterDay, string? separatorAfterDaySearchType, string? numberGroupSeparator, string? numberGroupSeparatorSearchType, string? numberFormat, string? numberFormatSearchType
        );

    }
}
