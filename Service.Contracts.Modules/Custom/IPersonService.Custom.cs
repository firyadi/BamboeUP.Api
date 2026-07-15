using Shared.DataTransferObjects;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface IPersonService
    {
        Task<PersonDto> OnboardPersonAsync(PersonQuickOnboardDto input);
    }
}
