/*
  TEST wysyłki epicaHR - jednorazowy rekord testowy.
  Wiadomość trafi do kolejki (dbo.Wiadomosc, status '2') i zostanie wysłana przez indigoERP.Worker
  na adres andrzej.latocha@huta-czestochowa.pl. Worker na JAPET musi działać.
  Wzorzec: msgID 8 ("epicaHR - powiadomienie", msgAplikacjaID 100).
  Skrypt jest idempotentny: jeśli test już istnieje, nie dodaje drugiego rekordu.

  Uruchomienie w SSMS: połącz z JAPET (uwierzytelnianie Windows), otwórz plik i naciśnij F5.
  Na końcu zobaczysz wiersz wiadomości i adresata ze statusem '2'.
*/
USE indigoERP;
GO

SET NOCOUNT ON;

IF EXISTS (SELECT 1 FROM dbo.Wiadomosc WHERE wiaTemat = 'TEST epicaHR - wysylka' AND ISNULL(Usuniety, 0) = 0)
BEGIN
    PRINT 'Rekord testowy już istnieje - pominięto.';
END
ELSE
BEGIN
    BEGIN TRANSACTION;

    DECLARE @wiaID int;

    INSERT INTO dbo.Wiadomosc (wiaAplikacjaID, wiaUzytkownikID, wiaEmail, wiaTemat, wiaTresc, wiaData,
                               wiaWzorzecID, wiaSesja, wiaStatus, wiaPubliczna)
    VALUES (100, NULL, 'andrzej.latocha@huta-czestochowa.pl', 'TEST epicaHR - wysylka',
            'Test wysylki z epicaHR. Prosze zignorowac te wiadomosc.', GETDATE(),
            8, NEWID(), '2', 0);

    SET @wiaID = SCOPE_IDENTITY();

    INSERT INTO dbo.WiadomoscPracownik (wprWiadomoscID, wprPracownikID, wprEmail, wprStatus)
    VALUES (@wiaID, NULL, 'andrzej.latocha@huta-czestochowa.pl', '2');

    COMMIT TRANSACTION;

    PRINT CONCAT('Dodano rekord testowy wiaID = ', @wiaID);
END;

SELECT w.wiaID, w.wiaAplikacjaID, w.wiaEmail, w.wiaTemat, w.wiaWzorzecID, w.wiaStatus,
       p.wprID, p.wprEmail, p.wprStatus
FROM dbo.Wiadomosc w
LEFT JOIN dbo.WiadomoscPracownik p ON p.wprWiadomoscID = w.wiaID
WHERE w.wiaTemat = 'TEST epicaHR - wysylka'
ORDER BY w.wiaID DESC;
