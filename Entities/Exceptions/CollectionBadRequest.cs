using Entities.Models;

namespace Entities.Exceptions
{


    public sealed class CollectionBadRequest : BadRequestException
    {
        public CollectionBadRequest(string tableName)
        : base($" {tableName} collection sent from a client is null.")
        {
        }
    }
}
