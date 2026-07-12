using BamboeUp.Report.Abstractions;
using Contracts;
using System.Data;

namespace Service.Shell.Reporting;

internal sealed class RepositoryReportDataProvider : IReportDataProvider
{
    private readonly IRepositoryManager _repository;

    public RepositoryReportDataProvider(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public Task<DataTable> ExecuteStoredProcedureAsync(
        string storedProcedureName,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken = default)
        => _repository.Report.ExecuteReportDataAsync(storedProcedureName, parameters);
}
