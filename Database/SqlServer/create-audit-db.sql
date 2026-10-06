-- Creates the audit database for a generated BamboeUp application (SQL Server).
-- Placeholder {{AuditDatabaseName}} is replaced by SchemaStudio Create Solution
-- as {DatabaseName}AuditDB (e.g. ViolettaHR → ViolettaHRAuditDB).
-- Run once before init-audit.sql. Does not DROP an existing database.

IF DB_ID(N'{{AuditDatabaseName}}') IS NULL
BEGIN
    CREATE DATABASE [{{AuditDatabaseName}}];
END
GO

PRINT 'Database {{AuditDatabaseName}} is ready.';
GO
