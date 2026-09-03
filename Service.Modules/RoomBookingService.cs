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
    public partial class RoomBookingService : IRoomBookingService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly ITransactionManager _transactionManager;
        private readonly IAuditService _audit;
        private readonly IUserContext _userContext;

        public RoomBookingService(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = null!;
            _audit = null!;
            _userContext = null!;
        }

        public RoomBookingService(
            IRepositoryManager repository,
            ILoggerManager logger,
            ITransactionManager transactionManager,
            IAuditService audit,
            IUserContext userContext)
        {
            _repository = repository;
            _logger = logger;
            _transactionManager = transactionManager;
            _audit = audit;
            _userContext = userContext;
        }

        public async Task<IEnumerable<RoomBookingDto>> GetAllRoomBookingsAsync(bool trackChanges)
        {
            var entities = await _repository.RoomBooking.GetAllByPersonGuidAsync(Guid.Empty);
            return entities.Adapt<IEnumerable<RoomBookingDto>>();
        }

        public async Task<RoomBookingDto?> GetRoomBookingByGuidAsync(Guid roomBookingGuid, bool trackChanges)
        {
            var entity = await _repository.RoomBooking.GetRoomBookingAsync(roomBookingGuid, trackChanges);
            if (entity == null) return null;
            var dto = entity.Adapt<RoomBookingDto>();
            return dto;
        }

        public async Task<RoomBookingDto> CreateRoomBookingAsync(Guid personGuid, RoomBookingForCreationDto input)
        {
            var model = input.Adapt<RoomBooking>();
            model.StatusId = 1;
            model.CreatedTime = DateTime.UtcNow;
            await _repository.RoomBooking.CreateRoomBookingAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "CREATE",
                RootTableName = "RoomBooking",
                RootEntityKey = model.RoomBookingGuid.ToString(),
                RootDisplayName = model.RoomBookingCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.CreatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "RoomBooking",
                        EntityKey = model.RoomBookingGuid.ToString(),
                        EntityDisplayName = model.RoomBookingCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "CREATE",
                        OldEntity = null,
                        NewEntity = model
                    }
                }
            });

            return model.Adapt<RoomBookingDto>();
        }

        public async Task UpdateRoomBookingAsync(Guid personGuid, Guid roomBookingGuid, RoomBookingForUpdateDto input, bool trackChanges)
        {
            var oldDetail = await _repository.RoomBooking.GetRoomBookingAsync(roomBookingGuid, false);

            var model = input.Adapt<RoomBooking>();
            model.RoomBookingGuid = roomBookingGuid;
            model.StatusId = 2;
            model.UpdatedTime = DateTime.UtcNow;
            await _repository.RoomBooking.UpdateRoomBookingAsync(model);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "UPDATE",
                RootTableName = "RoomBooking",
                RootEntityKey = model.RoomBookingGuid.ToString(),
                RootDisplayName = model.RoomBookingCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : model.UpdatedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "RoomBooking",
                        EntityKey = model.RoomBookingGuid.ToString(),
                        EntityDisplayName = model.RoomBookingCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "UPDATE",
                        OldEntity = oldDetail,
                        NewEntity = model
                    }
                }
            });
        }

        public async Task DeleteRoomBookingAsync(Guid personGuid, Guid roomBookingGuid, RoomBookingForDeleteDto input, bool trackChanges)
        {
            var oldDetail = await _repository.RoomBooking.GetRoomBookingAsync(roomBookingGuid, false);
            var model = new RoomBooking
            {
                RoomBookingGuid = roomBookingGuid,
                DeletedById = input.DeletedById,
                DeletedTime = DateTime.UtcNow
            };
            await _repository.RoomBooking.SoftDeleteRoomBookingAsync(model, input.DeletedById);

            await _audit.LogSessionAsync(new AuditSessionInput
            {
                SessionType = "DELETE",
                RootTableName = "RoomBooking",
                RootEntityKey = roomBookingGuid.ToString(),
                RootDisplayName = oldDetail?.RoomBookingCode,
                UserId = _userContext.UserGuid != Guid.Empty ? _userContext.UserGuid.ToString() : input.DeletedById.ToString(),
                Entries = new()
                {
                    new AuditLogEntry
                    {
                        TableName = "RoomBooking",
                        EntityKey = roomBookingGuid.ToString(),
                        EntityDisplayName = oldDetail?.RoomBookingCode,
                        ParentTableName = "Person",
                        ParentEntityKey = personGuid.ToString(),
                        ActionType = "DELETE",
                        OldEntity = oldDetail,
                        NewEntity = null
                    }
                }
            });
        }

        public async Task DeleteRoomBookingByAdminAsync(Guid roomBookingGuid, bool trackChanges)
        {
            await _repository.RoomBooking.DeleteRoomBookingAsync(roomBookingGuid);
        }

        public async Task<IEnumerable<RoomBookingDto>> SearchRoomBookingAsync(
            string? roomBookingCode, string? roomBookingCodeSearchType, string? subject, string? subjectSearchType, string? startDateTime, string? startDateTimeSearchType, string? endDateTime, string? endDateTimeSearchType, string? note, string? noteSearchType,
            Guid personGuid, Guid roomBookingGuid)
        {
            var data = await _repository.RoomBooking.SearchRoomBookingAsync(
                roomBookingCode, roomBookingCodeSearchType, subject, subjectSearchType, startDateTime, startDateTimeSearchType, endDateTime, endDateTimeSearchType, note, noteSearchType, 
                personGuid, roomBookingGuid);
            return data.Adapt<IEnumerable<RoomBookingDto>>();
        }

        public async Task<IEnumerable<RoomBookingDto>> GetAllByPersonGuidAsync(Guid personGuid)
        {
            var result = await _repository.RoomBooking.GetAllByPersonGuidAsync(personGuid);
            return result.Adapt<IEnumerable<RoomBookingDto>>();
        }

        public async Task<RoomBookingDto?> GetByPersonGuidAndRoomBookingGuidAsync(Guid personGuid, Guid roomBookingGuid)
        {
            var result = await _repository.RoomBooking.GetByPersonGuidAndRoomBookingGuidAsync(personGuid, roomBookingGuid);
            if (result == null) return null;
            return result.Adapt<RoomBookingDto>();
        }
    }
}
