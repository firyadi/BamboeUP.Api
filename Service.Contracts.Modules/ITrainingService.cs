using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts.Modules
{
    public partial interface ITrainingService
    {
        Task<IEnumerable<TrainingDto>> GetAllTrainingAsync(bool trackChanges);
        Task<TrainingDto?> GetTrainingByGuidAsync(Guid trainingGuid, bool trackChanges);
        Task<TrainingDto> CreateTrainingAsync(TrainingForCreationDto input);
        Task UpdateTrainingAsync(Guid trainingGuid, TrainingForUpdateDto input, bool trackChanges);
        Task DeleteTrainingAsync(Guid trainingGuid, TrainingForDeleteDto input, bool trackChanges);
        Task DeleteTrainingByAdminAsync(Guid trainingGuid, bool trackChanges);

        Task<IEnumerable<TrainingDto>> SearchTrainingAsync(
            string? trainingCode, string? trainingCodeSearchType, string? trainingName, string? trainingNameSearchType, string? trainingShortName, string? trainingShortNameSearchType, string? srTrainingCategory, string? srTrainingCategorySearchType, string? srTrainingType, string? srTrainingTypeSearchType, string? srTrainingLevel, string? srTrainingLevelSearchType, string? durationHour, string? durationHourSearchType, string? validityMonth, string? validityMonthSearchType, string? srTrainingProvider, string? srTrainingProviderSearchType, string? isMandatory, string? isMandatorySearchType, string? isCertification, string? isCertificationSearchType, string? isExternal, string? isExternalSearchType, string? description, string? descriptionSearchType, string? isActive, string? isActiveSearchType

        );
    }
}
