using Shared.DataTransferObjects;

namespace Service.Contracts.Modules
{
    public partial interface IBankService
    {
        Task<IEnumerable<BankDto>> GetAllBanksAsync(bool trackChanges);
        Task<BankDto?> GetBankByGuidAsync(Guid bankGuid, bool trackChanges);
        Task<BankDto> CreateBankAsync(BankForCreationDto input);
        Task UpdateBankAsync(Guid bankGuid, BankForUpdateDto input, bool trackChanges);
        Task DeleteBankAsync(Guid bankGuid, BankForDeleteDto input, bool trackChanges);
        Task DeleteBankByAdminAsync(Guid bankGuid, bool trackChanges);

        Task<IEnumerable<BankDto>> SearchBankAsync(
            string? bankName, string? bankNameSearchType, string? bankInitial, string? bankInitialSearchType
        );

    }
}
