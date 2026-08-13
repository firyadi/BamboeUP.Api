using System.Collections.Generic;

namespace Shared.DataTransferObjects
{
    public partial record CompanyForCreationDto
    {
        public IEnumerable<CompanyOfficeForCreationDto>? Offices { get; set; }
    }

    public partial record CompanyForUpdateDto
    {
        public IEnumerable<CompanyOfficeForUpdateDto>? Offices { get; set; }
    }
}
