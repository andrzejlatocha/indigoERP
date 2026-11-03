/*
  Wznowienie wysyłki wiadomości 1093 ("Kobiety Grupy Kapitałowej Węglokoks - Porozmawiajmy o nas")
  do adresatów z dbo.WiadomoscPracownik (status '2'). Nadawca: itsupport@huta-czestochowa.pl.
  Skrypt zmienia dane tylko wtedy, gdy wszystkie warunki są spełnione (patrz sekcja kontroli).
  Uruchamia użytkownik w SSMS na JAPET (uwierzytelnianie Windows, F5).
*/
USE indigoERP;
GO

SET NOCOUNT ON;

DECLARE @wiaID int = 1093;
DECLARE @nadawca nvarchar(200) = N'itsupport@huta-czestochowa.pl';
DECLARE @ileOdbiorcow int = (SELECT COUNT(*) FROM dbo.WiadomoscPracownik WHERE wprWiadomoscID = @wiaID AND wprStatus = '2' AND ISNULL(wprEmail, '') <> '');
DECLARE @wzorzecOk bit = (SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.WiadomoscWzorzec WHERE msgID = w.wiaWzorzecID AND msgStatus = '2') THEN 1 ELSE 0 END
                          FROM dbo.Wiadomosc w WHERE w.wiaID = @wiaID);

-- Kontrola (tylko odczyt)
SELECT w.wiaID, w.wiaStatus, w.wiaEmail, w.wiaWzorzecID, @wzorzecOk AS WzorzecAktywny, @ileOdbiorcow AS OdbiorcyDoWysylki
FROM dbo.Wiadomosc w
WHERE w.wiaID = @wiaID;

IF EXISTS (
    SELECT 1 FROM dbo.Wiadomosc
    WHERE wiaID = @wiaID AND wiaStatus = '4' AND wiaEmail IS NULL AND wiaWzorzecID IS NOT NULL
)
AND @ileOdbiorcow > 0
AND @wzorzecOk = 1
BEGIN
    UPDATE dbo.Wiadomosc
    SET wiaEmail = @nadawca,
        wiaStatus = '2'
    WHERE wiaID = @wiaID;

    PRINT CONCAT(N'Wznowiono wiadomość ', @wiaID, N' do ', @ileOdbiorcow, N' adresatów.');
END
ELSE
BEGIN
    PRINT N'Warunki nie są spełnione - nic nie zmieniono. Sprawdź wynik kontroli powyżej.';
END;
