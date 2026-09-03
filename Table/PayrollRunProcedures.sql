USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* =========================================================
   PROCEDURE: app.usp_PayrollRun
   Fake-but-realistic payroll batch for UI-07 TemplateUiProcess.

   Phases (@Phase):
     VALIDATE   — check company + period uniqueness; create Draft/Validated run
     CALCULATE  — build employee lines (from app.Employee or demo rows)
     COMMIT     — post run (or skip when @ValidateOnly = 1)
     RUN        — all phases in one call (default)

   Result sets:
     1) Header summary (PayrollRun)
     2) Detail lines — when phase includes calculate/commit/run
   ========================================================= */

CREATE OR ALTER PROCEDURE [app].[usp_PayrollRun]
    @CompanyId              BIGINT,
    @PeriodYear             INT,
    @PeriodMonth            INT,
    @ValidateOnly           BIT = 0,
    @CreatedById            BIGINT = 1,
    @ProcessExecutionGuid   UNIQUEIDENTIFIER = NULL,
    @Phase                  VARCHAR(20) = 'RUN',
    @PayrollRunGuid         UNIQUEIDENTIFIER = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @PhaseNorm VARCHAR(20) = UPPER(LTRIM(RTRIM(ISNULL(@Phase, 'RUN'))));
    DECLARE @RunId BIGINT;
    DECLARE @RunGuid UNIQUEIDENTIFIER = @PayrollRunGuid;
    DECLARE @RunCode VARCHAR(30);
    DECLARE @PeriodStart DATE;
    DECLARE @PeriodEnd DATE;
    DECLARE @CompanyName NVARCHAR(200);
    DECLARE @Currency CHAR(3) = 'IDR';
    DECLARE @Msg NVARCHAR(500);

    IF @PhaseNorm NOT IN ('VALIDATE', 'CALCULATE', 'COMMIT', 'RUN')
    BEGIN
        RAISERROR(N'Invalid @Phase. Use VALIDATE | CALCULATE | COMMIT | RUN.', 16, 1);
        RETURN;
    END;

    IF @PeriodMonth NOT BETWEEN 1 AND 12
    BEGIN
        RAISERROR(N'@PeriodMonth must be 1–12.', 16, 1);
        RETURN;
    END;

    IF @PeriodYear NOT BETWEEN 2000 AND 2100
    BEGIN
        RAISERROR(N'@PeriodYear out of range.', 16, 1);
        RETURN;
    END;

    SET @PeriodStart = DATEFROMPARTS(@PeriodYear, @PeriodMonth, 1);
    SET @PeriodEnd = EOMONTH(@PeriodStart);

    SELECT TOP (1)
        @CompanyName = c.CompanyName,
        @Currency = ISNULL(NULLIF(RTRIM(c.DefaultCurrency), ''), 'IDR')
    FROM [app].[Company] c
    WHERE c.CompanyId = @CompanyId
      AND c.StatusId > 0
      AND c.DeletedTime IS NULL;

    IF @CompanyName IS NULL
    BEGIN
        RAISERROR(N'CompanyId %I64d not found or inactive.', 16, 1, @CompanyId);
        RETURN;
    END;

    BEGIN TRY
        BEGIN TRAN;

        /* ---------- VALIDATE ---------- */
        IF @PhaseNorm IN ('VALIDATE', 'RUN')
        BEGIN
            IF EXISTS (
                SELECT 1
                FROM [app].[PayrollRun] r
                WHERE r.CompanyId = @CompanyId
                  AND r.PeriodYear = @PeriodYear
                  AND r.PeriodMonth = @PeriodMonth
                  AND r.DeletedTime IS NULL
                  AND r.RunStatus <> 'Cancelled'
                  AND (@RunGuid IS NULL OR r.PayrollRunGuid <> @RunGuid)
            )
            BEGIN
                SET @Msg = CONCAT(N'Payroll already exists for ', @CompanyName, N' ',
                    FORMAT(@PeriodStart, 'yyyy-MM'), N'. Cancel or use existing run.');
                RAISERROR(@Msg, 16, 1);
            END;

            IF @RunGuid IS NULL
                SET @RunGuid = NEWID();

            SET @RunCode = CONCAT('PR-', @CompanyId, '-', FORMAT(@PeriodStart, 'yyyyMM'), '-',
                RIGHT(REPLACE(CONVERT(VARCHAR(36), @RunGuid), '-', ''), 6));

            IF NOT EXISTS (SELECT 1 FROM [app].[PayrollRun] WHERE PayrollRunGuid = @RunGuid)
            BEGIN
                INSERT INTO [app].[PayrollRun]
                    (PayrollRunGuid, PayrollRunCode, CompanyId, PeriodYear, PeriodMonth,
                     PeriodStart, PeriodEnd, CurrencyCode, RunStatus, ValidateOnly,
                     Note, ProcessExecutionGuid, StatusId, CreatedById, CreatedTime)
                VALUES
                    (@RunGuid, @RunCode, @CompanyId, @PeriodYear, @PeriodMonth,
                     @PeriodStart, @PeriodEnd, @Currency, 'Validated', @ValidateOnly,
                     CONCAT(N'Payroll run for ', @CompanyName), @ProcessExecutionGuid, 1, @CreatedById, SYSDATETIME());
            END
            ELSE
            BEGIN
                UPDATE [app].[PayrollRun]
                SET RunStatus = 'Validated',
                    ValidateOnly = @ValidateOnly,
                    ProcessExecutionGuid = ISNULL(@ProcessExecutionGuid, ProcessExecutionGuid),
                    UpdatedById = @CreatedById,
                    UpdatedTime = SYSDATETIME()
                WHERE PayrollRunGuid = @RunGuid
                  AND DeletedTime IS NULL;
            END;

            SELECT @RunId = PayrollRunId
            FROM [app].[PayrollRun]
            WHERE PayrollRunGuid = @RunGuid;
        END;

        IF @RunId IS NULL
        BEGIN
            IF @RunGuid IS NULL
                RAISERROR(N'@PayrollRunGuid required for CALCULATE/COMMIT (or use Phase=RUN).', 16, 1);

            SELECT @RunId = PayrollRunId, @RunCode = PayrollRunCode
            FROM [app].[PayrollRun]
            WHERE PayrollRunGuid = @RunGuid
              AND DeletedTime IS NULL;

            IF @RunId IS NULL
                RAISERROR(N'PayrollRun not found for @PayrollRunGuid.', 16, 1);
        END;

        SET @PayrollRunGuid = @RunGuid;

        /* ---------- CALCULATE ---------- */
        IF @PhaseNorm IN ('CALCULATE', 'RUN')
        BEGIN
            DELETE FROM [app].[PayrollRunDetail]
            WHERE PayrollRunId = @RunId;

            ;WITH Src AS
            (
                SELECT
                    ROW_NUMBER() OVER (ORDER BY e.EmployeeId) AS [LineNo],
                    e.EmployeeId,
                    e.EmployeeNo,
                    LTRIM(RTRIM(CONCAT(
                        ISNULL(p.FirstName, N''), N' ',
                        ISNULL(p.MiddleName, N''), N' ',
                        ISNULL(p.LastName, N'')
                    ))) AS EmployeeName,
                    CAST(4500000 + (e.EmployeeId % 17) * 250000 AS DECIMAL(18,2)) AS BaseSalary,
                    CAST(500000 + (e.EmployeeId % 5) * 100000 AS DECIMAL(18,2)) AS AllowanceAmount,
                    CAST((e.EmployeeId % 3) * 150000 AS DECIMAL(18,2)) AS OvertimeAmount,
                    CAST((e.EmployeeId % 4) * 75000 AS DECIMAL(18,2)) AS DeductionAmount
                FROM [app].[Employee] e
                LEFT JOIN [app].[Person] p ON p.PersonId = e.PersonId
                WHERE e.StatusId > 0
                  AND e.DeletedTime IS NULL
                  AND e.TerminationDate IS NULL
            ),
            Demo AS
            (
                SELECT * FROM (VALUES
                    (1, CAST(NULL AS BIGINT), N'DEMO-001', N'Demo Employee One',   CAST(5500000 AS DECIMAL(18,2)), CAST(750000 AS DECIMAL(18,2)), CAST(200000 AS DECIMAL(18,2)), CAST(100000 AS DECIMAL(18,2))),
                    (2, CAST(NULL AS BIGINT), N'DEMO-002', N'Demo Employee Two',   CAST(6200000 AS DECIMAL(18,2)), CAST(800000 AS DECIMAL(18,2)), CAST(0 AS DECIMAL(18,2)), CAST(150000 AS DECIMAL(18,2))),
                    (3, CAST(NULL AS BIGINT), N'DEMO-003', N'Demo Employee Three', CAST(4800000 AS DECIMAL(18,2)), CAST(500000 AS DECIMAL(18,2)), CAST(300000 AS DECIMAL(18,2)), CAST(50000 AS DECIMAL(18,2))),
                    (4, CAST(NULL AS BIGINT), N'DEMO-004', N'Demo Employee Four',  CAST(7100000 AS DECIMAL(18,2)), CAST(900000 AS DECIMAL(18,2)), CAST(100000 AS DECIMAL(18,2)), CAST(200000 AS DECIMAL(18,2))),
                    (5, CAST(NULL AS BIGINT), N'DEMO-005', N'Demo Employee Five',  CAST(3900000 AS DECIMAL(18,2)), CAST(400000 AS DECIMAL(18,2)), CAST(0 AS DECIMAL(18,2)), CAST(0 AS DECIMAL(18,2)))
                ) AS v([LineNo], EmployeeId, EmployeeNo, EmployeeName, BaseSalary, AllowanceAmount, OvertimeAmount, DeductionAmount)
                WHERE NOT EXISTS (SELECT 1 FROM Src)
            ),
            Combined AS
            (
                SELECT * FROM Src
                UNION ALL
                SELECT * FROM Demo
            ),
            Calc AS
            (
                SELECT
                    c.*,
                    CAST(c.BaseSalary + c.AllowanceAmount + c.OvertimeAmount AS DECIMAL(18,2)) AS GrossPay,
                    CAST(ROUND((c.BaseSalary + c.AllowanceAmount + c.OvertimeAmount) * 0.05, 0) AS DECIMAL(18,2)) AS TaxAmount,
                    CASE
                        WHEN c.BaseSalary < 3000000 THEN 'Error'
                        WHEN c.DeductionAmount > c.BaseSalary * 0.3 THEN 'Warning'
                        ELSE 'OK'
                    END AS LineStatus,
                    CASE
                        WHEN c.BaseSalary < 3000000 THEN N'Base salary below minimum threshold'
                        WHEN c.DeductionAmount > c.BaseSalary * 0.3 THEN N'Deduction exceeds 30% of base'
                        ELSE NULL
                    END AS Message
                FROM Combined c
            )
            INSERT INTO [app].[PayrollRunDetail]
                (PayrollRunId, [LineNo], EmployeeId, EmployeeNo, EmployeeName,
                 BaseSalary, AllowanceAmount, OvertimeAmount, DeductionAmount, TaxAmount,
                 GrossPay, NetPay, LineStatus, Message, StatusId, CreatedById, CreatedTime)
            SELECT
                @RunId,
                [LineNo],
                EmployeeId,
                EmployeeNo,
                ISNULL(NULLIF(LTRIM(RTRIM(EmployeeName)), N''), EmployeeNo),
                BaseSalary,
                AllowanceAmount,
                OvertimeAmount,
                DeductionAmount,
                TaxAmount,
                GrossPay,
                CAST(GrossPay - DeductionAmount - TaxAmount AS DECIMAL(18,2)),
                LineStatus,
                Message,
                1,
                @CreatedById,
                SYSDATETIME()
            FROM Calc;

            UPDATE r
            SET
                RunStatus = 'Calculated',
                EmployeeCount = x.Cnt,
                SuccessCount = x.OkCnt,
                WarningCount = x.WarnCnt,
                FailedCount = x.ErrCnt,
                TotalGross = x.Gross,
                TotalAllowance = x.Allow,
                TotalDeduction = x.Ded,
                TotalTax = x.Tax,
                TotalNet = x.Net,
                UpdatedById = @CreatedById,
                UpdatedTime = SYSDATETIME()
            FROM [app].[PayrollRun] r
            CROSS APPLY (
                SELECT
                    COUNT(*) AS Cnt,
                    SUM(CASE WHEN d.LineStatus = 'OK' THEN 1 ELSE 0 END) AS OkCnt,
                    SUM(CASE WHEN d.LineStatus = 'Warning' THEN 1 ELSE 0 END) AS WarnCnt,
                    SUM(CASE WHEN d.LineStatus = 'Error' THEN 1 ELSE 0 END) AS ErrCnt,
                    SUM(d.GrossPay) AS Gross,
                    SUM(d.AllowanceAmount) AS Allow,
                    SUM(d.DeductionAmount) AS Ded,
                    SUM(d.TaxAmount) AS Tax,
                    SUM(d.NetPay) AS Net
                FROM [app].[PayrollRunDetail] d
                WHERE d.PayrollRunId = @RunId AND d.DeletedTime IS NULL
            ) x
            WHERE r.PayrollRunId = @RunId;
        END;

        /* ---------- COMMIT ---------- */
        IF @PhaseNorm IN ('COMMIT', 'RUN')
        BEGIN
            IF @ValidateOnly = 1
            BEGIN
                UPDATE [app].[PayrollRun]
                SET RunStatus = 'Validated',
                    Note = LEFT(CONCAT(ISNULL(Note, N''), N' [ValidateOnly - not posted]'), 1000),
                    UpdatedById = @CreatedById,
                    UpdatedTime = SYSDATETIME()
                WHERE PayrollRunId = @RunId;
            END
            ELSE
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM [app].[PayrollRunDetail]
                    WHERE PayrollRunId = @RunId AND LineStatus = 'Error' AND DeletedTime IS NULL
                )
                    RAISERROR(N'Cannot post payroll: one or more lines have Error status.', 16, 1);

                UPDATE [app].[PayrollRun]
                SET RunStatus = 'Posted',
                    UpdatedById = @CreatedById,
                    UpdatedTime = SYSDATETIME()
                WHERE PayrollRunId = @RunId;
            END;
        END;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH;

    SELECT
        r.PayrollRunId,
        r.PayrollRunGuid,
        r.PayrollRunCode,
        r.CompanyId,
        @CompanyName AS CompanyName,
        r.PeriodYear,
        r.PeriodMonth,
        r.PeriodStart,
        r.PeriodEnd,
        r.CurrencyCode,
        r.RunStatus,
        r.ValidateOnly,
        r.EmployeeCount,
        r.SuccessCount,
        r.WarningCount,
        r.FailedCount,
        r.TotalGross,
        r.TotalAllowance,
        r.TotalDeduction,
        r.TotalTax,
        r.TotalNet,
        r.Note,
        r.ProcessExecutionGuid,
        SYSDATETIME() AS ProcessedAt
    FROM [app].[PayrollRun] r
    WHERE r.PayrollRunId = @RunId;

    IF @PhaseNorm <> 'VALIDATE'
    BEGIN
        SELECT
            d.PayrollRunDetailId,
            d.PayrollRunDetailGuid,
            d.PayrollRunId,
            d.[LineNo],
            d.EmployeeId,
            d.EmployeeNo,
            d.EmployeeName,
            d.BaseSalary,
            d.AllowanceAmount,
            d.OvertimeAmount,
            d.DeductionAmount,
            d.TaxAmount,
            d.GrossPay,
            d.NetPay,
            d.LineStatus,
            d.Message
        FROM [app].[PayrollRunDetail] d
        WHERE d.PayrollRunId = @RunId
          AND d.DeletedTime IS NULL
        ORDER BY d.[LineNo];
    END;
END
GO

PRINT 'Created [app].[usp_PayrollRun].';
GO
