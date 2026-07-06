using System;

namespace Shared.DataTransferObjects
{
    public record VwStandardReferenceItemDto
    {
        public long StandardReferenceItemId { get; init; }
        public Guid StandardReferenceItemGuid { get; init; }
        public long CompanyId { get; init; }
        public Guid CompanyGuid { get; init; }
        public long CompanyOfficeId { get; init; }
        public Guid CompanyOfficeGuid { get; init; }
        public long StandardReferenceId { get; init; }
        public Guid StandardReferenceGuid { get; init; }
        public string StandardReferenceInitial { get; init; }
        public string StandardReferenceItemInitial { get; init; }
        public string StandardReferenceItemName { get; init; }
        public string Note { get; init; }
    }
}
