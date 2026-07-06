using Entities.Exceptions;

namespace Entities.Exceptions
{
    public sealed class VwStandardReferenceItemNotFoundException : NotFoundException
    {
        public VwStandardReferenceItemNotFoundException(long id)
            : base($"VwStandardReferenceItem with id: {id} doesn't exist in the database.")
        {
        }
    }
}
