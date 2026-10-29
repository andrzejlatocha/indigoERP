-- Nadanie service-to-service: indigoERP -> epicaHR, rola HCZ-epicaHR-Kadry (epicaHR: PracownikEmail/UstawEmailSluzbowy).
-- Idempotentne. Uruchamia użytkownik na indigoERP (JAPET). Wymaga wcześniej ustawionej zmiennej ServiceClient_indigoERP.
USE indigoERP;
GO

DECLARE @zrodlo int = (SELECT appID FROM dbo.Aplikacja WHERE appNazwa = N'indigoERP');
DECLARE @cel int = (SELECT appID FROM dbo.Aplikacja WHERE appNazwa = N'epicaHR');

IF @zrodlo IS NULL OR @cel IS NULL
   OR (SELECT COUNT(*) FROM dbo.Aplikacja WHERE appNazwa IN (N'indigoERP', N'epicaHR')) <> 2
BEGIN
    RAISERROR(N'Brak lub zduplikowana aplikacja indigoERP/epicaHR w dbo.Aplikacja. Przerwano.', 16, 1);
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM app.AplikacjaUslugaUprawnienie
               WHERE usgAplikacjaZrodloID = @zrodlo AND usgAplikacjaCelID = @cel AND usgKodRola = N'HCZ-epicaHR-Kadry')
BEGIN
    INSERT INTO app.AplikacjaUslugaUprawnienie (usgAplikacjaZrodloID, usgAplikacjaCelID, usgKodRola, usgAktywny, usgDataOd, usgOpis)
    VALUES (@zrodlo, @cel, N'HCZ-epicaHR-Kadry', 1, GETDATE(),
            N'Synchronizacja adresu e-mail służbowego (indigoERP -> epicaHR, PracownikKontakt)');
END;
GO

-- Kontrola (tylko odczyt)
SELECT usg.usgID, zr.appNazwa AS zrodlo, cl.appNazwa AS cel, usg.usgKodRola, usg.usgAktywny, usg.usgDataOd, usg.usgDataDo
FROM app.AplikacjaUslugaUprawnienie AS usg
JOIN dbo.Aplikacja AS zr ON zr.appID = usg.usgAplikacjaZrodloID
JOIN dbo.Aplikacja AS cl ON cl.appID = usg.usgAplikacjaCelID
WHERE zr.appNazwa = N'indigoERP' AND cl.appNazwa = N'epicaHR';
GO
