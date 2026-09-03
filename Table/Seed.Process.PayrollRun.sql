USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   Seed ProcessDefinition: PAYROLL_RUN
   Companion SP: app.usp_PayrollRun
   UI-07 TemplateUiProcess pilot
   ========================================================= */

IF NOT EXISTS (SELECT 1 FROM [core].[ProcessDefinition] WHERE [ProcessCode] = 'PAYROLL_RUN' AND [DeletedTime] IS NULL)
BEGIN
    DECLARE @DefId BIGINT;
    DECLARE @DefGuid UNIQUEIDENTIFIER = NEWID();

    INSERT INTO [core].[ProcessDefinition]
        ([ProcessDefinitionGuid], [ProcessCode], [ProcessName], [Description], [Category],
         [Version], [ExecutionMode], [RequiresApproval], [AllowRetry], [AllowCancel], [AllowSchedule],
         [IsActive], [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefGuid, 'PAYROLL_RUN', N'Payroll Run',
         N'Calculate and post monthly payroll for a company period (demo SP app.usp_PayrollRun)',
         'Payroll', 1, 2, 0, 1, 1, 0, 1, 1, 1, SYSDATETIME());

    SET @DefId = SCOPE_IDENTITY();

    INSERT INTO [core].[ProcessParameterDefinition]
        ([ProcessDefinitionId], [ParameterCode], [ParameterName], [DataType], [ControlType],
         [IsRequired], [DefaultValue], [DisplayOrder], [GroupName], [IsVisible],
         [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefId, 'CompanyId',    N'Company',       'bigint',  'Lookup',   1, NULL, 1, N'Scope',   1, 1, 1, SYSDATETIME()),
        (@DefId, 'PeriodYear',   N'Period Year',   'int',     'Number',   1, CAST(YEAR(GETDATE()) AS NVARCHAR(10)), 2, N'Period', 1, 1, 1, SYSDATETIME()),
        (@DefId, 'PeriodMonth',  N'Period Month',  'int',     'Number',   1, CAST(MONTH(GETDATE()) AS NVARCHAR(10)), 3, N'Period', 1, 1, 1, SYSDATETIME()),
        (@DefId, 'ValidateOnly', N'Validate Only', 'boolean', 'Boolean',  0, N'false', 4, N'Options', 1, 1, 1, SYSDATETIME());

    INSERT INTO [core].[ProcessStepDefinition]
        ([ProcessDefinitionId], [StepNo], [StepCode], [StepName], [StepType],
         [IsMandatory], [AllowRetry], [AllowSkip], [ExecutionOrder], [ConfigurationJson],
         [StatusId], [CreatedById], [CreatedTime])
    VALUES
        (@DefId, 1, 'VALIDATE_PERIOD', N'Validate Period', 'Validation', 1, 1, 0, 1,
         N'{"handlerKey":"PayrollValidatePeriodStep","procedure":"app.usp_PayrollRun","phase":"VALIDATE"}', 1, 1, SYSDATETIME()),
        (@DefId, 2, 'CALCULATE_PAY', N'Calculate Pay', 'DataProcessing', 1, 1, 0, 2,
         N'{"handlerKey":"PayrollCalculatePayStep","procedure":"app.usp_PayrollRun","phase":"CALCULATE"}', 1, 1, SYSDATETIME()),
        (@DefId, 3, 'COMMIT_PAYROLL', N'Commit Payroll', 'DatabaseCommand', 1, 1, 0, 3,
         N'{"handlerKey":"PayrollCommitStep","procedure":"app.usp_PayrollRun","phase":"COMMIT"}', 1, 1, SYSDATETIME());

    PRINT 'Seeded PAYROLL_RUN process definition.';
END
ELSE
    PRINT 'PAYROLL_RUN process definition already exists.';
GO
