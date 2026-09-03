using BamboeUp.Audit.Contracts;
using BamboeUp.Audit.Models;
using Mapster;
using Contracts;
using Entities.Models;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Modules
{
    public partial class HolidayService(
        IRepositoryManager repository,
        ILoggerManager logger,
        ITransactionManager transactionManager,
        IAuditService audit,
        IUserContext userContext) : IHolidayService
    {
        public async Task<IEnumerable<HolidayDto>> GetAllHolidaysAsync(bool trackChanges)
        {
            var entities = await repository.Holiday.GetAllHolidaysAsync(trackChanges);
            return entities.Adapt<IEnumerable<HolidayDto>>();
        }

        public async Task<HolidayDto?> GetHolidayByGuidAsync(Guid holidayGuid, bool trackChanges)
        {
            var entity = await repository.Holiday.GetHolidayAsync(holidayGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<HolidayDto>();
            return dto;
        }

        public async Task<HolidayDto> CreateHolidayAsync(HolidayForCreationDto input)
        {
            var model = input.Adapt<Holiday>();

            if (model.HolidayGuid == Guid.Empty)
                model.HolidayGuid = Guid.NewGuid();

            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await repository.Holiday.CreateHolidayAsync(model);


            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "Holiday",
                RootEntityKey = model.HolidayGuid.ToString(),
                RootDisplayName = model.HolidayName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Holiday",
                        EntityKey = model.HolidayGuid.ToString(),
                        EntityDisplayName = model.HolidayName,
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                ]
            });

            return model.Adapt<HolidayDto>();
        }

        public async Task UpdateHolidayAsync(Guid holidayGuid, HolidayForUpdateDto input, bool trackChanges)
        {
            var oldEntity = await repository.Holiday.GetHolidayAsync(holidayGuid, false)
                ?? throw new KeyNotFoundException($"Holiday '{{holidayGuid}}' was not found.");

            var model = input.Adapt<Holiday>();
            model.HolidayGuid = holidayGuid;

            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await repository.Holiday.UpdateHolidayAsync(model);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "Holiday",
                RootEntityKey = model.HolidayGuid.ToString(),
                RootDisplayName = model.HolidayName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Holiday",
                        EntityKey = model.HolidayGuid.ToString(),
                        EntityDisplayName = model.HolidayName,
                        ActionType = "UPDATE",
                        OldEntity = oldEntity,
                        NewEntity = model
                    }
                ]
            });
        }

        public async Task DeleteHolidayAsync(Guid holidayGuid, HolidayForDeleteDto input, bool trackChanges)
        {
            var oldEntity = await repository.Holiday.GetHolidayAsync(holidayGuid, false);
            var model = new Holiday
            {
                HolidayGuid = holidayGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await repository.Holiday.SoftDeleteHolidayAsync(model, input.DeletedById);

            await audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "Holiday",
                RootEntityKey = holidayGuid.ToString(),
                RootDisplayName = oldEntity?.HolidayName,
                UserId = userContext.UserGuid != Guid.Empty ? userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries =
                [
                    new AuditLogEntry
                    {
                        TableName = "Holiday",
                        EntityKey = holidayGuid.ToString(),
                        EntityDisplayName = oldEntity?.HolidayName,
                        ActionType = "DELETE",
                        OldEntity = oldEntity,
                        NewEntity = null
                    }
                ]
            });
        }

        public async Task DeleteHolidayByAdminAsync(Guid holidayGuid, bool trackChanges)
        {
            await repository.Holiday.DeleteHolidayAsync(holidayGuid);
        }

        public async Task<IEnumerable<HolidayDto>> SearchHolidayAsync(
            string? holidayDate, string? holidayDateSearchType, string? holidayName, string? holidayNameSearchType, string? holidayDescription, string? holidayDescriptionSearchType, string? srHolidayType, string? srHolidayTypeSearchType, string? isNational, string? isNationalSearchType, string? isCompanyHoliday, string? isCompanyHolidaySearchType

            )
        {
            var data = await repository.Holiday.SearchHolidayAsync(
                holidayDate, holidayDateSearchType, holidayName, holidayNameSearchType, holidayDescription, holidayDescriptionSearchType, srHolidayType, srHolidayTypeSearchType, isNational, isNationalSearchType, isCompanyHoliday, isCompanyHolidaySearchType

                );
            return data.Adapt<IEnumerable<HolidayDto>>();
        }

        public async Task<IReadOnlyList<CalendarEventDto>> GetCalendarEventsAsync(
            DateTime start, DateTime end, string? category = null, string? search = null)
        {
            var rows = await repository.Holiday.GetHolidaysByDateRangeAsync(start, end, category, search);
            return rows.Select(h => new CalendarEventDto
            {
                EventGuid = h.HolidayGuid,
                Title = h.HolidayName,
                Start = h.HolidayDate.Date,
                End = h.HolidayDate.Date.AddDays(1).AddTicks(-1),
                IsAllDay = true,
                Category = h.SrHolidayType?.ToString(),
                Description = h.HolidayDescription,
                Color = h.IsNational ? "#7E57C2" : h.IsCompanyHoliday ? "#1976D2" : "#78909C",
                Status = h.StatusId > 0 ? "Active" : "Inactive",
                CanEdit = true,
                CanDelete = true
            }).ToList();
        }
    }
}
