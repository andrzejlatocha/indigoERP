/*
  TEST wysyłki epicaHR - poprawny odbiorca dla workera.
  Worker czyta odbiorców wyłącznie z dbo.WiadomoscUzytkownik (join do dbo.Uzytkownik po usrID, z usrEmail i usrStatus = '2').
  Poprzedni test zapisał odbiorcę w dbo.WiadomoscPracownik, którego worker nie czyta.
  Skrypt dodaje wiersz odbiorcy dla testowej wiadomości (temat 'TEST epicaHR - wysylka') i użytkownika
  o adresie andrzej.latocha@huta-czestochowa.pl. Idempotentny: drugie uruchomienie nic nie dodaje.
  Uruchomienie w SSMS: JAPET, uwierzytelnianie Windows, F5.
*/
USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @wiaID int = (
    SELECT TOP (1) wiaID FROM dbo.Wiadomosc
    WHERE wiaTemat = 'TEST epicaHR - wysylka' AND ISNULL(Usuniety, 0) = 0
    ORDER BY wiaID DESC);

DECLARE @usrID int = (
    SELECT usrID FROM dbo.Uzytkownik
    WHERE usrEmail = 'andrzej.latocha@huta-czestochowa.pl' AND usrStatus = '2' AND ISNULL(Usuniety, 0) = 0);

IF @wiaID IS NULL
    PRINT 'Brak testowej wiadomości - najpierw uruchom indigoERP20261029140000_test_wysylki.sql.';
ELSE IF @usrID IS NULL
    PRINT 'Brak aktywnego użytkownika Uzytkownik z tym adresem e-mail (usrStatus = 2).';
ELSE IF EXISTS (SELECT 1 FROM dbo.WiadomoscUzytkownik WHERE wuzWiadomoscID = @wiaID AND wuzUzytkownikID = @usrID)
    PRINT CONCAT('Odbiorca już istnieje dla wiaID = ', @wiaID, ' - pominięto.');
ELSE
BEGIN
    INSERT INTO dbo.WiadomoscUzytkownik (wuzWiadomoscID, wuzUzytkownikID, wuzAkceptacja, wuzOdczyt, wuzStatus)
    VALUES (@wiaID, @usrID, 0, 0, '2');

    PRINT CONCAT('Dodano odbiorcę usrID = ', @usrID, ' dla wiaID = ', @wiaID);
END;

SELECT wu.wuzID, wu.wuzWiadomoscID, wu.wuzUzytkownikID, wu.wuzStatus, u.usrEmail, u.usrStatus
FROM dbo.WiadomoscUzytkownik wu
JOIN dbo.Uzytkownik u ON u.usrID = wu.wuzUzytkownikID
WHERE wu.wuzWiadomoscID = @wiaID;
