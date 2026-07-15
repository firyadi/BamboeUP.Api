-- Person header table already exists in Dev.BamboeHR with full HR schema
-- (FirstName, MiddleName, LastName, SrGenderType, SrReligion, etc.).
-- Do NOT drop [app].[Person] — other tables (PersonFamily, PersonWorkExperience, …) reference it.
--
-- Use detail scripts only when bootstrapping a fresh environment:
--   PersonAddress.sql
--   PersonIdentification.sql
--   PersonEducation.sql
--   PersonEmergencyContact.sql

USE [Dev.BamboeHR]
GO

SELECT OBJECT_ID(N'[app].[Person]', N'U') AS PersonTableExists;
GO
