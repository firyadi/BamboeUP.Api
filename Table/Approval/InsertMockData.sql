USE [Dev.BamboeHR]
GO

-- Pastikan script dijalankan setelah tabel-tabel apv.* dibuat

-- Script ini menggunakan variabel untuk menyimpan ID yang baru diinsert
-- agar bisa digunakan sebagai Foreign Key di tabel berikutnya.

DECLARE @TemplateId BIGINT;
DECLARE @Level1Id BIGINT;
DECLARE @Level2Id BIGINT;
DECLARE @RequestId BIGINT;
DECLARE @Step1Id BIGINT;
DECLARE @Step2Id BIGINT;

-- User IDs (asumsi User dengan ID ini ada di tabel core.Users)
DECLARE @SystemUserId BIGINT = 1;      -- User System pembuat master data
DECLARE @EmployeeUserId BIGINT = 101;  -- Staff yang mengajukan cuti
DECLARE @ManagerUserId BIGINT = 102;   -- Atasan dari staff
DECLARE @HrUserId BIGINT = 103;        -- HR yang melakukan final approval

PRINT '--- 1. INSERT ApprovalTemplate ---'
INSERT INTO [apv].[ApprovalTemplate]
    (ApprovalTemplateGuid, TemplateName, ModuleCode, Description, IsActive, StatusId, CreatedById, CreatedTime)
VALUES
    (NEWID(), 'Pengajuan Cuti Tahunan', 'LEAVE', 'SOP Approval untuk cuti tahunan staff', 1, 1, @SystemUserId, GETUTCDATE());

SET @TemplateId = SCOPE_IDENTITY();

PRINT '--- 2. INSERT ApprovalTemplateLevel ---'
-- Level 1: Atasan Langsung (Direct Manager)
INSERT INTO [apv].[ApprovalTemplateLevel]
    (ApprovalTemplateLevelGuid, ApprovalTemplateId, LevelOrder, LevelName, ApproverType, RequireAllApprovers, CanSkipIfPreviousNotApproved, SlaHours, EscalateToLevelOrder, StatusId, CreatedById, CreatedTime)
VALUES
    (NEWID(), @TemplateId, 1, 'Atasan Langsung', 'DIRECT_MANAGER', 0, 0, 24, NULL, 1, @SystemUserId, GETUTCDATE());

SET @Level1Id = SCOPE_IDENTITY();

-- Level 2: HRD
INSERT INTO [apv].[ApprovalTemplateLevel]
    (ApprovalTemplateLevelGuid, ApprovalTemplateId, LevelOrder, LevelName, ApproverType, RequireAllApprovers, CanSkipIfPreviousNotApproved, SlaHours, EscalateToLevelOrder, StatusId, CreatedById, CreatedTime)
VALUES
    (NEWID(), @TemplateId, 2, 'HR Department', 'SPECIFIC_USER', 0, 0, 48, NULL, 1, @SystemUserId, GETUTCDATE());

SET @Level2Id = SCOPE_IDENTITY();


PRINT '--- 3. INSERT ApprovalTemplateLevelApprover ---'
-- Untuk Level 1 (Atasan Langsung), kita tidak perlu assign spesifik user karena akan dicari via LeaderUserId di runtime
-- Tapi untuk contoh, kita assign jika misalnya 'DIRECT_MANAGER' kosong. Kita lewati dulu.

-- Untuk Level 2 (HRD - SPECIFIC_USER), kita assign HrUserId
INSERT INTO [apv].[ApprovalTemplateLevelApprover]
    (ApprovalTemplateLevelApproverGuid, ApprovalTemplateLevelId, UserId, UserGroupId, StatusId, CreatedById, CreatedTime)
VALUES
    (NEWID(), @Level2Id, @HrUserId, NULL, 1, @SystemUserId, GETUTCDATE());


PRINT '--- 4. INSERT ApprovalRequest ---'
-- Contoh Staff (EmployeeUserId) mengajukan cuti awal bulan
INSERT INTO [apv].[ApprovalRequest]
    (ApprovalRequestGuid, ApprovalTemplateId, RequestNumber, ModuleCode, ReferenceGuid, ReferenceNumber, RequestedByUserId, CurrentLevelOrder, StatusId, Notes, RequestedTime, CreatedById, CreatedTime)
VALUES
    (NEWID(), @TemplateId, 'REQ-LV-202610-001', 'LEAVE', NEWID(), 'LV-2610-0099', @EmployeeUserId, 1, 1, 'Cuti tahunan untuk liburan keluarga', GETUTCDATE(), @EmployeeUserId, GETUTCDATE());

SET @RequestId = SCOPE_IDENTITY();

PRINT '--- 5. INSERT ApprovalStep ---'
-- Step 1 (Menunggu Atasan - ManagerUserId)
INSERT INTO [apv].[ApprovalStep]
    (ApprovalStepGuid, ApprovalRequestId, LevelOrder, LevelName, ApproverUserId, DelegatedFromUserId, StatusId, ActionTime, Comment, SlaDeadline, IsEscalated, CreatedById, CreatedTime)
VALUES
    (NEWID(), @RequestId, 1, 'Atasan Langsung', @ManagerUserId, NULL, 1 /* Pending */, NULL, NULL, DATEADD(hour, 24, GETUTCDATE()), 0, @SystemUserId, GETUTCDATE());

SET @Step1Id = SCOPE_IDENTITY();

-- Step 2 (Menunggu HRD - HrUserId) - Status masih Waiting (0) karena Level 1 belum selesai
INSERT INTO [apv].[ApprovalStep]
    (ApprovalStepGuid, ApprovalRequestId, LevelOrder, LevelName, ApproverUserId, DelegatedFromUserId, StatusId, ActionTime, Comment, SlaDeadline, IsEscalated, CreatedById, CreatedTime)
VALUES
    (NEWID(), @RequestId, 2, 'HR Department', @HrUserId, NULL, 0 /* Waiting */, NULL, NULL, NULL, 0, @SystemUserId, GETUTCDATE());

SET @Step2Id = SCOPE_IDENTITY();


PRINT '--- 6. INSERT ApprovalHistory ---'
-- Log history bahwa request telah di-submit
INSERT INTO [apv].[ApprovalHistory]
    (ApprovalHistoryGuid, ApprovalRequestId, ApprovalStepId, ActionType, ActionByUserId, ActionTime, Comment, FromStatus, ToStatus, LevelOrder)
VALUES
    (NEWID(), @RequestId, NULL, 'SUBMITTED', @EmployeeUserId, GETUTCDATE(), 'User mengajukan cuti', 'Draft', 'Pending', 1);

PRINT 'Mock data berhasi dibuat!'
GO
