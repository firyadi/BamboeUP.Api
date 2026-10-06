using Mapster;
using Shared.DataTransferObjects;

namespace Service.Shell
{
    public partial class CompanyService
    {
        public async Task<IEnumerable<CompanyDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var accessibleIds = (await _userScope.GetAccessibleCompaniesAsync())
                .Select(c => c.CompanyId)
                .ToHashSet();

            var data = await _repository.Company.GetAllByPersonGuidAsync(personGuid);

            return data
                .Where(c => accessibleIds.Contains(c.CompanyId))
                .Adapt<IEnumerable<CompanyDto>>();
        }
    }
}
