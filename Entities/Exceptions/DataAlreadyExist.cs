namespace Entities.Exceptions
{
    public sealed class DataAlreadyExist : NotFoundException
    {

        public DataAlreadyExist(string tableName, Guid dataId)
        : base($"The Table Name: {tableName}, with RecordId: {dataId} Data AlreadyExist in the database.")
        {
        }

    }
}
