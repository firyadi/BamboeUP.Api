USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    AUTO-GENERATED — Phase 4.2 SP stub for Attendance
    DataSource: Table [app].[Attendance]
    ReportDefinition StoreProcedureName: app.rpt_Attendance
    ParameterProfile: standard
    FastReport DataSource: AttendanceData
*/

IF OBJECT_ID(N'[app].[rpt_Attendance]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[rpt_Attendance];
GO

CREATE PROCEDURE [app].[rpt_Attendance]
    @CompanyId NVARCHAR(50) = NULL,
    @CompanyOfficeId NVARCHAR(50) = NULL,
    @DateFrom DATETIME = NULL,
    @DateTo DATETIME = NULL
            AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.AttendanceId,
        t.AttendanceGuid,
        t.CompanyId,
        t.CompanyOfficeId,
        t.PersonId,
        t.AttendanceDate,
        t.CheckinTime,
        t.CheckOutTime,
        t.CheckinChannel,
        t.CheckOutChannel,
        t.CheckinFileStorageId,
        t.CheckOutFileStorageId,
        t.CheckinLatitude,
        t.CheckinLongitude,
        t.CheckinAccuracyMeters,
        t.CheckOutLatitude,
        t.CheckOutLongitude,
        t.CheckOutAccuracyMeters,
        t.Note,
        t.StatusId,
        SYSDATETIME() AS PrintedAt
    FROM [app].[Attendance] t
    WHERE 1 = 1
      AND t.StatusId > 0
      -- TODO: add soft-delete filter if applicable
      AND (@CompanyId IS NULL OR CAST(t.CompanyId AS NVARCHAR(50)) = @CompanyId)
      AND (@CompanyOfficeId IS NULL OR CAST(t.CompanyOfficeId AS NVARCHAR(50)) = @CompanyOfficeId)
      AND (@DateFrom IS NULL OR t.CreatedTime >= @DateFrom)
      AND (@DateTo IS NULL OR t.CreatedTime < DATEADD(day, 1, CAST(CAST(@DateTo AS DATE) AS DATETIME)))
                ;
END
GO

PRINT 'app.rpt_Attendance created.';
GO