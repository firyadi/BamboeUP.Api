using System;

namespace Shared.DataTransferObjects
{
    public partial record WorkCapabilityDto
    {
        public long WorkCapabilityId { get; set; }
        public Guid WorkCapabilityGuid { get; init; }
        public string CapabilityCode { get; set; } = string.Empty;
        public string CapabilityName { get; set; } = string.Empty;
        public string? CapabilityDescription { get; set; }
        public int StatusId { get; set; }
    }
}
