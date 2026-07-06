using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace Service.Modules
{
    public class HolidayService : IHolidayService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;

        public HolidayService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
        }

        public async Task<IEnumerable<HolidayDto>> GetAllHolidaysAsync(bool trackChanges)
        {
            var entities = await _repository.Holiday.GetAllHolidaysAsync(trackChanges);
            return entities.Adapt<IEnumerable<HolidayDto>>();
        }

        public async Task<HolidayDto> GetHolidayByGuidAsync(Guid holidayGuid, bool trackChanges)
        {
            var entity = await _repository.Holiday.GetHolidayAsync(holidayGuid, trackChanges);
            return entity.Adapt<HolidayDto>();
        }

        public async Task<HolidayDto> CreateHolidayAsync(HolidayForCreationDto input)
        {
            var model = input.Adapt<Holiday>();
            model.StatusId = 1;
            await _repository.Holiday.CreateHolidayAsync(model);
            return model.Adapt<HolidayDto>();
        }

        public async Task UpdateHolidayAsync(Guid holidayGuid, HolidayForUpdateDto input, bool trackChanges)
        {
            var model = input.Adapt<Holiday>();
            model.HolidayGuid = holidayGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.Holiday.UpdateHolidayAsync(model);
        }

        // Soft delete (pass full entity for audit)
        public async Task DeleteHolidayAsync(Guid holidayGuid, HolidayForDeleteDto input, bool trackChanges)
        {
            var model = new Holiday { HolidayGuid = holidayGuid };
            await _repository.Holiday.SoftDeleteHolidayAsync(model, input.DeletedById ?? 0);
        }

        public async Task DeleteHolidayByAdminAsync(Guid holidayGuid, bool trackChanges)
        {
            await _repository.Holiday.DeleteHolidayAsync(holidayGuid);
        }

        public async Task<IEnumerable<HolidayDto>> SearchHolidayAsync(
            string? yearPeriode, string? yearPeriodeSearchType, string? note, string? noteSearchType, DateTime? holidayDatesFrom, DateTime? holidayDatesTo)
        {
            var data = await _repository.Holiday.SearchHolidayAsync(
                yearPeriode, yearPeriodeSearchType, note, noteSearchType, holidayDatesFrom, holidayDatesTo);
            return data.Adapt<IEnumerable<HolidayDto>>();
        }
    }
}