using Contracts;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Repository
{
    public class DapperTransactionManager : ITransactionManager
    {
        private readonly RepositoryContext _context;
        private SqlTransaction _transaction;

        public DapperTransactionManager(RepositoryContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            var connection = _context.CreateConnection();
            await connection.OpenAsync();
            _transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
            Cleanup();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            Cleanup();
        }

        public IDbTransaction GetTransaction() => _transaction;

        private void Cleanup()
        {
            _transaction?.Connection?.Close();
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}