using Microsoft.Data.SqlClient;
using System.Data;

namespace Repository
{
    public class RepositoryContext
    {
        private readonly string _connectionString;

        public RepositoryContext(string connectionString)
            => _connectionString = connectionString;

        public SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}