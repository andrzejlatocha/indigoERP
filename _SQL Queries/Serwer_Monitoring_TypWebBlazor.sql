-- Dodaje typy 'WEB' (WebForms) i 'BLZ' (Blazor) do słownika dic.SerwerTyp (obok WIN/SQL/ORA/API),
-- żeby dało się rejestrować w tabeli dbo.Serwer strony WebForms/Blazor jako osobną kategorię
-- na dashboardzie monitoringu — sprawdzanie działa identycznie jak dla typu API (HTTP GET).
-- Uruchomić na JAPET/indigoERP.Developer (i produkcyjnej bazie przy wdrożeniu).

IF NOT EXISTS (SELECT 1 FROM dic.SerwerTyp WHERE styKOD = 'WEB')
BEGIN
    INSERT INTO dic.SerwerTyp (styKOD, styOpis, styStatus) VALUES ('WEB', N'WebForms', '2');
END

IF NOT EXISTS (SELECT 1 FROM dic.SerwerTyp WHERE styKOD = 'BLZ')
BEGIN
    INSERT INTO dic.SerwerTyp (styKOD, styOpis, styStatus) VALUES ('BLZ', N'Blazor', '2');
END
GO
