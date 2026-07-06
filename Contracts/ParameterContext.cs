namespace Contracts
{
    public static class ParameterContext
    {
        private static readonly AsyncLocal<int?> _maxResultRecord = new AsyncLocal<int?>();

        public static int MaxResultRecord
        {
            get => _maxResultRecord.Value ?? 150;
            set => _maxResultRecord.Value = value;
        }
    }
}
