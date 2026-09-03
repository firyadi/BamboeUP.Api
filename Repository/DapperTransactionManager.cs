using Contracts;
using System.Data;

namespace Repository;

public class DapperTransactionManager : ITransactionManager
{
    private readonly RepositoryContext _context;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public DapperTransactionManager(RepositoryContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        _connection = _context.CreateConnection();
        if (_connection is System.Data.Common.DbConnection dbConnection)
            await dbConnection.OpenAsync();
        else
            _connection.Open();

        if (_connection is System.Data.Common.DbConnection dbConn)
            _transaction = await dbConn.BeginTransactionAsync();
        else
            _transaction = _connection.BeginTransaction();
    }

    public async Task CommitAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction has not been started.");

        if (_transaction is System.Data.Common.DbTransaction dbTransaction)
            await dbTransaction.CommitAsync();
        else
            _transaction.Commit();

        Cleanup();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction has not been started.");

        if (_transaction is System.Data.Common.DbTransaction dbTransaction)
            await dbTransaction.RollbackAsync();
        else
            _transaction.Rollback();

        Cleanup();
    }

    public IDbTransaction GetTransaction() =>
        _transaction ?? throw new InvalidOperationException("Transaction has not been started.");

    private void Cleanup()
    {
        _transaction?.Dispose();
        _transaction = null;

        if (_connection is not null)
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}
