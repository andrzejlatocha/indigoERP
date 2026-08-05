-- Wersja dla SQL Server 2000 skryptu _SQL Queries/Serwer_Monitoring_Login.sql.
-- SQL Server 2000 nie obsługuje CREATE LOGIN / CREATE USER ... FOR LOGIN / ALTER ROLE
-- (dodane dopiero w SQL Server 2005) — to błąd składni na etapie parsowania całej partii,
-- więc nie da się tego rozgałęzić IF-em w jednym skrypcie z nowoczesną wersją.
-- Używamy starych systemowych procedur, które istnieją w SQL Server 2000.
--
-- Login/hasło muszą się zgadzać z indigoERP.Api/appsettings.json ->
-- appSettings:MonitoringSqlLogin / MonitoringSqlPassword.
--
-- Uruchomić NA KAŻDYM monitorowanym serwerze SQL 2000 (typ 'SQL' w tabeli dbo.Serwer).

USE master
GO

IF NOT EXISTS (SELECT 1 FROM master.dbo.syslogins WHERE name = 'indigo.monitoring')
BEGIN
    EXEC sp_addlogin
        @loginame = 'indigo.monitoring',
        @passwd = 'mUNLB0GbJQjEDbee5s2a3U6OmOOL0m7c3uIOHEarBh9I04xekx',
        @defdb = 'msdb'
END
GO

USE msdb
GO

IF NOT EXISTS (SELECT 1 FROM sysusers WHERE name = 'indigo.monitoring')
BEGIN
    EXEC sp_grantdbaccess @loginame = 'indigo.monitoring', @name_in_db = 'indigo.monitoring'
END
GO

-- SQL Server 2000 nie ma ról SQLAgentReaderRole/SQLAgentUserRole (dodanych w 2005) —
-- w 2000 msdb.dbo.sysjobs/sysjobhistory nie mają tego samego row-level security co w nowszych
-- wersjach, więc zwykły dostęp do bazy (db_datareader) wystarcza żeby SELECT-ować wszystkie JOB-y.
IF NOT EXISTS (
    SELECT 1
    FROM sysmembers m
    JOIN sysusers r ON r.uid = m.groupuid
    JOIN sysusers u ON u.uid = m.memberuid
    WHERE r.name = 'db_datareader' AND u.name = 'indigo.monitoring'
)
BEGIN
    EXEC sp_addrolemember 'db_datareader', 'indigo.monitoring'
END
GO

-- Weryfikacja:
-- SELECT name FROM master.dbo.syslogins WHERE name = 'indigo.monitoring';
-- SELECT name FROM msdb.dbo.sysusers WHERE name = 'indigo.monitoring';
