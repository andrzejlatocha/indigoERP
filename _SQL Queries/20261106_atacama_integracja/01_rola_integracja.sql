-- indigoERP: rola integracyjna HCZ-atacamaDMS-Integracja (token serwisowy indigoERP -> atacamaDMS.Api, PoleceniSluzbowe/Pdf)
-- Idempotentny: drugie uruchomienie nic nie dodaje. Uruchamia uzytkownik w SSMS na JAPET (F5).
-- srlKluczSortowania jest kolumna wyliczana - nie jest wpisywana.
USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @appAtacama int = 210;
DECLARE @appIndigo int = 900;
DECLARE @rola nvarchar(100) = N'HCZ-atacamaDMS-Integracja';

IF NOT EXISTS (SELECT 1 FROM dbo.Rola WHERE srlNazwa = @rola AND srlAplikacjaID = @appAtacama)
BEGIN
    INSERT INTO dbo.Rola (srlAplikacjaID, srlEdycja, srlNazwa, srlOpis, srlBlokada, srlStatus)
    VALUES (@appAtacama, 0, @rola, N'Integracja: token serwisowy indigoERP -> atacamaDMS.Api (PoleceniSluzbowe/Pdf)', 0, 2);
    PRINT N'Dodano role ' + @rola + N'.';
END
ELSE
    PRINT N'Rola ' + @rola + N' juz istnieje - pominieto.';
GO

DECLARE @appAtacama2 int = 210;
DECLARE @appIndigo2 int = 900;
DECLARE @rola2 nvarchar(100) = N'HCZ-atacamaDMS-Integracja';

IF NOT EXISTS (SELECT 1 FROM app.AplikacjaUslugaUprawnienie
               WHERE usgAplikacjaZrodloID = @appIndigo2 AND usgAplikacjaCelID = @appAtacama2 AND usgKodRola = @rola2)
BEGIN
    INSERT INTO app.AplikacjaUslugaUprawnienie (usgAplikacjaZrodloID, usgAplikacjaCelID, usgKodRola, usgAktywny, usgDataOd, usgDataDo, usgOpis)
    VALUES (@appIndigo2, @appAtacama2, @rola2, 1, GETDATE(), NULL, N'indigoERP -> atacamaDMS PoleceniSluzbowe.Pdf');
    PRINT N'Dodano uprawnienie indigoERP -> atacamaDMS.';
END
ELSE
    PRINT N'Uprawnienie indigoERP -> atacamaDMS juz istnieje - pominieto.';
GO

-- Kontrola (tylko odczyt)
SELECT srlID, srlAplikacjaID, srlNazwa, srlKluczSortowania, srlEdycja, srlBlokada, srlStatus
FROM dbo.Rola
WHERE srlNazwa = N'HCZ-atacamaDMS-Integracja';

SELECT usgID, usgAplikacjaZrodloID, usgAplikacjaCelID, usgKodRola, usgAktywny, usgDataOd, usgDataDo
FROM app.AplikacjaUslugaUprawnienie
WHERE usgKodRola = N'HCZ-atacamaDMS-Integracja';
GO
