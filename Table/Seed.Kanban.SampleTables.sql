USE [Dev.BamboeHR]
GO

SET NOCOUNT ON
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

/****** Seed UI-09 Kanban — app.WorkItem
      Pipeline: DRAFT → IN_PROGRESS → REVIEW → DONE → ARCHIVED
      Safe to re-run: upsert by WorkItemCode
******/
DECLARE @CreatedById BIGINT = 1
DECLARE @Today DATE = CAST(SYSDATETIME() AS DATE)

IF OBJECT_ID(N'[app].[WorkItem]', N'U') IS NULL
BEGIN
    RAISERROR(N'Table [app].[WorkItem] missing — run Kanban.SampleTables.sql first.', 16, 1)
    RETURN
END

DECLARE @Seed TABLE (
    WorkItemCode varchar(20) NOT NULL PRIMARY KEY,
    Title nvarchar(400) NOT NULL,
    Description nvarchar(4000) NULL,
    Module varchar(100) NULL,
    WorkItemType varchar(50) NULL,
    OwnerName varchar(100) NULL,
    OwnerUserId bigint NULL,
    WorkflowStatus varchar(30) NOT NULL,
    Priority varchar(20) NULL,
    CompletionPercent int NULL,
    Tags varchar(200) NULL,
    Framework varchar(50) NULL,
    DueDate date NULL,
    Note varchar(4000) NULL,
    CreatedTime datetime2(7) NOT NULL,
    UpdatedTime datetime2(7) NOT NULL
)

INSERT INTO @Seed VALUES
/* ——— DRAFT ——— */
('WI-001', N'[Draft] Employee Registration',
 N'Multi-step onboarding — belum mulai desain UI.',
 N'Human Resources', N'Wizard', N'Admin', 1, N'DRAFT', N'Medium', 10,
 N'Onboarding,Backlog', N'MudBlazor', DATEADD(DAY, 28, @Today), N'Start: capture requirements',
 DATEADD(DAY, -2, SYSDATETIME()), DATEADD(DAY, -2, SYSDATETIME())),
('WI-002', N'[Draft] Sales Order Header-Detail',
 N'Sales order + lines — schema siap, UI belum.',
 N'Sales', N'HeaderDetail', N'R&D', 1, N'DRAFT', N'Medium', 15,
 N'Sales,Backlog', N'MudBlazor', DATEADD(DAY, 21, @Today), N'Start: wireframe',
 DATEADD(DAY, -3, SYSDATETIME()), DATEADD(DAY, -3, SYSDATETIME())),
('WI-003', N'[Draft] Customer Registration Card',
 N'CRM customer master card workspace.',
 N'CRM', N'Card', N'Roy', 1, N'DRAFT', N'Low', 5,
 N'CRM,Backlog', N'MudBlazor', DATEADD(DAY, 35, @Today), NULL,
 DATEADD(DAY, -1, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME())),
('WI-004', N'[Draft] Asset Check-Out Form',
 N'Form peminjaman aset karyawan.',
 N'Inventory', N'Form', N'Admin', 1, N'DRAFT', N'Low', 8,
 N'Inventory,Backlog', N'MudBlazor', DATEADD(DAY, 40, @Today), NULL,
 DATEADD(DAY, -4, SYSDATETIME()), DATEADD(DAY, -4, SYSDATETIME())),
('WI-005', N'[Draft] Training Enrollment Wizard',
 N'Wizard daftar pelatihan internal.',
 N'Human Resources', N'Wizard', N'Sara', 1, N'DRAFT', N'Medium', 12,
 N'HR,Training', N'MudBlazor', DATEADD(DAY, 25, @Today), NULL,
 DATEADD(DAY, -5, SYSDATETIME()), DATEADD(DAY, -5, SYSDATETIME())),
('WI-006', N'[Draft] Vendor Master',
 N'Master data vendor procurement.',
 N'Procurement', N'Form', N'R&D', 1, N'DRAFT', N'Medium', 20,
 N'Procurement,Backlog', N'MudBlazor', DATEADD(DAY, 30, @Today), NULL,
 DATEADD(DAY, -6, SYSDATETIME()), DATEADD(DAY, -6, SYSDATETIME())),
('WI-007', N'[Draft] Shift Swap Request',
 N'Request tukar shift antar karyawan.',
 N'Human Resources', N'Form', N'Roy', 1, N'DRAFT', N'Low', 0,
 N'HR,Shift', N'MudBlazor', DATEADD(DAY, 45, @Today), N'Ide baru dari ops',
 SYSDATETIME(), SYSDATETIME()),

/* ——— IN_PROGRESS ——— */
('WI-010', N'[Build] Room Booking Calendar',
 N'Heron calendar + room filters — UI-08 sibling.',
 N'Room Booking', N'Calendar', N'Roy', 1, N'IN_PROGRESS', N'High', 82,
 N'MudBlazor,Heron,Build', N'MudBlazor', DATEADD(DAY, 7, @Today), N'Core day/week done',
 DATEADD(DAY, -10, SYSDATETIME()), SYSDATETIME()),
('WI-011', N'[Build] Overtime Request',
 N'Overtime claim + manager path.',
 N'Human Resources', N'Form', N'Admin', 1, N'IN_PROGRESS', N'Medium', 55,
 N'HR,Payroll,Build', N'MudBlazor', DATEADD(DAY, 10, @Today), N'Form + validation OK',
 DATEADD(DAY, -8, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME())),
('WI-012', N'[Build] Expense Claim',
 N'Expense claim dengan lampiran receipt.',
 N'Finance', N'Form', N'Admin', 1, N'IN_PROGRESS', N'Medium', 48,
 N'Finance,Build', N'MudBlazor', DATEADD(DAY, 12, @Today), N'Upload attachment WIP',
 DATEADD(DAY, -9, SYSDATETIME()), DATEADD(DAY, -2, SYSDATETIME())),
('WI-013', N'[Build] Work Item Kanban Board',
 N'UI-09 TemplateUiKanban proof page.',
 N'Platform', N'Kanban', N'Roy', 1, N'IN_PROGRESS', N'High', 70,
 N'Kanban,UI-09,Build', N'MudBlazor', DATEADD(DAY, 5, @Today), N'DnD + WIP working',
 DATEADD(DAY, -4, SYSDATETIME()), SYSDATETIME()),
('WI-014', N'[Build] Holiday Calendar Sync',
 N'Sinkronisasi libur nasional ke calendar workspace.',
 N'Human Resources', N'Calendar', N'Sara', 1, N'IN_PROGRESS', N'Medium', 40,
 N'HR,Calendar,Build', N'MudBlazor', DATEADD(DAY, 14, @Today), NULL,
 DATEADD(DAY, -7, SYSDATETIME()), DATEADD(DAY, -3, SYSDATETIME())),
('WI-015', N'[Build] Bank Account Section',
 N'Section rekening di Person detail.',
 N'CRM', N'Form', N'Admin', 1, N'IN_PROGRESS', N'Low', 35,
 N'CRM,Build', N'MudBlazor', DATEADD(DAY, 18, @Today), NULL,
 DATEADD(DAY, -11, SYSDATETIME()), DATEADD(DAY, -4, SYSDATETIME())),
('WI-016', N'[Build] Meeting Room Capacity Filter',
 N'Filter kapasitas + lokasi pada booking.',
 N'Room Booking', N'Form', N'R&D', 1, N'IN_PROGRESS', N'Medium', 62,
 N'Room,Build', N'MudBlazor', DATEADD(DAY, 9, @Today), NULL,
 DATEADD(DAY, -5, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME())),

/* ——— REVIEW ——— */
('WI-020', N'[Review] Purchase Order Approval',
 N'PO header-detail + approval — siap QA.',
 N'Procurement', N'Workflow', N'R&D', 1, N'REVIEW', N'High', 88,
 N'Approval,QA', N'MudBlazor', DATEADD(DAY, 2, @Today), N'Awaiting QA sign-off',
 DATEADD(DAY, -12, SYSDATETIME()), SYSDATETIME()),
('WI-021', N'[Review] Payment Request',
 N'Finance payment request — UAT round 1.',
 N'Finance', N'Form', N'R&D', 1, N'REVIEW', N'High', 80,
 N'Finance,QA', N'MudBlazor', DATEADD(DAY, 3, @Today), N'Cek dual-control',
 DATEADD(DAY, -9, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME())),
('WI-022', N'[Review] Goods Receipt',
 N'GR against PO lines — review matching rules.',
 N'Procurement', N'HeaderDetail', N'Admin', 1, N'REVIEW', N'High', 85,
 N'Procurement,QA', N'MudBlazor', DATEADD(DAY, 1, @Today), NULL,
 DATEADD(DAY, -10, SYSDATETIME()), SYSDATETIME()),
('WI-023', N'[Review] Leave Request Form',
 N'Leave form lifecycle — peer review UI.',
 N'Human Resources', N'Form', N'Sara', 1, N'REVIEW', N'Medium', 78,
 N'HR,QA', N'MudBlazor', DATEADD(DAY, 4, @Today), N'Review date validation',
 DATEADD(DAY, -14, SYSDATETIME()), DATEADD(DAY, -2, SYSDATETIME())),
('WI-024', N'[Review] Certification Master',
 N'Master sertifikasi karyawan.',
 N'Human Resources', N'Form', N'Admin', 1, N'REVIEW', N'Low', 72,
 N'HR,QA', N'MudBlazor', DATEADD(DAY, 6, @Today), NULL,
 DATEADD(DAY, -8, SYSDATETIME()), DATEADD(DAY, -1, SYSDATETIME())),
('WI-025', N'[Review] Award Criteria Rules',
 N'Rules engine kriteria award — code review.',
 N'Human Resources', N'Workflow', N'Roy', 1, N'REVIEW', N'High', 90,
 N'HR,Award,QA', N'MudBlazor', DATEADD(DAY, 2, @Today), N'Need security review',
 DATEADD(DAY, -15, SYSDATETIME()), SYSDATETIME()),

/* ——— DONE ——— */
('WI-030', N'[Done] Leave Request v1',
 N'Employee leave form — published UAT.',
 N'Human Resources', N'Form', N'Roy', 1, N'DONE', N'Low', 100,
 N'HR,Done', N'MudBlazor', DATEADD(DAY, -10, @Today), N'Shipped to UAT',
 DATEADD(DAY, -30, SYSDATETIME()), DATEADD(DAY, -7, SYSDATETIME())),
('WI-031', N'[Done] Inventory Transfer',
 N'Warehouse transfer antar lokasi — live.',
 N'Inventory', N'HeaderDetail', N'R&D', 1, N'DONE', N'Medium', 100,
 N'Inventory,Done', N'MudBlazor', DATEADD(DAY, -3, @Today), N'Prod ready',
 DATEADD(DAY, -25, SYSDATETIME()), DATEADD(DAY, -3, SYSDATETIME())),
('WI-032', N'[Done] Skill Catalog',
 N'Master skill + level — selesai.',
 N'Human Resources', N'Form', N'Sara', 1, N'DONE', N'Low', 100,
 N'HR,Done', N'MudBlazor', DATEADD(DAY, -14, @Today), NULL,
 DATEADD(DAY, -40, SYSDATETIME()), DATEADD(DAY, -12, SYSDATETIME())),
('WI-033', N'[Done] Hospital Master',
 N'Master rumah sakit reimburse.',
 N'Finance', N'Form', N'Admin', 1, N'DONE', N'Medium', 100,
 N'Finance,Done', N'MudBlazor', DATEADD(DAY, -8, @Today), NULL,
 DATEADD(DAY, -35, SYSDATETIME()), DATEADD(DAY, -8, SYSDATETIME())),
('WI-034', N'[Done] Company List CRUD',
 N'UI-01 company list/detail — done.',
 N'Platform', N'Form', N'R&D', 1, N'DONE', N'Low', 100,
 N'Platform,Done', N'MudBlazor', DATEADD(DAY, -20, @Today), N'Baseline CRUD',
 DATEADD(DAY, -50, SYSDATETIME()), DATEADD(DAY, -18, SYSDATETIME())),
('WI-035', N'[Done] Parameter Maintenance',
 N'Parameter app shell — selesai.',
 N'Platform', N'Form', N'Admin', 1, N'DONE', N'Low', 100,
 N'Platform,Done', N'MudBlazor', DATEADD(DAY, -22, @Today), NULL,
 DATEADD(DAY, -55, SYSDATETIME()), DATEADD(DAY, -20, SYSDATETIME())),

/* ——— ARCHIVED ——— */
('WI-040', N'[Archived] Invoice Processing v1',
 N'AP invoice v1 — diganti v2.',
 N'Finance', N'Workflow', N'Roy', 1, N'ARCHIVED', N'Low', 100,
 N'Finance,Archive', N'MudBlazor', DATEADD(DAY, -45, @Today), N'Superseded by v2',
 DATEADD(DAY, -80, SYSDATETIME()), DATEADD(DAY, -40, SYSDATETIME())),
('WI-041', N'[Archived] Legacy Leave Excel Import',
 N'Import cuti Excel — diganti form native.',
 N'Human Resources', N'Process', N'Admin', 1, N'ARCHIVED', N'Low', 100,
 N'HR,Archive', N'MudBlazor', DATEADD(DAY, -60, @Today), N'Replaced by Leave Request',
 DATEADD(DAY, -100, SYSDATETIME()), DATEADD(DAY, -55, SYSDATETIME())),
('WI-042', N'[Archived] Old Meeting Booker',
 N'Prototype booking sebelum Room Booking calendar.',
 N'Room Booking', N'Form', N'R&D', 1, N'ARCHIVED', N'Low', 100,
 N'Room,Archive', N'MudBlazor', DATEADD(DAY, -50, @Today), N'Replaced by UI-08',
 DATEADD(DAY, -90, SYSDATETIME()), DATEADD(DAY, -48, SYSDATETIME())),
('WI-043', N'[Archived] Temporary Kanban Mock',
 N'Mock board hard-coded — diganti TemplateUiKanban.',
 N'Platform', N'Kanban', N'Roy', 1, N'ARCHIVED', N'Low', 100,
 N'Kanban,Archive', N'MudBlazor', DATEADD(DAY, -15, @Today), N'Replaced by UI-09',
 DATEADD(DAY, -35, SYSDATETIME()), DATEADD(DAY, -12, SYSDATETIME())),
('WI-044', N'[Archived] POC Wizard Shell',
 N'POC wizard sebelum TemplateUiWizard.',
 N'Platform', N'Wizard', N'Sara', 1, N'ARCHIVED', N'Low', 100,
 N'Platform,Archive', N'MudBlazor', DATEADD(DAY, -70, @Today), NULL,
 DATEADD(DAY, -110, SYSDATETIME()), DATEADD(DAY, -65, SYSDATETIME()))

/* Update existing sample rows */
UPDATE w
SET
    w.[Title] = s.Title,
    w.[Description] = s.Description,
    w.[Module] = s.Module,
    w.[WorkItemType] = s.WorkItemType,
    w.[OwnerName] = s.OwnerName,
    w.[OwnerUserId] = s.OwnerUserId,
    w.[WorkflowStatus] = s.WorkflowStatus,
    w.[Priority] = s.Priority,
    w.[CompletionPercent] = s.CompletionPercent,
    w.[Tags] = s.Tags,
    w.[Framework] = s.Framework,
    w.[DueDate] = s.DueDate,
    w.[Note] = s.Note,
    w.[UpdatedById] = @CreatedById,
    w.[UpdatedTime] = s.UpdatedTime
FROM [app].[WorkItem] w
INNER JOIN @Seed s ON s.WorkItemCode = w.[WorkItemCode]
WHERE w.[DeletedTime] IS NULL

/* Insert missing codes */
INSERT INTO [app].[WorkItem]
(
    [WorkItemCode], [Title], [Description], [Module], [WorkItemType],
    [OwnerName], [OwnerUserId], [WorkflowStatus], [Priority], [CompletionPercent],
    [Tags], [Framework], [DueDate], [Note], [StatusId], [CreatedById], [CreatedTime], [UpdatedTime]
)
SELECT
    s.WorkItemCode, s.Title, s.Description, s.Module, s.WorkItemType,
    s.OwnerName, s.OwnerUserId, s.WorkflowStatus, s.Priority, s.CompletionPercent,
    s.Tags, s.Framework, s.DueDate, s.Note, 1, @CreatedById, s.CreatedTime, s.UpdatedTime
FROM @Seed s
WHERE NOT EXISTS (
    SELECT 1 FROM [app].[WorkItem] w
    WHERE w.[WorkItemCode] = s.WorkItemCode AND w.[DeletedTime] IS NULL
)

PRINT N'WorkItem Kanban seed upsert complete (31 cards across DRAFT→ARCHIVED).'
GO

SELECT [WorkflowStatus], COUNT(*) AS [Cnt]
FROM [app].[WorkItem]
WHERE [DeletedTime] IS NULL AND [StatusId] = 1
GROUP BY [WorkflowStatus]
ORDER BY
    CASE [WorkflowStatus]
        WHEN N'DRAFT' THEN 1
        WHEN N'IN_PROGRESS' THEN 2
        WHEN N'REVIEW' THEN 3
        WHEN N'DONE' THEN 4
        WHEN N'ARCHIVED' THEN 5
        ELSE 9
    END
GO
