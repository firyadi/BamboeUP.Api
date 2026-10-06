-- Creates the audit database for a generated BamboeUp application (PostgreSQL).
-- Placeholder {{AuditDatabaseName}} is replaced by SchemaStudio Create Solution
-- as {DatabaseName}AuditDB. Run once as a role that can CREATE DATABASE,
-- then run init-audit.sql while connected to that database.
-- If the database already exists, skip this file and run init-audit.sql only.

CREATE DATABASE "{{AuditDatabaseName}}";
