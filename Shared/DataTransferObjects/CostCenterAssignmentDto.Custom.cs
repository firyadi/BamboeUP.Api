using Shared.Settings.Enums;

namespace Shared.DataTransferObjects
{
    public partial class CostCenterAssignmentSearchDto
    {
        public string? CompanyId { get; set; }
        public SearchType CompanyIdSearchType { get; set; } = SearchType.Contains;

        public string? CompanyOfficeId { get; set; }
        public SearchType CompanyOfficeIdSearchType { get; set; } = SearchType.Contains;

        public string? ProfitCenterId { get; set; }
        public SearchType ProfitCenterIdSearchType { get; set; } = SearchType.Contains;

        public string? CostCenterManagerEmployeeId { get; set; }
        public SearchType CostCenterManagerEmployeeIdSearchType { get; set; } = SearchType.Contains;
    }
}
