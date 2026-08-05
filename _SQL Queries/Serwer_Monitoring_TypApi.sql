-- Dodaje typ 'API' do słownika dic.SerwerTyp (obok WIN/SQL/ORA), żeby dało się
-- rejestrować w tabeli dbo.Serwer instancje API do monitorowania (HTTP GET zamiast
-- ping/TCP-probe — patrz ServerCheckService.CheckHttpAsync w indigoERP.Api).
-- Uruchomić na JAPET/indigoERP.Developer (i produkcyjnej bazie przy wdrożeniu).

IF NOT EXISTS (SELECT 1 FROM dic.SerwerTyp WHERE styKOD = 'API')
BEGIN
    INSERT INTO dic.SerwerTyp (styKOD, styOpis, styStatus) VALUES ('API', N'Web API', '2');
END
GO
