using System.ComponentModel.DataAnnotations;

namespace Shared.RequestFeatures
{
    public class StandardReferenceDisplayParameters
    {
        public long CompanyId { get; set; } = 0;
        public long CompanyOfficeId { get; set; } = 0;
        
        [Required]
        public string StandardReferenceInitial { get; set; } = string.Empty;
        public long? StandardReferenceGroupId { get; set; }
    }
}
