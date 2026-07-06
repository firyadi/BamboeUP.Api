-- INSERT SCRIPT FOR AUTO NUMBER LOG MONITOR
-- Pastikan Module 'Logs' atau sejenisnya ada, jika tidak, taruh di module 'Auto Number'

DECLARE @ParentId BIGINT = (SELECT TOP 1 ProgramId FROM [app].[Program] WHERE ProgramCode = 'SYS.AUTONUMBER');

IF NOT EXISTS (SELECT 1 FROM [app].[Program] WHERE ProgramCode = 'SYS.AUTONUMBER.LOGS')
BEGIN
    INSERT INTO [app].[Program]
    (ParentId, ProgramCode, ProgramName, ProgramType, NavigateUrl, IconCode, SortOrder, IsActive, CreatedTime)
    VALUES
    (@ParentId, 'SYS.AUTONUMBER.LOGS', 'Auto Number Logs', 'PRG', '/app/autonumberlogmonitor', 'Icons.Material.Filled.History', 30, 1, GETUTCDATE());
END
GO
