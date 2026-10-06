using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ITempContoh1Repository
    {
        Task<TempContoh1?> GetTempContoh1Async(Guid tempContoh1Guid, bool trackChanges);
        Task<IEnumerable<TempContoh1>> GetAllTempContoh1sAsync(bool trackChanges);

        Task CreateTempContoh1Async(TempContoh1 tempContoh1, IDbTransaction? transaction = null);
        Task UpdateTempContoh1Async(TempContoh1 tempContoh1, IDbTransaction? transaction = null);
        Task DeleteTempContoh1Async(Guid tempContoh1Guid, IDbTransaction? transaction = null);
        Task SoftDeleteTempContoh1Async(TempContoh1 tempContoh1, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<TempContoh1>> SearchTempContoh1Async(

string? personName, string? personNameSearchType,
string? awardTypeName, string? awardTypeNameSearchType,

            IDbTransaction? transaction = null);
    }
}
