using Contracts;
using Dapper;
using Entities.Models;
using Repository.Database;
using Repository.Extensions;
using System.Data;

namespace Repository
{
    public partial class AttendanceRepository(RepositoryContext context, ISqlDialect dialect) : IAttendanceRepository
    {
        private readonly string _table = dialect.Qualify("app", "Attendance");
        private readonly int _maxRows = ParameterContext.MaxResultRecord;

        public async Task<Attendance?> GetAttendanceAsync(Guid attendanceGuid, bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE a.AttendanceGuid = @attendanceGuid
                  AND a.StatusId > 0
                  AND a.DeletedTime IS NULL";
            return await connection.QuerySingleOrDefaultAsync<Attendance>(sql, new { attendanceGuid });
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync(bool trackChanges)
        {
            using var connection = context.CreateConnection();
            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName



                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE a.StatusId > 0 AND a.DeletedTime IS NULL
                ORDER BY a.AttendanceId DESC{dialect.AppendLimit(_maxRows)}";
            return await connection.QueryAsync<Attendance>(sql);
        }

        public async Task CreateAttendanceAsync(Attendance attendance, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                INSERT INTO {_table}
                (AttendanceGuid, CreatedById, StatusId, CreatedTime, CompanyId, CompanyOfficeId, PersonId, AttendanceDate, CheckinTime, CheckOutTime, CheckinChannel, CheckOutChannel, CheckinFileStorageId, CheckOutFileStorageId, CheckinLatitude, CheckinLongitude, CheckinAccuracyMeters, CheckOutLatitude, CheckOutLongitude, CheckOutAccuracyMeters, Note
                )
                VALUES
                (@AttendanceGuid, @CreatedById, @StatusId, @CreatedTime, @CompanyId, @CompanyOfficeId, @PersonId, @AttendanceDate, @CheckinTime, @CheckOutTime, @CheckinChannel, @CheckOutChannel, @CheckinFileStorageId, @CheckOutFileStorageId, @CheckinLatitude, @CheckinLongitude, @CheckinAccuracyMeters, @CheckOutLatitude, @CheckOutLongitude, @CheckOutAccuracyMeters, @Note
                ){dialect.ReturnIdentitySuffix("AttendanceId")}";
            attendance.AttendanceId = await conn.QuerySingleAsync<long>(sql, attendance, transaction);
        }

        public async Task UpdateAttendanceAsync(Attendance attendance, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET                     CompanyId = @CompanyId,
                    CompanyOfficeId = @CompanyOfficeId,
                    PersonId = @PersonId,
                    AttendanceDate = @AttendanceDate,
                    CheckinTime = @CheckinTime,
                    CheckOutTime = @CheckOutTime,
                    CheckinChannel = @CheckinChannel,
                    CheckOutChannel = @CheckOutChannel,
                    CheckinFileStorageId = @CheckinFileStorageId,
                    CheckOutFileStorageId = @CheckOutFileStorageId,
                    CheckinLatitude = @CheckinLatitude,
                    CheckinLongitude = @CheckinLongitude,
                    CheckinAccuracyMeters = @CheckinAccuracyMeters,
                    CheckOutLatitude = @CheckOutLatitude,
                    CheckOutLongitude = @CheckOutLongitude,
                    CheckOutAccuracyMeters = @CheckOutAccuracyMeters,
                    Note = @Note,
                    StatusId = @StatusId,
                    SyncVersion = SyncVersion + 1,
                    UpdatedById = @UpdatedById,
                    UpdatedTime = @UpdatedTime
                WHERE AttendanceGuid = @AttendanceGuid";
            await conn.ExecuteAsync(sql, attendance, transaction);
        }

        public async Task SoftDeleteAttendanceAsync(Attendance attendance, long deletedBy, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"
                UPDATE {_table}
                SET StatusId = 0,
                    DeletedById = @DeletedById,
                    DeletedTime = @DeletedTime
                WHERE AttendanceGuid = @AttendanceGuid";

            await conn.ExecuteAsync(sql, attendance, transaction);
        }

        public async Task DeleteAttendanceAsync(Guid attendanceGuid, IDbTransaction? transaction = null)
        {
            var conn = transaction?.Connection ?? context.CreateConnection();
            var sql = $@"DELETE FROM {_table} WHERE AttendanceGuid = @attendanceGuid";
            await conn.ExecuteAsync(sql, new { attendanceGuid }, transaction);
        }

        public async Task<IEnumerable<Attendance>> SearchAttendanceAsync(
            string? attendanceDate,
            string? attendanceDateSearchType,
            string? checkinTime,
            string? checkinTimeSearchType,
            string? checkOutTime,
            string? checkOutTimeSearchType,
            string? checkinChannel,
            string? checkinChannelSearchType,
            string? checkOutChannel,
            string? checkOutChannelSearchType,
            string? checkinLatitude,
            string? checkinLatitudeSearchType,
            string? checkinLongitude,
            string? checkinLongitudeSearchType,
            string? checkinAccuracyMeters,
            string? checkinAccuracyMetersSearchType,
            string? checkOutLatitude,
            string? checkOutLatitudeSearchType,
            string? checkOutLongitude,
            string? checkOutLongitudeSearchType,
            string? checkOutAccuracyMeters,
            string? checkOutAccuracyMetersSearchType,
            string? note,
            string? noteSearchType,
string? companyName, string? companyNameSearchType,
string? companyOfficeName, string? companyOfficeNameSearchType,
string? personName, string? personNameSearchType,

            IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? context.CreateConnection();
            List<string> whereClauses = [ "a.StatusId > 0", "a.DeletedTime IS NULL" ];
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(attendanceDate))
            {
                var param = SqlFilterHelper.BuildFilter("a.AttendanceDate", "@attendanceDate", attendanceDateSearchType, parameters, "attendanceDate", attendanceDate, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkinTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckinTime", "@checkinTime", checkinTimeSearchType, parameters, "checkinTime", checkinTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkOutTime))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckOutTime", "@checkOutTime", checkOutTimeSearchType, parameters, "checkOutTime", checkOutTime, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkinChannel))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckinChannel", "@checkinChannel", checkinChannelSearchType, parameters, "checkinChannel", checkinChannel, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkOutChannel))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckOutChannel", "@checkOutChannel", checkOutChannelSearchType, parameters, "checkOutChannel", checkOutChannel, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkinLatitude))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckinLatitude", "@checkinLatitude", checkinLatitudeSearchType, parameters, "checkinLatitude", checkinLatitude, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkinLongitude))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckinLongitude", "@checkinLongitude", checkinLongitudeSearchType, parameters, "checkinLongitude", checkinLongitude, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkinAccuracyMeters))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckinAccuracyMeters", "@checkinAccuracyMeters", checkinAccuracyMetersSearchType, parameters, "checkinAccuracyMeters", checkinAccuracyMeters, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkOutLatitude))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckOutLatitude", "@checkOutLatitude", checkOutLatitudeSearchType, parameters, "checkOutLatitude", checkOutLatitude, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkOutLongitude))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckOutLongitude", "@checkOutLongitude", checkOutLongitudeSearchType, parameters, "checkOutLongitude", checkOutLongitude, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(checkOutAccuracyMeters))
            {
                var param = SqlFilterHelper.BuildFilter("a.CheckOutAccuracyMeters", "@checkOutAccuracyMeters", checkOutAccuracyMetersSearchType, parameters, "checkOutAccuracyMeters", checkOutAccuracyMeters, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                var param = SqlFilterHelper.BuildFilter("a.Note", "@note", noteSearchType, parameters, "note", note, dialect);
                if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param);
            }
if (!string.IsNullOrWhiteSpace(companyName)) { var param = SqlFilterHelper.BuildFilter("j_CompanyId.CompanyName", "@companyName", companyNameSearchType, parameters, "companyName", companyName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(companyOfficeName)) { var param = SqlFilterHelper.BuildFilter("j_CompanyOfficeId.CompanyOfficeName", "@companyOfficeName", companyOfficeNameSearchType, parameters, "companyOfficeName", companyOfficeName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }
if (!string.IsNullOrWhiteSpace(personName)) { var param = SqlFilterHelper.BuildFilter("j_PersonId.PersonName", "@personName", personNameSearchType, parameters, "personName", personName, dialect); if (!string.IsNullOrWhiteSpace(param)) whereClauses.Add(param); }


            var sql = $@"
                {SqlDialectQueryHelper.SelectLimited(dialect, _maxRows, "a.*")}
, j_CompanyId.CompanyName AS CompanyName
, j_CompanyOfficeId.CompanyOfficeName AS CompanyOfficeName
, j_PersonId.PersonName AS PersonName


                FROM {_table} a
LEFT JOIN {dialect.Qualify("app", "Company")} j_CompanyId ON a.CompanyId = j_CompanyId.CompanyId
LEFT JOIN {dialect.Qualify("app", "CompanyOffice")} j_CompanyOfficeId ON a.CompanyOfficeId = j_CompanyOfficeId.CompanyOfficeId
LEFT JOIN {dialect.Qualify("app", "Person")} j_PersonId ON a.PersonId = j_PersonId.PersonId


                WHERE {string.Join(" AND ", whereClauses)}
                ORDER BY a.AttendanceId DESC{dialect.AppendLimit(_maxRows)}";

            return await connection.QueryAsync<Attendance>(sql, parameters, transaction);
        }
    }
}
