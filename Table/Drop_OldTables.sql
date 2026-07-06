/* =========================================================
   DROP OLD TABLES — After Migration Verification
   Database  : Dev.BamboeHR
   
   JALANKAN SCRIPT INI HANYA SETELAH:
   1. MultiCompany_Hybrid_Schema.sql sudah dijalankan sukses
   2. Data migration sudah diverifikasi (Section B)
   3. API sudah diupdate ke tabel baru
   
   TABEL YANG DIHAPUS:
     - core.UsersCompanies      → digantikan core.UserCompanyScope
     - core.UsersCompanyOffices → digantikan core.UserCompanyScope
     - core.UserGroupUsers      → digantikan core.UserGroupScope
   ========================================================= */

USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================
-- STEP 0: SAFETY CHECK — Pastikan tabel pengganti sudah ada
--         dan sudah berisi data sebelum drop tabel lama
-- =========================================================
DECLARE @ErrorMsg NVARCHAR(500)
DECLARE @UCSCount INT, @UGSCount INT

-- Cek core.UserCompanyScope sudah ada dan berisi data
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserCompanyScope]'))
BEGIN
    SET @ErrorMsg = 'ABORT: core.UserCompanyScope belum dibuat. Jalankan MultiCompany_Hybrid_Schema.sql terlebih dahulu.'
    RAISERROR(@ErrorMsg, 16, 1)
    RETURN
END

-- Cek core.UserGroupScope sudah ada dan berisi data
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupScope]'))
BEGIN
    SET @ErrorMsg = 'ABORT: core.UserGroupScope belum dibuat. Jalankan MultiCompany_Hybrid_Schema.sql terlebih dahulu.'
    RAISERROR(@ErrorMsg, 16, 1)
    RETURN
END

SELECT @UCSCount = COUNT(*) FROM [core].[UserCompanyScope]
SELECT @UGSCount = COUNT(*) FROM [core].[UserGroupScope]

PRINT '==========================================='
PRINT '  SAFETY CHECK RESULTS'
PRINT '==========================================='
PRINT 'core.UserCompanyScope rows : ' + CAST(@UCSCount AS NVARCHAR(20))
PRINT 'core.UserGroupScope rows   : ' + CAST(@UGSCount AS NVARCHAR(20))
PRINT ''

-- Bandingkan row count lama vs baru
DECLARE @OldUCCount   INT = 0
DECLARE @OldUCOCount  INT = 0
DECLARE @OldUGUCount  INT = 0

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanies]'))
    SELECT @OldUCCount = COUNT(*) FROM [core].[UsersCompanies] WHERE StatusId > 0 AND DeletedTime IS NULL

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanyOffices]'))
    SELECT @OldUCOCount = COUNT(*) FROM [core].[UsersCompanyOffices] WHERE StatusId > 0 AND DeletedTime IS NULL

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupUsers]'))
    SELECT @OldUGUCount = COUNT(*) FROM [core].[UserGroupUsers] WHERE StatusId > 0 AND DeletedTime IS NULL

PRINT '--- Data Comparison ---'
PRINT 'UsersCompanies (aktif)     : ' + CAST(@OldUCCount  AS NVARCHAR(20))
PRINT 'UsersCompanyOffices (aktif): ' + CAST(@OldUCOCount AS NVARCHAR(20))
PRINT 'UserGroupUsers (aktif)     : ' + CAST(@OldUGUCount AS NVARCHAR(20))
PRINT ''
PRINT 'UserCompanyScope (baru)    : ' + CAST(@UCSCount    AS NVARCHAR(20))
PRINT 'UserGroupScope (baru)      : ' + CAST(@UGSCount    AS NVARCHAR(20))
PRINT ''

IF @UCSCount = 0 AND (@OldUCCount + @OldUCOCount) > 0
BEGIN
    PRINT '!! WARNING: UserCompanyScope masih kosong tapi tabel lama ada datanya.'
    PRINT '!! Jalankan Section B di MultiCompany_Hybrid_Schema.sql untuk migrasi data.'
    PRINT '!! Script DROP dibatalkan.'
    RETURN
END

IF @UGSCount = 0 AND @OldUGUCount > 0
BEGIN
    PRINT '!! WARNING: UserGroupScope masih kosong tapi UserGroupUsers ada datanya.'
    PRINT '!! Jalankan Section B di MultiCompany_Hybrid_Schema.sql untuk migrasi data.'
    PRINT '!! Script DROP dibatalkan.'
    RETURN
END

PRINT '*** Safety Check PASSED — Lanjut DROP tabel lama ***'
PRINT ''
GO

-- =========================================================
-- STEP 1: DROP core.UsersCompanies
--         (digantikan oleh core.UserCompanyScope)
-- =========================================================
PRINT '--- Dropping core.UsersCompanies ...'

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanies]'))
BEGIN
    -- Drop Foreign Key constraints terlebih dahulu
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UserCompanies_User'    AND parent_object_id = OBJECT_ID(N'[core].[UsersCompanies]'))
        ALTER TABLE [core].[UsersCompanies] DROP CONSTRAINT [FK_UserCompanies_User]

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UserCompanies_Company' AND parent_object_id = OBJECT_ID(N'[core].[UsersCompanies]'))
        ALTER TABLE [core].[UsersCompanies] DROP CONSTRAINT [FK_UserCompanies_Company]

    DROP TABLE [core].[UsersCompanies]
    PRINT '  -> core.UsersCompanies DROPPED.'
END
ELSE
    PRINT '  -> core.UsersCompanies tidak ditemukan, skipped.'
GO

-- =========================================================
-- STEP 2: DROP core.UsersCompanyOffices
--         (digantikan oleh core.UserCompanyScope)
-- =========================================================
PRINT '--- Dropping core.UsersCompanyOffices ...'

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanyOffices]'))
BEGIN
    -- Drop Foreign Key constraints terlebih dahulu
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UserCompanyOffices_User'          AND parent_object_id = OBJECT_ID(N'[core].[UsersCompanyOffices]'))
        ALTER TABLE [core].[UsersCompanyOffices] DROP CONSTRAINT [FK_UserCompanyOffices_User]

    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_UserCompanyOffices_CompanyOffice' AND parent_object_id = OBJECT_ID(N'[core].[UsersCompanyOffices]'))
        ALTER TABLE [core].[UsersCompanyOffices] DROP CONSTRAINT [FK_UserCompanyOffices_CompanyOffice]

    DROP TABLE [core].[UsersCompanyOffices]
    PRINT '  -> core.UsersCompanyOffices DROPPED.'
END
ELSE
    PRINT '  -> core.UsersCompanyOffices tidak ditemukan, skipped.'
GO

-- =========================================================
-- STEP 3: DROP core.UserGroupUsers
--         (digantikan oleh core.UserGroupScope)
-- =========================================================
PRINT '--- Dropping core.UserGroupUsers ...'

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupUsers]'))
BEGIN
    -- UserGroupUsers tidak memiliki FK keluar, langsung drop
    DROP TABLE [core].[UserGroupUsers]
    PRINT '  -> core.UserGroupUsers DROPPED.'
END
ELSE
    PRINT '  -> core.UserGroupUsers tidak ditemukan, skipped.'
GO

-- =========================================================
-- STEP 4: Verifikasi final
-- =========================================================
PRINT ''
PRINT '==========================================='
PRINT '  FINAL VERIFICATION'
PRINT '==========================================='

DECLARE @RemainingOld INT = 0

IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanies]'))      SET @RemainingOld += 1
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UsersCompanyOffices]')) SET @RemainingOld += 1
IF EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID(N'[core].[UserGroupUsers]'))      SET @RemainingOld += 1

IF @RemainingOld = 0
    PRINT '✓ Semua tabel lama berhasil dihapus.'
ELSE
    PRINT '!! ' + CAST(@RemainingOld AS NVARCHAR(5)) + ' tabel lama BELUM terhapus. Cek pesan error di atas.'

PRINT ''

-- Perlihatkan semua tabel yang aktif sekarang
PRINT '--- Active Tables (core & app schema) ---'
SELECT
    s.name + '.' + t.name AS TableName,
    t.create_date          AS CreatedDate,
    p.rows                 AS [RowCount]
FROM sys.tables t
JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
WHERE s.name IN ('core', 'app')
ORDER BY s.name, t.name
GO

PRINT ''
PRINT '========================================================='
PRINT '  DROP SCRIPT COMPLETED'
PRINT '========================================================='
PRINT ''
PRINT 'ACTIVE TABLE STRUCTURE AFTER CLEANUP:'
PRINT '  app.Company'
PRINT '  app.CompanyOffice'
PRINT '  core.Users'
PRINT '  core.Programs'
PRINT '  core.UserGroup'
PRINT '  core.UserGroupProgram    (modified: +ViewAble, +UQ)'
PRINT '  core.UserCompanyScope    (NEW: replaces UsersCompanies + UsersCompanyOffices)'
PRINT '  core.UserGroupScope      (NEW: replaces UserGroupUsers + company context)'
GO
