-- indigoERP: rola integracji epicaHR (zapis powiadomień, wiadomości, zadań i zdarzeń przez API indigoERP).
-- Uruchamia użytkownik w SSMS: najpierw CERES, potem JAPET. Skrypt idempotentny (można uruchomić ponownie).
-- Nie zawiera żadnego sekretu: tokenu epicaHR nie ma w bazie (zmienna systemowa epicaHR_ServiceClient na serwerach epicaHR).

SET NOCOUNT ON;

DECLARE @AplikacjaID int = 900; -- indigoERP (appSettings:AppID)
DECLARE @Nazwa varchar(100) = 'HCZ-indigoERP-Integracja-epicaHR';

IF NOT EXISTS (SELECT 1 FROM dbo.Rola WHERE srlNazwa = @Nazwa AND srlAplikacjaID = @AplikacjaID AND ISNULL(Usuniety, 0) = 0)
BEGIN
    INSERT INTO dbo.Rola (srlAplikacjaID, srlNazwa, srlOpis, srlBlokada)
    VALUES (@AplikacjaID, @Nazwa, 'Integracja epicaHR: zapis powiadomień, wiadomości, zadań i zdarzeń', 0);
    PRINT 'Dodano rolę ' + @Nazwa;
END
ELSE
BEGIN
    PRINT 'Rola ' + @Nazwa + ' już istnieje - bez zmian.';
END

-- Kontrola stanu (tylko odczyt).
SELECT srlID, srlAplikacjaID, srlNazwa, srlOpis, srlBlokada, Usuniety
FROM dbo.Rola
WHERE srlNazwa = @Nazwa;
