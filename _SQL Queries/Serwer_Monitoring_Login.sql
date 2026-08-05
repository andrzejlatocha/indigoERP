-- Login SQL do odczytu JOB-ów SQL Agenta (msdb) dla monitoringu serwerów.
-- Uruchomić NA KAŻDYM monitorowanym serwerze SQL (typ 'SQL' w tabeli dbo.Serwer),
-- nie tylko na JAPET — SerwerController.SelectJoby łączy się tym loginem
-- bezpośrednio do msdb wskazanego serwera (Integrated Security nie działa przez VPN).
--
-- Ten skrypt jest dla SQL Server 2005 i nowszych. Dla SQL Server 2000 (brak CREATE LOGIN/
-- CREATE USER/ALTER ROLE) użyj Serwer_Monitoring_Login_SQL2000.sql zamiast tego pliku.
--
-- Login/hasło muszą się zgadzać z indigoERP.Api/appsettings.json ->
-- appSettings:MonitoringSqlLogin / MonitoringSqlPassword.
--
-- Zwykły T-SQL (bez trybu SQLCMD) — jeśli zmieniasz login/hasło poniżej,
-- zmień je też w obu miejscach (CREATE LOGIN i appsettings.json).

USE master;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'indigo.monitoring')
BEGIN
    CREATE LOGIN [indigo.monitoring]
        WITH PASSWORD = N'mUNLB0GbJQjEDbee5s2a3U6OmOOL0m7c3uIOHEarBh9I04xekx',
        CHECK_POLICY = ON, CHECK_EXPIRATION = OFF;
END
GO

USE msdb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'indigo.monitoring')
BEGIN
    CREATE USER [indigo.monitoring] FOR LOGIN [indigo.monitoring];
END
GO

-- SQLAgentReaderRole (nie SQLAgentUserRole) — pozwala widzieć WSZYSTKIE JOB-y i ich historię,
-- niezależnie od właściciela; potrzebne bo lista pokazuje joby ujb*/MaintenancePlan* niezależnie od tego,
-- kto jest ich właścicielem w SQL Agencie.
-- sp_addrolemember (nie ALTER ROLE ... ADD MEMBER, bo ta składnia jest dopiero od SQL Server 2012) —
-- sp_addrolemember działa na całym zakresie SQL Server 2005-2022.
IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members m
    JOIN sys.database_principals r ON r.principal_id = m.role_principal_id
    JOIN sys.database_principals u ON u.principal_id = m.member_principal_id
    WHERE r.name = N'SQLAgentReaderRole' AND u.name = N'indigo.monitoring'
)
BEGIN
    EXEC sp_addrolemember N'SQLAgentReaderRole', N'indigo.monitoring';
END
GO

-- Weryfikacja:
-- SELECT name, type_desc FROM sys.database_principals WHERE name = N'indigo.monitoring';
-- SELECT r.name FROM sys.database_role_members m
--     JOIN sys.database_principals r ON r.principal_id = m.role_principal_id
--     JOIN sys.database_principals u ON u.principal_id = m.member_principal_id
--     WHERE u.name = N'indigo.monitoring';
