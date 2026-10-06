using Shared.DataTransferObjects;

namespace Service.Contracts.Shell
{
    public partial interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllByPersonGuidAsync(Guid personGuid);
    }
}
