-- Naprawa uprawnień dla loginu monitoringu (SerwerController.SelectJoby) na msdb/JAPET.
-- Znaleziony problem: indigo.monitoring jest członkiem SQLAgentReaderRole, ale ta wbudowana
-- rola w tej instancji msdb nie ma NIC nadanego (sys.database_permissions pusty dla niej —
-- dla porównania SQLAgentUserRole ma 59 wpisów). Wygląda na niekompletną/uszkodzoną
-- konfigurację wbudowanej roli, nie coś specyficznego dla indigoERP. Zamiast próbować
-- odtwarzać wewnętrzną konfigurację roli SQL Servera, nadajemy SELECT wprost loginowi —
-- pokrywa dokładnie to, czego SerwerController.SelectJoby potrzebuje (msdb.dbo.sysjobs,
-- msdb.dbo.sysjobhistory), bez ryzyka grzebania w systemowych rolach.
-- Uruchomić na JAPET (na tej samej instancji SQL Server, którą Serwer.SelectJoby odpytuje).

USE msdb;
GO

GRANT SELECT ON msdb.dbo.sysjobs TO [indigo.monitoring];
GRANT SELECT ON msdb.dbo.sysjobhistory TO [indigo.monitoring];
GO
