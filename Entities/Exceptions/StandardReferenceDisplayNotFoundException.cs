using Entities.Exceptions;

namespace Entities.Exceptions
{
    public sealed class StandardReferenceDisplayNotFoundException : NotFoundException
    {
        public StandardReferenceDisplayNotFoundException(long id)
            : base($"StandardReferenceDisplay with id: {id} doesn't exist in the database.")
        {
        }
    }
}
