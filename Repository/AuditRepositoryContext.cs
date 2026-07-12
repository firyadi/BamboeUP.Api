using Microsoft.Data.SqlClient;

namespace Repository
{
    public class AuditRepositoryContext
    {
        private readonly string _connectionString;

        public AuditRepositoryContext(string connectionString)
            => _connectionString = connectionString;

        public SqlConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
