using Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Contracts
{
    public partial interface ITrainingRepository
    {
        Task<Training?> GetTrainingAsync(Guid trainingGuid, bool trackChanges);
        Task<IEnumerable<Training>> GetAllTrainingAsync(bool trackChanges);

        Task CreateTrainingAsync(Training training, IDbTransaction? transaction = null);
        Task UpdateTrainingAsync(Training training, IDbTransaction? transaction = null);
        Task DeleteTrainingAsync(Guid trainingGuid, IDbTransaction? transaction = null);
        Task SoftDeleteTrainingAsync(Training training, long deletedBy, IDbTransaction? transaction = null);
        
        Task<IEnumerable<Training>> SearchTrainingAsync(
            string? trainingCode,
            string? trainingCodeSearchType,
            string? trainingName,
            string? trainingNameSearchType,
            string? trainingShortName,
            string? trainingShortNameSearchType,
            string? srTrainingCategory,
            string? srTrainingCategorySearchType,
            string? srTrainingType,
            string? srTrainingTypeSearchType,
            string? srTrainingLevel,
            string? srTrainingLevelSearchType,
            string? durationHour,
            string? durationHourSearchType,
            string? validityMonth,
            string? validityMonthSearchType,
            string? srTrainingProvider,
            string? srTrainingProviderSearchType,
            string? isMandatory,
            string? isMandatorySearchType,
            string? isCertification,
            string? isCertificationSearchType,
            string? isExternal,
            string? isExternalSearchType,
            string? description,
            string? descriptionSearchType,
            string? isActive,
            string? isActiveSearchType,

            IDbTransaction? transaction = null);
    }
}
