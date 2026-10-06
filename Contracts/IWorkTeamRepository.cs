using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface IWorkTeamRepository
    {
        Task<WorkTeam?> GetWorkTeamAsync(Guid workTeamGuid, bool trackChanges);
        Task<WorkTeam?> GetWorkTeamByIdAsync(long workTeamId, bool trackChanges);
        Task<IEnumerable<WorkTeam>> GetAllWorkTeamsAsync(bool trackChanges);

        Task CreateWorkTeamAsync(WorkTeam workTeam, IDbTransaction? transaction = null);
        Task UpdateWorkTeamAsync(WorkTeam workTeam, IDbTransaction? transaction = null);
        Task DeleteWorkTeamAsync(Guid workTeamGuid, IDbTransaction? transaction = null);
        Task SoftDeleteWorkTeamAsync(WorkTeam workTeam, long deletedBy, IDbTransaction? transaction = null);

        Task<IEnumerable<WorkTeam>> SearchWorkTeamAsync(
            string? workTeamCode,
            string? workTeamCodeSearchType,
            string? workTeamName,
            string? workTeamNameSearchType,
            string? isEditAble,
            string? isEditAbleSearchType,
            string? note,
            string? noteSearchType,
            IDbTransaction? transaction = null);
    }
}
