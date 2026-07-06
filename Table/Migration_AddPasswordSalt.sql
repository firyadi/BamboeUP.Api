/* =========================================================
   MIGRATION: Add PasswordSalt column to core.Users
   Database  : Dev.BamboeHR
   Author    : BamboeUp
   Created   : 2026-05-14
   
   PURPOSE:
     Menambahkan kolom PasswordSalt ke tabel core.Users untuk
     meningkatkan keamanan password hashing. Salt di-generate
     secara unik per user dan di-prepend ke password sebelum
     BCrypt hashing.
   
   BACKWARD COMPATIBLE:
     - Default value '' (empty string) memastikan data existing
       tetap valid.
     - Login flow akan auto-upgrade user lama: saat login berhasil
       dengan password tanpa salt, sistem otomatis generate salt
       baru dan update hash + salt.
   ========================================================= */

USE [Dev.BamboeHR]
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

PRINT '=== Migration: Add PasswordSalt to core.Users ==='
PRINT ''

-- Step 1: Add PasswordSalt column if not exists
IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[core].[Users]') 
      AND name = 'PasswordSalt'
)
BEGIN
    ALTER TABLE [core].[Users]
        ADD [PasswordSalt] NVARCHAR(128) NOT NULL
            CONSTRAINT [DF_Users_PasswordSalt] DEFAULT ('')

    PRINT '  -> Column [PasswordSalt] added to core.Users.'
END
ELSE
    PRINT '  -> Column [PasswordSalt] already exists, skipped.'
GO

PRINT ''
PRINT '=== Migration Completed ==='
PRINT ''
PRINT 'NOTES:'
PRINT '  1. Existing users will have PasswordSalt = '''' (empty).'
PRINT '  2. On next login, the system will auto-generate a new salt'
PRINT '     and re-hash the password with salt + BCrypt.'
PRINT '  3. New users created via API will always have a unique salt.'
GO
