USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    Phase 3.9.7 — Bank DOC print data sources
    Konvensi: satu parameter entity per SP — @EntityGuid (prioritas) atau @EntityId.
    Used by ReportDefinition StoreProcedureName:
      app.doc_BankMasterSlip
      app.doc_BankConfirmationLetter
*/

IF OBJECT_ID(N'[app].[doc_BankMasterSlip]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[doc_BankMasterSlip];
GO

CREATE PROCEDURE [app].[doc_BankMasterSlip]
    @EntityGuid UNIQUEIDENTIFIER = NULL,
    @EntityId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.BankId,
        b.BankGuid,
        b.BankName,
        b.BankInitial,
        SYSDATETIME() AS PrintedAt
    FROM [core].[Bank] b
    WHERE b.StatusId > 0
      AND b.DeletedTime IS NULL
      AND (
            (@EntityGuid IS NOT NULL AND b.BankGuid = @EntityGuid)
         OR (@EntityGuid IS NULL AND @EntityId IS NOT NULL AND b.BankId = @EntityId)
      );
END
GO

IF OBJECT_ID(N'[app].[doc_BankConfirmationLetter]', N'P') IS NOT NULL
    DROP PROCEDURE [app].[doc_BankConfirmationLetter];
GO

CREATE PROCEDURE [app].[doc_BankConfirmationLetter]
    @EntityGuid UNIQUEIDENTIFIER = NULL,
    @EntityId BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.BankId,
        b.BankGuid,
        b.BankName,
        b.BankInitial,
        SYSDATETIME() AS PrintedAt
    FROM [core].[Bank] b
    WHERE b.StatusId > 0
      AND b.DeletedTime IS NULL
      AND (
            (@EntityGuid IS NOT NULL AND b.BankGuid = @EntityGuid)
         OR (@EntityGuid IS NULL AND @EntityId IS NOT NULL AND b.BankId = @EntityId)
      );
END
GO

PRINT 'Bank DOC procedures created (single entity param: @EntityGuid / @EntityId).';
GO
