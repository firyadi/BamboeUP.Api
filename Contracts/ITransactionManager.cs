// Contracts/ITransactionManager.cs
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        IDbTransaction GetTransaction();
    }
}