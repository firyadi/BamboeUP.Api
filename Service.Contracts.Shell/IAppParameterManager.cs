namespace Service.Contracts.Shell
{
    public interface IAppParameterManager
    {
        Task<T> GetValueAsync<T>(string parameterName, T defaultValue);
        Task<int> GetMaxResultRecordAsync();
    }
}
