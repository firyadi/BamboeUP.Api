namespace Entities.Exceptions
{
    public sealed class DataNotFoundException : NotFoundException
    {

        public DataNotFoundException(string tableName, Guid dataId)
        : base($"The Table Name: {tableName}, with RecordId: {dataId} doesn't exist in the database.")
        {
        }

    }
}
